$serviceName = "ShutdownTimer.Server"
$exeDirectory = $PSScriptRoot
$exePath = "$PSScriptRoot\ShutdownTimer.Server.exe"
$user = Get-Credential "MSS-Laptop\SelfMadeServers"
$acl = Get-Acl $exeDirectory
$aclRuleArgs = $user.UserName, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl $exeDirectory


New-Service -Name $serviceName -BinaryPathName $exePath -Credential $user.UserName -Description "Server used for ShutdownTimer" -DisplayName "ShutdownTimer Server" -StartupType Automatic
Start-Service -Name $serviceName