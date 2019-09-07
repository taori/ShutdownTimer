using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ShutdownTimer.Server.WindowsService.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ServiceUser class
    public class ServiceUser : IdentityUser
    {
    }
}
