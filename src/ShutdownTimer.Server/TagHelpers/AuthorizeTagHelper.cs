using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ShutdownTimer.Server.Models;

namespace ShutdownTimer.Server.TagHelpers
{
	[HtmlTargetElement(Attributes = "asp-authorize")]
	public class AuthorizeTagHelper : TagHelper
	{
		public IHttpContextAccessor HttpContextAccessor { get; }

		public AuthorizeTagHelper(IHttpContextAccessor httpContextAccessor)
		{
			HttpContextAccessor = httpContextAccessor;
		}

		/// <summary>
		/// Verifies claims in a pattern like type1:value1 or type1:value1,type2:value2
		/// </summary>
		[HtmlAttributeName("asp-claims")]
		public string Claims { get; set; }

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			if (string.IsNullOrEmpty(Claims))
			{
				await base.ProcessAsync(context, output);
				return;
			}

			if (!HasRequiredClaims())
			{
				output.SuppressOutput();
			}

			await base.ProcessAsync(context, output);
		}

		private bool HasRequiredClaims()
		{
			if (HttpContextAccessor.HttpContext.User.IsInRole(WellKnownRoleNames.Administrator))
				return true;

			var claimPairs = Claims.Split(',', StringSplitOptions.RemoveEmptyEntries);
			var allMatched = claimPairs
				.Select(d => d.Split(':'))
				.All(d => HttpContextAccessor.HttpContext.User.HasClaim(d[0], d[1]));

			return allMatched;
		}
	}
}