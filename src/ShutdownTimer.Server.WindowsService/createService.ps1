$serviceName = "ShutdownTimer.Server"
$exeDirectory = $PSScriptRoot
$exePath = "$PSScriptRoot\ShutdownTimer.Server.WindowsService.exe"
$user = Get-Credential -Credential MSS-Laptop\SelfMadeServers
$acl = Get-Acl $exeDirectory
$aclRuleArgs = $user, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl $exeDirectory


New-Service -Name $serviceName -BinaryPathName $exePath -Credential $user -Description "Server used for ShutdownTimer" -DisplayName "ShutdownTimer Server" -StartupType Automatic
Start-Service -Name $serviceName