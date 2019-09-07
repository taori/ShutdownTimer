using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShutdownTimer.Server.Data;
using ShutdownTimer.Server.Integrity;
using ShutdownTimer.Server.WindowsService.Areas.Identity.Data;
using ShutdownTimer.Server.WindowsService.Models;

[assembly:HostingStartup(typeof(ServerStartup))]
namespace ShutdownTimer.Server.Integrity
{
	public class ServerStartup : IHostingStartup, IStartupFilter
	{
		public void Configure(IWebHostBuilder builder)
		{
			builder.ConfigureServices(d => d.AddSingleton<IStartupFilter, ServerStartup>());
		}

		public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
		{
			return async builder =>
			{
				next(builder);

				using (var scope = builder.ApplicationServices.CreateScope())
				{
					var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

					await EnsureRoleExists(roleManager, WellKnownRoleNames.Administrator);
					await EnsureRoleExists(roleManager, WellKnownRoleNames.Client);
				}
			};
		}

		private static async Task EnsureRoleExists(RoleManager<IdentityRole> roleManager, string roleName)
		{
			var result = await roleManager.FindByNameAsync(roleName);
			if (result != null)
				return;

			if (!(await roleManager.CreateAsync(new IdentityRole(roleName))).Succeeded)
				throw new Exception($"Failed to create role {roleName}.");
		}
	}
}