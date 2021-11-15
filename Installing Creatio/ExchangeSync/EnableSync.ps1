#Execute this in PowerShell
#.\EnableSync.ps1 -Domain http://k_krylov -AppPort 9060 -ContainerPort 10000

param($Domain, $AppPort, $ContainerPort, $ClioEnvName)

$updateClioCommand = "dotnet tool update clio -g";
Invoke-Expression $updateClioCommand;

#We need clio-gate installed to enable features automatically
$CheckgateCommand = "C:\clio\clio\bin\Debug\netcoreapp3.1\clio.exe packages -e ${ClioEnvName} | Select-String -Pattern cliogate";
Write-Host "Checking if clio gate is installed";
$result = Invoke-Expression $CheckgateCommand | Out-String;
if($result){
    Write-Host "OK - clio-gate insatlled" -ForegroundColor Green
}else{
    Write-Host "Installing clio-gate"
    $installGateCommand = "C:\clio\clio\bin\Debug\netcoreapp3.1\clio.exe  install-gate -e ${ClioEnvName}";
    Invoke-Expression $installGateCommand;
}

#Enable Features for older versions of creatio
# Write-Host "Enabeling Features" -ForegroundColor Green
# $EnableFeatureCommand = "C:\clio\clio\bin\Debug\netcoreapp3.1\clio.exe set-feature EmailIntegrationV2 1";
# Invoke-Expression $EnableFeatureCommand

# $EnableFeatureCommand = "C:\clio\clio\bin\Debug\netcoreapp3.1\clio.exe set-feature ExchangeListenerEnabled 1";
# Invoke-Expression $EnableFeatureCommand

# Set SysSettings  
# This is where docker lives
# http://k_krylov:10000/api/listeners
Write-Host "Configuring System Settings" -ForegroundColor Green
$SetSysSettingCommand = "C:\clio\clio\bin\Debug\netcoreapp3.1\clio.exe set-syssetting ExchangeListenerServiceUri '${Domain}:${ContainerPort}/api/listeners' Text -e ${ClioEnvName}";
Invoke-Expression $SetSysSettingCommand;

# EndPoint for docker to communicate back upon new event (email)
# http://k_krylov:9050/0/ServiceModel/ExchangeListenerService.svc/NewEmail
$SetSysSettingCommand = "C:\clio\clio\bin\Debug\netcoreapp3.1\clio.exe set-syssetting BpmonlineExchangeEventsEndpointUrl '${Domain}:${AppPort}/0/ServiceModel/ExchangeListenerService.svc/NewEmail' Text -e ${ClioEnvName}";
Invoke-Expression $SetSysSettingCommand;

#Start Docker Container in detached mode
# Write-Host "Staring Docker container" -ForegroundColor Green;
# $StartDockerCommand = "docker-compose up -d";
# Invoke-Expression $StartDockerCommand;