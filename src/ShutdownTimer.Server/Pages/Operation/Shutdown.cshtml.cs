using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Authorization;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Pages.Operation
{
	[OperationAuthorization(OperationType.Shutdown)]
	public class ShutdownModel : PageModel
	{
		private readonly IShutdownHistoryService _shutdownHistoryService;
		private readonly ISystemControlService _systemControlService;

		public ShutdownModel(IShutdownHistoryService shutdownHistoryService, ISystemControlService systemControlService)
		{
			_shutdownHistoryService = shutdownHistoryService;
			_systemControlService = systemControlService;
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

		public List<ShutdownHistoryItem> HistoryItems { get; set; }

		public async Task OnGetAsync(string returnUrl = null)
		{
			HistoryItems = await _shutdownHistoryService.GetHistoryForUserAsync(HttpContext.User);
			ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostHistoryShutdownAsync(int hour, int minute)
		{
			if (!ModelState.IsValid)
				return Page();

			var timeSpan = new TimeSpan(hour, minute, 0);
			if (_systemControlService.ShutDown(timeSpan))
			{
				await _shutdownHistoryService.LogShutdownAsync(HttpContext.User, timeSpan);
				return Redirect(ReturnUrl ?? Url.Content("~/"));
			}
			else
			{
				ModelState.AddModelError(string.Empty, "An error occured while trying to shut down.");
				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
				return Page();

			var timeSpan = new TimeSpan(Input.Hours, Input.Minutes, 0);
			if (_systemControlService.ShutDown(timeSpan))
			{
				await _shutdownHistoryService.LogShutdownAsync(HttpContext.User, timeSpan);
				return Redirect(ReturnUrl ?? Url.Content("~/"));
			}
			else
			{
				ModelState.AddModelError(string.Empty, "An error occured while trying to shut down.");
				return Page();
			}
		}
	}
}