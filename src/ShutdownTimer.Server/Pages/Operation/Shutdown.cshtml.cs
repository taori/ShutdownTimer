using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ShutdownTimer.Server.Authorization;

namespace ShutdownTimer.Server.Pages.Operation
{
	[OperationAuthorization(OperationType.Shutdown)]
	public class ShutdownModel : PageModel
	{
		private readonly ILogger<ShutdownModel> _logger;

		public ShutdownModel(ILogger<ShutdownModel> logger)
		{
			_logger = logger;
		}

		public class InputModel
		{
			[Required]
			[Range(0, 24)]
			public int Hours { get; set; }

			[Required]
			[Range(0, 60)]
			public int Minutes { get; set; }
		}

		[BindProperty]
		public InputModel Input { get; set; } = new InputModel();

		[BindProperty(SupportsGet = true)]
		public string ReturnUrl { get; set; }

		public void OnGet(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
		}

		public IActionResult OnPost()
		{
			if (!ModelState.IsValid)
				return Page();

			_logger.LogInformation($"Executing Shutdown in {Input.Hours}:{Input.Minutes}.");
			try
			{
				var ts = TimeSpan.FromHours(Input.Hours).Add(TimeSpan.FromMinutes(Input.Minutes));
				using (var process = Process.Start("shutdown", $"/s /t {ts.TotalSeconds} /d u:0:0"))
				{
					process.WaitForExit();
					return Redirect(ReturnUrl ?? Url.Content("~/"));
				}
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, "An error occured while trying to shut down.");
				_logger.LogError(e.ToString());
				return Page();
			}
		}
	}
}