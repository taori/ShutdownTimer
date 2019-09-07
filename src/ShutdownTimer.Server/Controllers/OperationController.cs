using System;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShutdownTimer.Server.WindowsService.Areas.Identity.Data;
using ShutdownTimer.Server.WindowsService.Authorization;
using ShutdownTimer.Server.WindowsService.Extensions;

namespace ShutdownTimer.Server.WindowsService.Controllers
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
		[Route("[action]/{seconds}")]
		[OperationAuthorization(OperationType.Shutdown)]
		public IActionResult Shutdown(int seconds)
		{
			_logger.LogInformation($"Executing Shutdown {seconds}.");
			try
			{
				using (var process = Process.Start("shutdown", $"/s /t {seconds} /d u:0:0"))
				{
					process.WaitForExit();
					return Ok();
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return StatusCode((int) HttpStatusCode.InternalServerError);
			}
		}

		[HttpGet]
		[Route("[action]")]
		[OperationAuthorization(OperationType.AbortShutdown)]
		public IActionResult AbortShutdown()
		{
			_logger.LogInformation($"Executing AbortShutdown.");
			try
			{
				using (var process = Process.Start("shutdown", $"/a"))
				{
					process.WaitForExit();
					return Ok();
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
		[HttpGet]
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
		[HttpGet]
		[OperationAuthorization(OperationType.Hibernate)]
		public IActionResult Hibernate()
		{
			_logger.LogInformation($"Executing Hibernate.");
			try
			{
				using (var process = Process.Start("shutdown", $"/h"))
				{
					process.WaitForExit();
					return Ok();
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[Route("[action]")]
		[HttpGet]
		[OperationAuthorization(OperationType.Restart)]
		public IActionResult Restart()
		{
			_logger.LogInformation($"Executing Restart.");
			try
			{
				using (var process = Process.Start("shutdown", $"/r"))
				{
					process.WaitForExit();
					return Ok();
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[Route("[action]")]
		[HttpGet]
		[OperationAuthorization(OperationType.Logout)]
		public IActionResult Logout()
		{
			_logger.LogInformation($"Executing Restart.");
			try
			{
				using (var process = Process.Start("shutdown", $"/l"))
				{
					process.WaitForExit();
					return Ok();
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