using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShutdownTimer.Server.Authorization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class AuthorizeRoleAttribute : Attribute, IAsyncAuthorizationFilter
	{
		public string RoleName { get; set; }

		public AuthorizeRoleAttribute(string roleName)
		{
			RoleName = roleName;
		}

		public Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			if (context.HttpContext.User.IsInRole(RoleName))
				return Task.CompletedTask;

			context.Result = new ForbidResult();

			return Task.CompletedTask;
		}
	}
}