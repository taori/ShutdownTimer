﻿using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShutdownTimer.Server.Abstraction;

namespace ShutdownTimer.Server.Services
{
	public class SystemControlService : ISystemControlService
	{
		private readonly ILogger<SystemControlService> _logger;

		private readonly ICustomCommandService _customCommandService;

		public SystemControlService(ILogger<SystemControlService> logger, ICustomCommandService customCommandService)
		{
			_logger = logger;
			_customCommandService = customCommandService;
		}

		public bool AbortShutdown()
		{
			_logger.LogInformation($"Executing {nameof(AbortShutdown)}.");
			try
			{
				using (var process = Process.Start("shutdown", $"/a"))
				{
					process.WaitForExit();
					return true;
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return false;
			}
		}

		public bool? IsShutdownPending()
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

					return isPending;
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return null;
			}
		}

		public bool Hibernate()
		{
			_logger.LogInformation($"Executing Hibernate.");
			try
			{
				using (var process = Process.Start("shutdown", $"/h"))
				{
					process.WaitForExit();
					return true;
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return false;
			}
		}

		public bool Restart()
		{
			_logger.LogInformation($"Executing Restart.");
			try
			{
				using (var process = Process.Start("shutdown", $"/r"))
				{
					process.WaitForExit();
					return true;
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return false;
			}
		}

		public bool Logout()
		{
			_logger.LogInformation($"Executing Restart.");
			try
			{
				using (var process = Process.Start("shutdown", $"/l"))
				{
					process.WaitForExit();
					return true;
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return false;
			}
		}

		public bool ShutDown(TimeSpan delay)
		{
			_logger.LogInformation($"Executing Shutdown in {delay.Hours}:{delay.Minutes}.");
			try
			{
				using (var process = Process.Start("shutdown", $"/s /t {delay.TotalSeconds} /d u:0:0"))
				{
					process.WaitForExit();
					return true;
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return false;
			}
		}

		public async Task<bool> ExecuteCustomCommandAsync(int id)
		{
			_logger.LogInformation($"Executing custom command [{id}].");

			var command = await _customCommandService.GetAsync(id);
			if (command == null)
			{
				_logger.LogError($"Command not found.");
				return false;
			}
				
			try
			{
				using (var process = Process.Start(command.Program, command.Argument))
				{
					process.WaitForExit();
					return true;
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e.ToString());
				return false;
			}
		}
	}
}