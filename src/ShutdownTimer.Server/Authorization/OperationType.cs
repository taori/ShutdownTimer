namespace ShutdownTimer.Server.WindowsService.Authorization
{
	public enum OperationType
	{
		Shutdown,
		AbortShutdown,
		IsShutdownPending,
		Hibernate,
		Restart,
		Logout
	}
}