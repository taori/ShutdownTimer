using System.Collections.Generic;

namespace ShutdownTimer.Server.Abstraction
{
	public interface IOperationProvider
	{
		IEnumerable<string> GetOperations();
	}
}