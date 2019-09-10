using System.Collections.Generic;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Abstraction
{
	public interface ICommandProvider
	{
		IEnumerable<CustomCommand> GetCommands();
	}
}