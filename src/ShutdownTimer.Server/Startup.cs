using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ShutdownTimer.Server.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Models;
using ShutdownTimer.Server.Services;

namespace ShutdownTimer.Server
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
		{
			Configuration = configuration;
			HostingEnvironment = hostingEnvironment;
		}

		public class Fu : ITrackingConsentFeature
		{
			public IHttpContextAccessor Accessor { get; }

			public Fu(IHttpContextAccessor accessor)
			{
				Accessor = accessor;
			}

			public void GrantConsent()
			{
				Accessor.HttpContext.Response.Cookies.Append("wellfuck","shit");
			}

			public void WithdrawConsent()
			{
				Accessor.HttpContext.Response.Cookies.Delete("wellfuck");
			}

			public string CreateConsentCookie()
			{
				return "shit";
			}

			public bool IsConsentNeeded => true;

			public bool HasConsent
			{
				get
				{
					return Accessor.HttpContext.Request.Cookies.TryGetValue("wellfuck", out var val) &&
					       val == "shit";
				}
			}

			public bool CanTrack => true;
		}

		public IConfiguration Configuration { get; }
		public IHostingEnvironment HostingEnvironment { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});
			services.AddScoped<IShutdownHistoryService, ShutdownHistoryService>();
			services.AddScoped<ISystemControlService, SystemControlService>();
			services.AddScoped<IOperationProvider, EnumOperationProvider>();
			services.AddScoped<IOperationProvider, CustomCommandService>();
			services.AddScoped<ICustomCommandService, CustomCommandService>();
			services.AddScoped<ICommandProvider, CustomCommandProvider>();

			if (HostingEnvironment.IsDevelopment())
			{
				services.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(
						Configuration.GetConnectionString("ApplicationContextConnection")));
			}
			else
			{
				services.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlite(
						Configuration.GetConnectionString("ApplicationContextConnection").Replace("%root%", HostingEnvironment.ContentRootPath)));
			}


			services.AddDefaultIdentity<ServiceUser>(options =>
				{
					options.Password.RequireDigit = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequiredLength = 4;
				})
				.AddRoles<IdentityRole>()
				.AddDefaultUI(UIFramework.Bootstrap4)
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services
				.AddMvc(options =>
				{
				})
				.AddRazorPagesOptions(options =>
				{
					options.AllowAreas = true;
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseAuthentication();
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
