namespace ShutdownTimer.Server.Authorization
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