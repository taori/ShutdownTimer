using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShutdownTimer.Server.Data;
using ShutdownTimer.Server.Integrity;
using ShutdownTimer.Server.Models;

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
					var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
					await dbContext.Database.MigrateAsync();
					var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
					
					await EnsureRoleExists(roleManager, WellKnownRoleNames.Administrator);
					await EnsureRoleExists(roleManager, WellKnownRoleNames.Client);
					
					var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ServiceUser>>();
					await EnsureAdminExistsAsync(userManager);
				}
			};
		}

		private static async Task EnsureAdminExistsAsync(UserManager<ServiceUser> userManager)
		{
			var admins = await userManager.GetUsersInRoleAsync(WellKnownRoleNames.Administrator);
			if (admins.Count == 0)
			{
				var user = await userManager.Users.OrderBy(d => d.Created).FirstAsync();
				await userManager.AddToRoleAsync(user, WellKnownRoleNames.Administrator);
			}
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