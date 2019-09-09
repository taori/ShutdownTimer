using System;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Authorization;
using ShutdownTimer.Server.Models;

namespace ShutdownTimer.Server.Controllers
{
	[Route("[controller]")]
	public class OperationController : Controller
	{
		private readonly ILogger<OperationController> _logger;
		private readonly ISystemControlService _systemControlService;

		public OperationController(ILogger<OperationController> logger, ISystemControlService systemControlService)
		{
			_logger = logger;
			_systemControlService = systemControlService;
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
			if (_systemControlService.AbortShutdown())
			{
				return Redirect(returnUrl ?? Url.Content("~/"));
			}
			else
			{
				return StatusCode((int) HttpStatusCode.InternalServerError);
			}
		}

		[Route("[action]")]
		[HttpPost]
		[OperationAuthorization(OperationType.IsShutdownPending)]
		public IActionResult IsShutDownPending()
		{
			var pending = _systemControlService.IsShutdownPending();
			if (pending.HasValue)
			{
				return Ok(pending.Value);
			}
			else
			{
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[Route("[action]")]
		[HttpPost]
		[OperationAuthorization(OperationType.Hibernate)]
		public IActionResult Hibernate(string returnUrl = null)
		{
			if (_systemControlService.Hibernate())
			{
				return Redirect(returnUrl ?? Url.Content("~/"));
			}
			else
			{
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[Route("[action]")]
		[HttpPost]
		[OperationAuthorization(OperationType.Restart)]
		public IActionResult Restart(string returnUrl = null)
		{
			if (_systemControlService.Restart())
			{
				return Redirect(returnUrl ?? Url.Content("~/"));
			}
			else
			{
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[Route("[action]")]
		[HttpPost]
		[OperationAuthorization(OperationType.Logout)]
		public IActionResult Logout(string returnUrl = null)
		{
			if (_systemControlService.Logout())
			{
				return Redirect(returnUrl ?? Url.Content("~/"));
			}
			else
			{
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}
	}
}