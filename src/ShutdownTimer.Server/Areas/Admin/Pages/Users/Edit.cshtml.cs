using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Authorization;
using ShutdownTimer.Server.Data;
using ShutdownTimer.Server.Models;

namespace ShutdownTimer.Server.Areas.Admin.Pages.Users
{
	public class EditModel : PageModel
	{
		private readonly UserManager<ServiceUser> _userManager;

		private readonly IEnumerable<IOperationProvider> _operationProviders;

		public EditModel(UserManager<ServiceUser> userManager, IEnumerable<IOperationProvider> operationProviders)
		{
			_userManager = userManager;
			_operationProviders = operationProviders;
		}

		public class InputModel
		{
			public List<TypedCheckboxOption<string>> AvailableOperations { get; set; } = new List<TypedCheckboxOption<string>>();
		}

		public ServiceUser CurrentUser { get; set; }

		[BindProperty(Name = "id", SupportsGet = true)]
		public Guid UserId { get; set; }

		[BindProperty(SupportsGet = true)]
		public string ReturnUrl { get; set; }

		[BindProperty]
		public InputModel Input { get; set; } = new InputModel();

		public IEnumerable<Claim> GetAvailableOperationClaims()
		{
			return _operationProviders
				.SelectMany(d => d.GetOperations())
				.Select(d => new Claim(OperationAuthorizationAttribute.ClaimPrefix, d));
		}

		public async Task<IActionResult> OnPostSaveAsync()
		{
			if (!ModelState.IsValid)
				return Page();

			var user = await _userManager.FindByIdAsync(UserId.ToString());

			var removeClaims = GetAvailableOperationClaims();
			var removal = await _userManager.RemoveClaimsAsync(user, removeClaims);
			if (!removal.Succeeded)
			{
				ModelState.AddModelError(string.Empty, string.Join("<br/>", removal.Errors.Select(d => d.Description)));
				return Page();
			}

			var addClaims = Input.AvailableOperations
				.Where(d => d.Checked)
				.Select(d => new Claim(OperationAuthorizationAttribute.ClaimPrefix, d.Value.ToString()));
			var addition = await _userManager.AddClaimsAsync(user, addClaims);
			if (!addition.Succeeded)
			{
				ModelState.AddModelError(string.Empty, string.Join("<br/>", removal.Errors.Select(d => d.Description)));
				return Page();
			}

			return Redirect(ReturnUrl ?? Url.Content("~/"));
		}

		public async Task OnGetAsync()
		{
			CurrentUser = await _userManager.FindByIdAsync(UserId.ToString());
			var claims = await _userManager.GetClaimsAsync(CurrentUser);

			var grantedOperationClaims = claims
				.Where(d => d.Type == OperationAuthorizationAttribute.ClaimPrefix)
				.Select(d => d.Value)
				.ToHashSet();

			Input.AvailableOperations = GetAvailableOperationClaims()
					.Select(d => new TypedCheckboxOption<string>(d.ToString(), grantedOperationClaims.Contains(d.Value), d.Value))
					.ToList();
		}
	}
}