using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Areas.Admin.Pages.Commands
{
	public class IndexModel : PageModel
	{
		private readonly ICustomCommandService _customCommandService;

		public IndexModel(ICustomCommandService customCommandService)
		{
			_customCommandService = customCommandService;
		}

		public List<CustomCommand> Commands { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			Commands = await _customCommandService.GetAllAsync();
			return Page();
		}

		public async Task<IActionResult> OnGetDeleteAsync(int id)
		{
			await _customCommandService.DeleteAsync(id);
			Commands = await _customCommandService.GetAllAsync();
			return Page();
		}
	}
}