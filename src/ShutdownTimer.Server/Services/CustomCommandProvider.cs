using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Services
{
	public class CustomCommandProvider : ICommandProvider
	{
		private readonly ApplicationDbContext _dbContext;

		public CustomCommandProvider(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IEnumerable<CustomCommand> GetCommands()
		{
			return _dbContext.CustomCommands.ToList();
		}
	}
}