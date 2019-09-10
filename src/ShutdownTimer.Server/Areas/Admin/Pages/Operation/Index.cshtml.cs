using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Authorization;
using ShutdownTimer.Server.Extensions;

namespace ShutdownTimer.Server.Areas.Admin.Pages.Operation
{
	public class IndexModel : PageModel
	{
		private readonly IEnumerable<IOperationProvider> _operationProviders;

		public IndexModel(IEnumerable<IOperationProvider> operationProviders)
		{
			_operationProviders = operationProviders;
		}

		public List<Claim> Options { get; set; } = new List<Claim>();

		public void OnGet()
		{
			Options = _operationProviders
				.SelectMany(d => d.GetOperations())
				.Select(d => new Claim(OperationAuthorizationAttribute.ClaimPrefix, d))
				.ToList();
		}
	}
}