using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShutdownTimer.Server.Models;

namespace ShutdownTimer.Server.Authorization
{
	public class OperationAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
	{
		public OperationType OperationType { get; }

		public const string ClaimPrefix = "operationClaim";

		public OperationAuthorizationAttribute(OperationType operationType)
		{
			OperationType = operationType;
		}

		public static string RenderClaim(OperationType type)
		{
			return $"{ClaimPrefix}:{type.ToString()}";
		}

		public Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			var passed = context.HttpContext.User.IsInRole(WellKnownRoleNames.Administrator) || context.HttpContext.User.HasClaim(ClaimPrefix, OperationType.ToString());
			if (!passed)
				context.Result = new ForbidResult();

			return Task.CompletedTask;
		}
	}
}