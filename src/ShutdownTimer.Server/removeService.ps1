$serviceName = "ShutdownTimer.Server"

Stop-Service -Name $serviceName
Remove-Service -Name $serviceName