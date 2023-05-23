Param (
	[Parameter(Mandatory = $false)][string]$server = "",
	[Parameter(Mandatory = $false)][string]$database = "",
	[Parameter(Mandatory = $false)][string]$username = "",
	[Parameter(Mandatory = $false)][string]$password = "",
	[Parameter(Mandatory = $false)][string]$ConnectionString = "",
	[Parameter(Mandatory = $false)][string]$appsettings = "",
	[Parameter(Mandatory = $false)][switch]$UseIntegratedSecurity,
	[Parameter(Mandatory = $false)][switch]$CreateDatabase,
	[Parameter(Mandatory = $false)][switch]$RebuildDatabase,
	[Parameter(Mandatory = $false)][switch]$TestData
)

$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';
# common repository functions
Import-Module .\repository.psm1

# module to execute sql statements
try {
	# ErrorAction must be Stop in order to trigger catch
	Import-Module SqlServer -ErrorAction Stop
} catch {
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
	Install-PackageProvider -Name PowershellGet -Force -Scope CurrentUser
	Install-Module -Name SqlServer -AllowClobber -Force -Scope CurrentUser
	Import-Module SqlServer
}

Function Execute-Sql {
	Param (
        [Parameter(Mandatory = $true)] [string] $database,
        [Parameter(Mandatory = $true)] [string] $sql
    )
	
	$error.clear(); 
	$result = "";
	if ($ConnectionString -ne "") {
		$conn = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
		$conn.set_ConnectionString($ConnectionString)	
		$conn.Database = $database;	
		$result = invoke-sqlcmd -ConnectionString $conn.ConnectionString -Query $sql -OutputSqlErrors $true
	} elseif ($username -eq "") {
		$result = invoke-sqlcmd -Server "$server" -Database $database -Query $sql -OutputSqlErrors $true
	} else {
		$secpasswd = ConvertTo-SecureString $password -AsPlainText -Force
		$creds = New-Object System.Management.Automation.PSCredential ($username, $secpasswd)
		$result = invoke-sqlcmd -Credential $creds -Server $server -Database $database -Query $sql -OutputSqlErrors $true
	}
	
	if ($error -ne $null) {
		Write-Error "ERROR: $error"
		return $error
	}

	return $result
}

Function Get-Files($path) {
	$files = @()
	if (Test-Path -path $path) {
		gci -Recurse @($path) | % {
			$files += $_.FullName
		}
	}
	return $files
}

$config = Get-RepositoryConfiguration

if ((Test-Path 'env:MSSQL_SERVER') -and $server -eq "") {
	$server = $env:MSSQL_SERVER

	if ((Test-Path 'env:MSSQL_USER')) {
		$username = $env:MSSQL_USER
	}
	if ((Test-Path 'env:MSSQL_PASSWORD')) {
		$password = $env:MSSQL_PASSWORD
	}
}
if ($appsettings -ne "") {
	$connectionString = (get-content $appsettings | ConvertFrom-Json).Database.ConnectionString
}
if ($connectionString -ne "") {
	$conn = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
	$conn.set_ConnectionString($ConnectionString)	
	$database = $conn.Database;	
	$server = $conn.Server
}
if ($server -eq "") {
	$server = "(LocalDB)\MSSQLLocalDB"
}
if ($database -eq "" -and $connectionString -eq "") {
	$database = $config.database.name
}

echo "Server:   $server"
echo "Database: $database"
echo "User:     $username"

if ($RebuildDatabase.IsPresent) {
	Write-Output "Rebuilding database..."
	Execute-Sql -database "master" -sql "IF EXISTS(SELECT * FROM sys.databases WHERE name = '$database') BEGIN alter database [$database] set single_user with rollback immediate; DROP database [$database] END"
}

if ($CreateDatabase.IsPresent -OR $RebuildDatabase.IsPresent) {
	Write-Output "Creating database..."
	Execute-Sql -database "master" -sql "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '$database') BEGIN CREATE database [$database] END"
}

$scripts = @()
$scripts += Get-Files(".\src\sql\types\*.sql")
$scripts += Get-Files(".\src\sql\migration\*.sql")
$scripts += Get-Files(".\src\sql\table\*.sql")
$scripts += Get-Files(".\src\sql\view\*.sql")
$scripts += Get-Files(".\src\sql\function\*.sql")
$scripts += Get-Files(".\src\sql\proc\*.sql")
$scripts += Get-Files(".\src\sql\trigger\*.sql")
$scripts += Get-Files(".\src\sql\data\*.sql")
$scripts += Get-Files(".\src\sql\release\*.sql")
if ($TestData.IsPresent) {
	$scripts += Get-Files(".\src\sql\testdata\*.sql")
}

echo "update database..."
foreach ($script in $scripts) {
	echo "running $script"
	$log = $script -replace "(.*).sql(.*)", '$1.log$2'
	$result = Execute-Sql -database $database -sql (Get-Content -Raw "$script")
	$result | Out-File $log
}

echo "done"
