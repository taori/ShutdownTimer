using System.Collections.Generic;
using System.Threading.Tasks;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Abstraction
{
	public interface ICustomCommandService
	{
		Task<DescriptiveResult> AddAsync(string commandName, string program, string arguments, string permissionName);

		Task<CustomCommand> GetAsync(int id);

		Task<List<CustomCommand>> GetAllAsync();

		Task<DescriptiveResult> DeleteAsync(int id);
	}
}