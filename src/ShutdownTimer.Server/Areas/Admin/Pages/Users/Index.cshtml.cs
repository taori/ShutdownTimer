using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
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

        public async Task<IActionResult> OnGetDeleteAsync(Guid id)
        {
	        var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ServiceUser>>();
	        var user = await userManager.FindByIdAsync(id.ToString());
	        if (user != null)
			{
				if (!(await userManager.DeleteAsync(user)).Succeeded)
				{
					ModelState.AddModelError(string.Empty, "Failed to delete user.");
					return Page();
				}
			}
	        else
			{
				ModelState.AddModelError(string.Empty, "User not found.");
				return Page();
			}

	        return RedirectToPage("Index");
        }
    }
}