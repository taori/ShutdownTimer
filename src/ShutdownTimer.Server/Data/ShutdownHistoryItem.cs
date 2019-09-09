using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShutdownTimer.Server.Models;

namespace ShutdownTimer.Server.Data
{
	public class ShutdownHistoryItem
	{
		public DateTime LastExecution { get; set; }

		[ForeignKey(nameof(UserId))]
		public ServiceUser User { get; set; }

		public string UserId { get; set; }

		public int Hours { get; set; }

		public int Minutes { get; set; }
	}
}