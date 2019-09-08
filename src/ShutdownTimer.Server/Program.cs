﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShutdownTimer.Server.Extensions;

namespace ShutdownTimer.Server
{
	public class Program
	{
		// https://docs.microsoft.com/de-de/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-2.2&tabs=visual-studio
		public static void Main(string[] args)
		{
			var isService = !(Debugger.IsAttached || args.Contains("--console"));

			if (isService)
			{
				var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
				var pathToContentRoot = Path.GetDirectoryName(pathToExe);
				Directory.SetCurrentDirectory(pathToContentRoot);
			}

			var builder = CreateWebHostBuilder(
				args.Where(arg => arg != "--console").ToArray());

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

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.AddEventLog();
				})
				.ConfigureAppConfiguration((context, config) =>
				{
					// Configure the app here.
				})
				.UseStartup<Startup>();

//		public static void Main(string[] args)
//		{
//			CreateWebHostBuilder(args).Build().Run();
//		}
//
//		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
//			WebHost.CreateDefaultBuilder(args)
//				.UseStartup<Startup>();
	}
}
