# PowerShell script to add program to the Windows startup registry
$programName = "CTI_Notifier"  # Name of the application in the registry
$programPath = "C:\Path\To\YourApp.exe"  # Path to your application's executable

# Adds a registry key under HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run
$regKeyPath = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run"
Set-ItemProperty -Path $regKeyPath -Name $programName -Value $programPath

Write-Host "Application successfully added to startup."
