using System.ComponentModel.DataAnnotations;

namespace ShutdownTimer.Server.Data
{
	public class CustomCommand
	{
		public int Id { get; set; }

		public string CommandName { get; set; }

		[Required]
		public string Program { get; set; }

		public string Argument { get; set; }

		public string PermissionName { get; set; }
	}
}