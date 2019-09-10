using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Services
{
	public class CustomCommandService : ICustomCommandService, IOperationProvider
	{
		private readonly ApplicationDbContext _dbContext;

		public CustomCommandService(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<DescriptiveResult> AddAsync(string commandName, string program, string arguments, string permissionName)
		{
			permissionName = permissionName.ToLowerInvariant();

			if (_dbContext.CustomCommands.FirstOrDefault(d => d.PermissionName == permissionName) != null)
			{
				return DescriptiveResult.Fail(new DescriptiveError("0",
					$"a similar permission name [{permissionName}] already exists"));
			}

			_dbContext.CustomCommands.Add(new CustomCommand()
			{
				Program = program,
				Argument = arguments,
				PermissionName = permissionName,
				CommandName = commandName
			});

			return await _dbContext.SaveChangesAsync() > 0
				? DescriptiveResult.Success
				: DescriptiveResult.Fail().With(string.Empty, "Save process failed.");
		}

		public async Task<CustomCommand> GetAsync(int id)
		{
			return await _dbContext.CustomCommands.FirstOrDefaultAsync(d => d.Id == id).ConfigureAwait(false);
		}

		public async Task<List<CustomCommand>> GetAllAsync()
		{
			return await _dbContext.CustomCommands.ToListAsync().ConfigureAwait(false);
		}

		public async Task<DescriptiveResult> DeleteAsync(int id)
		{
			var item = await _dbContext.CustomCommands.FirstOrDefaultAsync(d => d.Id == id);
			_dbContext.Entry(item).State = EntityState.Deleted;

			return await _dbContext.SaveChangesAsync() > 0
				? DescriptiveResult.Success
				: DescriptiveResult.Fail().With(string.Empty, "Failed to delete command");
		}

		public IEnumerable<string> GetOperations()
		{
			return _dbContext.CustomCommands.Select(d => d.PermissionName);
		}
	}
}