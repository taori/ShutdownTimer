using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShutdownTimer.Server.Authorization;
using ShutdownTimer.Server.Data;
using ShutdownTimer.Server.Models;

namespace ShutdownTimer.Server.Areas.Admin.Pages.Users
{
	public class EditModel : PageModel
	{
		public UserManager<ServiceUser> UserManager { get; }

		public EditModel(UserManager<ServiceUser> userManager)
		{
			UserManager = userManager;
		}

		public class InputModel
		{
			public List<TypedCheckboxOption<OperationType>> AvailableOperations { get; set; } = new List<TypedCheckboxOption<OperationType>>();
		}

		public ServiceUser CurrentUser { get; set; }

		[BindProperty(Name = "id", SupportsGet = true)]
		public Guid UserId { get; set; }

		[BindProperty(SupportsGet = true)]
		public string ReturnUrl { get; set; }

		[BindProperty]
		public InputModel Input { get; set; } = new InputModel();

		public async Task<IActionResult> OnPostSaveAsync()
		{
			if (!ModelState.IsValid)
				return Page();

			var user = await UserManager.FindByIdAsync(UserId.ToString());

			var removeClaims = Enum.GetValues(typeof(OperationType))
				.Cast<OperationType>()
				.Select(d => new Claim(OperationAuthorizationAttribute.ClaimPrefix, d.ToString()));
			var removal = await UserManager.RemoveClaimsAsync(user, removeClaims);
			if (!removal.Succeeded)
			{
				ModelState.AddModelError(string.Empty, string.Join("<br/>", removal.Errors.Select(d => d.Description)));
				return Page();
			}

			var addClaims = Input.AvailableOperations
				.Where(d => d.Checked)
				.Select(d => new Claim(OperationAuthorizationAttribute.ClaimPrefix, d.Value.ToString()));
			var addition = await UserManager.AddClaimsAsync(user, addClaims);
			if (!addition.Succeeded)
			{
				ModelState.AddModelError(string.Empty, string.Join("<br/>", removal.Errors.Select(d => d.Description)));
				return Page();
			}

			return Redirect(ReturnUrl ?? Url.Content("~/"));
		}

		public async Task OnGetAsync()
		{
			CurrentUser = await UserManager.FindByIdAsync(UserId.ToString());
			var claims = await UserManager.GetClaimsAsync(CurrentUser);

			var grantedOperationClaims = claims
				.Where(d => d.Type == OperationAuthorizationAttribute.ClaimPrefix)
				.Select(d => Enum.Parse<OperationType>(d.Value))
				.ToHashSet();

			Input.AvailableOperations =
				Enum.GetValues(typeof(OperationType))
					.Cast<OperationType>()
					.Select(d => new TypedCheckboxOption<OperationType>(d.ToString(), grantedOperationClaims.Contains(d), d))
					.ToList();
		}
	}
}