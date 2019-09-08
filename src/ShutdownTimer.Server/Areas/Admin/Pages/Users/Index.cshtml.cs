using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShutdownTimer.Server.Authorization;
using ShutdownTimer.Server.Models;

namespace ShutdownTimer.Server.Areas.Admin.Pages.Users
{
	[AuthorizeRole(WellKnownRoleNames.Administrator)]
    public class IndexModel : PageModel
    {
	    private readonly UserManager<ServiceUser> _userManager;

	    public IndexModel(UserManager<ServiceUser> userManager)
	    {
		    _userManager = userManager;
	    }

	    public List<ServiceUser> Users { get; set; } = new List<ServiceUser>();

        public void OnGet()
        {
	        Users = _userManager.Users.ToList();
        }
    }
}