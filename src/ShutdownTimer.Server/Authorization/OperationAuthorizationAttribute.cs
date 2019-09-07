using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using ShutdownTimer.Server.WindowsService.Models;

namespace ShutdownTimer.Server.WindowsService.Authorization
{
	public class OperationAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
	{
		public OperationType OperationType { get; }

		public const string ClaimPrefix = "operationClaim:";

		public OperationAuthorizationAttribute(OperationType operationType)
		{
			OperationType = operationType;
		}

		public Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			return Task.FromResult(context.HttpContext.User.IsInRole(WellKnownRoleNames.Administrator) || context.HttpContext.User.HasClaim(ClaimPrefix, OperationType.ToString()));
		}
	}
}