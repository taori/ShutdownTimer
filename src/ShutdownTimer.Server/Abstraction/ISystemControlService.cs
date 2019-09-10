using System;
using System.Threading.Tasks;

namespace ShutdownTimer.Server.Abstraction
{
	public interface ISystemControlService
	{
		bool AbortShutdown();
		bool? IsShutdownPending();
		bool Hibernate();
		bool Restart();
		bool Logout();
		bool ShutDown(TimeSpan delay);
		Task<bool> ExecuteCustomCommandAsync(int id);
	}
}