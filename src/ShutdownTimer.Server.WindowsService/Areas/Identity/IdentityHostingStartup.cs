using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShutdownTimer.Server.WindowsService.Areas.Identity.Data;
using ShutdownTimer.Server.WindowsService.Models;

[assembly: HostingStartup(typeof(ShutdownTimer.Server.WindowsService.Areas.Identity.IdentityHostingStartup))]
namespace ShutdownTimer.Server.WindowsService.Areas.Identity
{
	public class IdentityHostingStartup : IHostingStartup
	{
		public void Configure(IWebHostBuilder builder)
		{
			builder.ConfigureServices(async (context, services) =>
			{
				services.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(
						context.Configuration.GetConnectionString("ApplicationContextConnection")));

				services.AddDefaultIdentity<ServiceUser>(options =>
					{
						options.Password.RequireDigit = false;
						options.Password.RequireUppercase = false;
						options.Password.RequireNonAlphanumeric = false;
						options.Password.RequiredLength = 4;
					})
					.AddRoles<IdentityRole>()
					.AddEntityFrameworkStores<ApplicationDbContext>();

				using (var scope = services.BuildServiceProvider().CreateScope())
				{
					var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

					await EnsureRoleExists(roleManager, WellKnownRoleNames.Administrator);
					await EnsureRoleExists(roleManager, WellKnownRoleNames.Client);
				}
			});
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