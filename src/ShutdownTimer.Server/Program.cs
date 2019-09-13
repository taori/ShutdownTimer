using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using ShutdownTimer.Server.Extensions;

namespace ShutdownTimer.Server
{
	public class Program
	{
		// https://docs.microsoft.com/de-de/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-2.2&tabs=visual-studio
		public static void Main(string[] args)
		{
			var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
			try
			{
				var isService = !(Debugger.IsAttached || args.Contains("--console"));

				if (isService)
				{
					var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
					var pathToContentRoot = Path.GetDirectoryName(pathToExe);
					Directory.SetCurrentDirectory(pathToContentRoot);
				}

				var builder = CreateWebHostBuilder(args.Where(arg => arg != "--console").ToArray())
					.UseKestrel();

				var host = builder.Build();

				if (isService)
				{
					// To run the app without the CustomWebHostService change the
					// next line to host.RunAsService();
					host.RunAsCustomService();
				}
				else
				{
					host.Run();
				}
			}
			catch (Exception e)
			{
				logger.Error(e, $"Stopped program because of an exception.");
			}
			finally
			{
				System.Text.RegularExpressions.Regex
				NLog.LogManager.Shutdown();
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, config) =>
				{
					config.AddJsonFile("appsettings.postdeployment.json", true, true);
					config.AddJsonFile("appsettings.{Environment}.postdeployment.json", true, true);
					config.AddCommandLine(args);
				})
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.AddEventLog();
					logging.AddNLog();
				})
				.UseKestrel()
				.UseNLog()
				.UseStartup<Startup>();
	}
}
