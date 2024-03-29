﻿using System.ComponentModel;
using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ShutdownTimer.Server.Extensions
{
	[DesignerCategory("Code")]
	internal class CustomWebHostService : WebHostService
	{
		private ILogger _logger;
		private UdpClient _udpClient = new UdpClient();

		public CustomWebHostService(IWebHost host) : base(host)
		{
			_logger = host.Services
				.GetRequiredService<ILogger<CustomWebHostService>>();
		}

		protected override void OnStarting(string[] args)
		{
			_logger.LogInformation("OnStarting method called.");
			base.OnStarting(args);
		}

		protected override void OnStarted()
		{
			_logger.LogInformation("OnStarted method called.");
//			var client = new UdpClient();
//			client.
			base.OnStarted();
		}

		protected override void OnStopping()
		{
			_logger.LogInformation("OnStopping method called.");
			base.OnStopping();
		}
	}
}