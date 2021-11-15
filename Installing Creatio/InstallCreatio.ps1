
#.\Install.ps1 
	# -AppName bndl_demo 
	# -FolderName '7.17.4.2252_SalesEnterprise_Marketing_ServiceEnterprise_Demo_PostgreSQL_ENU' 
	# -dbUser Supervisor 
	# -dbPassword Supervisor 
	# -port 9070 
	# -redisDbNum 2
	# -redisPort 6380
	# -dbFileName "BPMonline7174SalesEnterprise_Marketing_ServiceEnterprise.backup"
	# -dbPort 5432

#Install Creatio
param($AppName, $FolderName, $dbUser, $dbPassword, $port, $redisDbNum, $redisPort, $dbFileName, $dbPort)
$myHost = Invoke-Expression 'hostname';

#Step 1 - Create Directory for new Installation
$AppPath = "C:\inetpub\wwwroot\${AppName}";
New-Item -ItemType directory -Path $AppPath -Force;
Write-Host "`nNEW $AppPath DIRECTORY CREATED " -ForegroundColor Green;

#Step2 - Copy folder to $AppPath Folder
Write-Host "Copying necessary files to $AppPath" -ForegroundColor Green
Copy-Item "C:\Build\${FolderName}\*" -Destination $AppPath -Recurse;

#Restoring PostgreSQL DataBase in Docker
Write-Host "Started Restoring DataBase"

# STEP 1 - CREATE DATABASE
$dbName = [string]$AppName.ToLower();
$psqlCmd = "docker exec -it PostgresDb psql -U Supervisor postgres -c `"CREATE DATABASE ${dbName} ENCODING='UTF8' CONNECTION LIMIT=-1 `"";
Invoke-Expression $psqlCmd;

# STEP 2 - RESTORE DATABASE
$psqlCmd = "docker exec -it PostgresDb pg_restore -U Supervisor --no-owner --no-privilege -d ${dbName} /usr/local/db/${dbFileName}";
Invoke-Expression $psqlCmd;

#Delete db folder
Remove-Item $AppPath\db\*
Remove-Item $AppPath\db\
Write-Host "\nDeleted db folder" - -ForegroundColor Green;


#Edit ConnectionString.config file
# Replace Redis

$file = "$AppPath\ConnectionStrings.config";
[xml]$doc  =  Get-Content $file;
[System.Xml.XmlNode]$redis = $doc.SelectSingleNode("/connectionStrings/add[@name='redis']");
[System.Xml.XmlAttribute]$redisAttr = $redis.Attributes.GetNamedItem("connectionString","");
$redisAttr.Value = "host=localhost; db=${redisDbNum}; port=6379";

[System.Xml.XmlNode]$db = $doc.SelectSingleNode("/connectionStrings/add[@name='db']");
[System.Xml.XmlAttribute]$dbAttr = $db.Attributes.GetNamedItem("connectionString","");
$dbAttr.Value = "Server=$myHost;Port=5432;Database=$AppName;User ID=${dbUser};password=${dbPassword};Timeout=500; CommandTimeout=400;MaxPoolSize=1024;"
$doc.Save($file);

#IIS
#Import-Module WebAdministration
# Create new webPool
New-WebAppPool -Name $AppName -Force;

# Create IIS Site
New-WebSite -Name $AppName -ApplicationPool $AppName -Port $port -HostHeader $myHost -PhysicalPath $AppPath -Force

# Add App
New-WebApplication -Name 0 -Site $Appname -ApplicationPool $AppName -PhysicalPath $AppPath"\Terrasoft.WebApp" -Force
Write-Host "IIS Site Created"
Write-Host "`nCREATED NEW SITE AND APP_POOL:"-ForegroundColor Green

#Update clio to the latest version
Invoke-Expression 'dotnet tool update clio -g'

#register with clio
$clioCmd = "clio reg-web-app -u http://${myHost}:${port} -p Supervisor -l Supervisor -m Customer -c true -e ${AppName}" ;
Invoke-Expression $clioCmd;

#Set Active Environment
#$clioCmd = "clio reg-web-app -a ${AppName}";
#Invoke-Expression $clioCmd;

$clioCmd = "clio install-gate -e ${AppName}";
Invoke-Expression $clioCmd;

#Do NOT Show widget on intro page
$clioCmd = "clio set-syssetting ShowWidgetOnIntroPage false Boolean -e ${AppName}";
Invoke-Expression $clioCmd;

#Do NOT Show widget on login page
$clioCmd = "clio set-syssetting ShowWidgetOnLoginPage false Boolean -e ${AppName}";
Invoke-Expression $clioCmd;

#set Default TimeZone to "Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius (GMT+02:00)"
$clioCmd = "clio set-syssetting DefaultTimeZone 97c71d34-55d8-df11-9b2a-001d60e938c6 Lookup -e ${AppName}";
Invoke-Expression $clioCmd;

#set Session Timeout, Max is 720 minutes (12 hours)"
$clioCmd = "clio set-syssetting UserSessionTimeout 720 Integer -e ${AppName}";
Invoke-Expression $clioCmd;

#Disable updating contact ages
$clioCmd = "clio set-syssetting ActualizeAge false Boolean -e ${AppName}";
Invoke-Expression $clioCmd;
$clioCmd = "clio set-syssetting RunAgeActualizationDaily false Boolean -e ${AppName}";
Invoke-Expression $clioCmd;

#Enable Debug mode
$clioCmd = "clio set-syssetting IsDebug true Boolean -e ${AppName}";
Invoke-Expression $clioCmd;

$clioCmd = "clio set-syssetting SchemaNamePrefix ' ' Text -e ${AppName}";
Invoke-Expression $clioCmd;

#Configure Exchange Listener
#https://academy.creatio.com/docs/developer/platform_overview/components_and_services/exchange_listener_synchronization_service
$clioCmd = "clio set-syssetting ExchangeListenerServiceUri 'http://${myHost}:10000/api/listeners' Text -e ${AppName}";
Invoke-Expression $clioCmd;

$clioCmd = "clio set-syssetting BpmonlineExchangeEventsEndpointUrl 'http://${myHost}:${port}/0/ServiceModel/ExchangeListenerService.svc/NewEmail' Text -e ${AppName}";
Invoke-Expression $clioCmd;

#Open Application
Start-Process "http://${myHost}:${port}"
Write-Host "`n!!! ENJOY YOUR CREATIO !!!" -ForegroundColor Green
