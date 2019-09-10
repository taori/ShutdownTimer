using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Pages.Operation
{
    public class IndexModel : PageModel
    {
	    private readonly ICustomCommandService _commandService;

	    public IndexModel(ICustomCommandService commandService)
	    {
		    _commandService = commandService;
	    }

	    public List<CustomCommand> CustomCommands { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
	        CustomCommands = await _commandService.GetAllAsync();
	        return Page();
        }
    }
}