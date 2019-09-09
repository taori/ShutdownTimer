using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Services
{
	public class ShutdownHistoryService : IShutdownHistoryService
	{
		private readonly ILogger<ShutdownHistoryService> _logger;
		private readonly ApplicationDbContext _dbContext;

		public ShutdownHistoryService(ILogger<ShutdownHistoryService> logger, ApplicationDbContext dbContext)
		{
			_logger = logger;
			_dbContext = dbContext;
		}

		public async Task LogShutdownAsync(ClaimsPrincipal identity, TimeSpan ts)
		{
			var id = identity.Claims.FirstOrDefault(d => d.Type == ClaimTypes.NameIdentifier);
			if (id == null)
			{
				_logger.LogError($"Failed to get identity id for user {identity}.");
				return;
			}

			var entry = _dbContext.ShutdownExecutionHotlinks.FirstOrDefault(d =>
				d.UserId == id.Value && d.Hours == ts.Hours && d.Minutes == ts.Minutes);

			if (entry == null)
			{
				_dbContext.ShutdownExecutionHotlinks.Add(new ShutdownHistoryItem()
				{
					Hours = ts.Hours,
					LastExecution = DateTime.UtcNow,
					Minutes = ts.Minutes,
					UserId = id.Value
				});
			}
			else
			{
				entry.LastExecution = DateTime.UtcNow;
				_dbContext.Entry(entry).State = EntityState.Modified;
			}

			var changes = await _dbContext.SaveChangesAsync();
			if(changes != 1)
				throw new Exception($"Failed to save history changes.");
		}

		public async Task<List<ShutdownHistoryItem>> GetHistoryForUserAsync(ClaimsPrincipal identity)
		{
			var id = identity.Claims.FirstOrDefault(d => d.Type == ClaimTypes.NameIdentifier);
			if (id == null)
			{
				_logger.LogError($"Failed to get identity id for user {identity}.");
				return new List<ShutdownHistoryItem>();
			}

			return await _dbContext.ShutdownExecutionHotlinks
				.Where(d => d.UserId == id.Value)
				.OrderByDescending(d => d.LastExecution)
				.ToListAsync();
		}
	}
}