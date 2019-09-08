using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Models
{
    // Add profile data for application users by adding properties to the ServiceUser class
    public class ServiceUser : IdentityUser, ICreatedStamp
    {
	    public DateTime Created { get; set; }
    }
}
