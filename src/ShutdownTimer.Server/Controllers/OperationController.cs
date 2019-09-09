using System;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShutdownTimer.Server.Authorization;
using ShutdownTimer.Server.Models;

namespace ShutdownTimer.Server.Controllers
{
	[Route("[controller]")]
	public class OperationController : Controller
	{
		private readonly ILogger<OperationController> _logger;

		public OperationController(ILogger<OperationController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		[Route("[action]")]
		[Authorize]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[Route("[action]")]
		[OperationAuthorization(OperationType.AbortShutdown)]
		public IActionResult AbortShutdown(string returnUrl = null)
		{
			_logger.LogInformation($"Executing AbortShutdown.");
			try
			{
				using (var process = Process.Start("shutdown", $"/a"))
				{
					process.WaitForExit();
					return Redirect(returnUrl ?? Url.Content("~/"));
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[Route("[action]")]
		[ProducesResponseType(typeof(bool), 200)]
		[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
		[HttpPost]
		[OperationAuthorization(OperationType.IsShutdownPending)]
		public IActionResult IsShutDownPending()
		{
			_logger.LogInformation($"Executing IsShutDownPending.");
			try
			{
				using (var process = Process.Start("shutdown", $"/s /t 500"))
				{
					process.WaitForExit();
					var status = process.ExitCode;
					var isPending = status == 1190;
					if (!isPending)
					{
						using (var subProcess = Process.Start("shutdown", $"/a"))
						{
							subProcess.WaitForExit();
						}
					}
					return Ok(isPending);
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[Route("[action]")]
		[HttpPost]
		[OperationAuthorization(OperationType.Hibernate)]
		public IActionResult Hibernate(string returnUrl = null)
		{
			_logger.LogInformation($"Executing Hibernate.");
			try
			{
				using (var process = Process.Start("shutdown", $"/h"))
				{
					process.WaitForExit();
					return Redirect(returnUrl ?? Url.Content("~/"));
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[Route("[action]")]
		[HttpPost]
		[OperationAuthorization(OperationType.Restart)]
		public IActionResult Restart(string returnUrl = null)
		{
			_logger.LogInformation($"Executing Restart.");
			try
			{
				using (var process = Process.Start("shutdown", $"/r"))
				{
					process.WaitForExit();
					return Redirect(returnUrl ?? Url.Content("~/"));
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[Route("[action]")]
		[HttpPost]
		[OperationAuthorization(OperationType.Logout)]
		public IActionResult Logout(string returnUrl = null)
		{
			_logger.LogInformation($"Executing Restart.");
			try
			{
				using (var process = Process.Start("shutdown", $"/l"))
				{
					process.WaitForExit();
					return Redirect(returnUrl ?? Url.Content("~/"));
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}
	}
}