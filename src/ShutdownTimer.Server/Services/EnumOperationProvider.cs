using System.Collections.Generic;
using System.Linq;
using ShutdownTimer.Server.Abstraction;
using ShutdownTimer.Server.Authorization;

namespace ShutdownTimer.Server.Services
{
	public class EnumOperationProvider : IOperationProvider
	{
		public IEnumerable<string> GetOperations()
		{
			return typeof(OperationType)
				.GetEnumValues()
				.Cast<OperationType>()
				.Select(d => d.ToString());
		}
	}
}