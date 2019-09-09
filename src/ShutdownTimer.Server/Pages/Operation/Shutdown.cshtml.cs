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
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Authorization;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Pages.Operation
{
	[OperationAuthorization(OperationType.Shutdown)]
	public class ShutdownModel : PageModel
	{
		private readonly ILogger<ShutdownModel> _logger;
		private readonly IShutdownHistoryService _shutdownHistoryService;

		public ShutdownModel(ILogger<ShutdownModel> logger, IShutdownHistoryService shutdownHistoryService)
		{
			_logger = logger;
			_shutdownHistoryService = shutdownHistoryService;
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

			_logger.LogInformation($"Executing Shutdown in {hour}:{minute}.");
			try
			{
				var ts = TimeSpan.FromHours(hour).Add(TimeSpan.FromMinutes(minute));
				using (var process = Process.Start("shutdown", $"/s /t {ts.TotalSeconds} /d u:0:0"))
				{
					process.WaitForExit();
					await _shutdownHistoryService.LogShutdownAsync(HttpContext.User, ts);
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

		public async Task<IActionResult> OnPostAsync()
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
					await _shutdownHistoryService.LogShutdownAsync(HttpContext.User, ts);
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