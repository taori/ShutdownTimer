using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using ShutdownTimer.Server.Data;

namespace ShutdownTimer.Server.Abstraction
{
	public interface IShutdownHistoryService
	{
		Task LogShutdownAsync(ClaimsPrincipal identity, TimeSpan ts);
		Task<List<ShutdownHistoryItem>> GetHistoryForUserAsync(ClaimsPrincipal httpContextUser);
	}
}