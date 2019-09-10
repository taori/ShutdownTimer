using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Extensions;

namespace ShutdownTimer.Server.Areas.Admin.Pages.Commands
{
	public class AddModel : PageModel
	{
		private readonly ICustomCommandService _customCommandService;

		public class InputModel
		{
			public string CommandName { get; set; }

			public string ProgramName { get; set; }

			public string Argument { get; set; }

			public string PermissionName { get; set; }
		}

		public AddModel(ICustomCommandService customCommandService)
		{
			_customCommandService = customCommandService;
		}

		[BindProperty(SupportsGet = true)]
		public string ReturnUrl { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if(await _customCommandService.AddAsync(Input.CommandName, Input.ProgramName, Input.Argument, Input.PermissionName) is var add && !add.Succeeded)
			{
				ModelState.FromDescriptiveResult(add);
				return Page();
			}

			return Redirect(ReturnUrl ?? Url.Content("~/"));
		}
	}
}