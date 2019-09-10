using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Authorization;
using ShutdownTimer.Server.Models;

namespace ShutdownTimer.Server.Areas.Admin.Pages
{
	[AuthorizeRole(WellKnownRoleNames.Administrator)]
	public class OptionsModel : PageModel
	{
		public OptionsModel()
		{
		}

		public void OnGet()
		{
		}
	}
}