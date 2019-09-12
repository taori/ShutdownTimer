using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShutdownTimer.Server.Integrity;
using ShutdownTimer.Server.Models;

namespace ShutdownTimer.Server.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<ServiceUser> _signInManager;
		private readonly UserManager<ServiceUser> _userManager;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;
		private readonly IOptions<DatabaseFeatureSettings> _databaseFeatureSettings;

		public RegisterModel(
			UserManager<ServiceUser> userManager,
			SignInManager<ServiceUser> signInManager,
			ILogger<RegisterModel> logger,
			IEmailSender emailSender, 
			IOptions<DatabaseFeatureSettings> databaseFeatureSettings)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
			_emailSender = emailSender;
			_databaseFeatureSettings = databaseFeatureSettings;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public string ReturnUrl { get; set; }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			[Display(Name = "Email")]
			public string Email { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "Password")]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Confirm password")]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; }
		}

		public void OnGet(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
		}

		public async Task<bool> CreateUserAsync()
		{
			var user = new ServiceUser() { UserName = Input.Email, Email = Input.Email };
			var result = await _userManager.CreateAsync(user, Input.Password);
			if (result.Succeeded)
			{
				if (await _userManager.Users.CountAsync() == 1)
				{
					_logger.LogInformation($"Assigning {user.UserName} to role {WellKnownRoleNames.Administrator}.");
					await _userManager.AddToRoleAsync(user, WellKnownRoleNames.Administrator);
				}
				else
				{
					await _userManager.AddToRoleAsync(user, WellKnownRoleNames.Client);
				}

				_logger.LogInformation("User created a new account with password.");

				var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				var callbackUrl = Url.Page(
					"/Account/ConfirmEmail",
					pageHandler: null,
					values: new { userId = user.Id, code = code },
					protocol: Request.Scheme);

				await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
					$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

				await _signInManager.SignInAsync(user, isPersistent: false);

				return true;
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return false;
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl = returnUrl ?? Url.Content("~/");
			if (ModelState.IsValid)
			{
				if (_databaseFeatureSettings.Value.SupportsTransactionScope)
				{
					using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
					{
						if (await CreateUserAsync())
						{
							scope.Complete();
							return LocalRedirect(returnUrl);
						}
					}
				}
				else
				{
					if (await CreateUserAsync())
						return LocalRedirect(returnUrl);
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}
	}
}
