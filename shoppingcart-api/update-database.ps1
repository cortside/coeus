Param
(
	[Parameter(Mandatory = $false)][string]$server = "",
	[Parameter(Mandatory=$false)][string]$database = "ShoppingCart",
	[Parameter(Mandatory = $false)][string]$username = "",
	[Parameter(Mandatory = $false)][string]$password = "",
	[Parameter(Mandatory = $false)][string]$ConnectionString = "",
	[Parameter(Mandatory = $false)][switch]$UseIntegratedSecurity,
	[Parameter(Mandatory = $false)][switch]$CreateDatabase,
	[Parameter(Mandatory = $false)][switch]$RebuildDatabase
)

if ((Test-Path 'env:MSSQL_SERVER') -and $server -eq "") {
	$server = $env:MSSQL_SERVER

	if ((Test-Path 'env:MSSQL_USER')) {
		$username = $env:MSSQL_USER
	}
	if ((Test-Path 'env:MSSQL_PASSWORD')) {
		$password = $env:MSSQL_PASSWORD
	}
}
if ($server -eq "") {
	$server = "(LocalDB)\MSSQLLocalDB"
}

echo "Server: $server"

$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';

try {
	# ErrorAction must be Stop in order to trigger catch
	Import-Module SqlServer -ErrorAction Stop
} catch {
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
	Install-PackageProvider -Name PowershellGet -Force
	Install-Module -Name SqlServer -AllowClobber -Force
	Import-Module SqlServer
}

if ($ConnectionString) {
	# use connection string details
	$conn = New-Object System.Data.SqlClient.SqlConnectionStringBuilder 
	Write-Verbose "connection string: $ConnectionString"
	$conn.set_ConnectionString($ConnectionString)
	$server = $conn.DataSource
	$database = $conn.InitialCatalog
	if (!($UseIntegratedSecurity.IsPresent)) {
		$conn.TryGetValue('User ID', [ref]$username)
		$conn.TryGetValue('Password', [ref]$password)
	} else {
		Write-Output "using integrated security"
	}
}

if ($RebuildDatabase.IsPresent) {
	Write-Output "Rebuilding database..."
	if ($username -eq "") {
		invoke-sqlcmd -Server "$server" -Query "IF EXISTS(SELECT * FROM sys.databases WHERE name = '$database') BEGIN alter database [$database] set single_user with rollback immediate; DROP database [$database] END"
	} else {
		$secpasswd = ConvertTo-SecureString $password -AsPlainText -Force
		$creds = New-Object System.Management.Automation.PSCredential ($username, $secpasswd)
		invoke-sqlcmd -Credential $creds -Server $server -Query "IF EXISTS(SELECT * FROM sys.databases WHERE name = '$database') BEGIN alter database [$database] set single_user with rollback immediate; DROP database [$database] END"
	}
}


if ($CreateDatabase.IsPresent -OR $RebuildDatabase.IsPresent) {
	Write-Output "Creating database..."
	if ($username -eq "") {
		invoke-sqlcmd -Server "$server" -Query "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '$database') BEGIN CREATE database [$database] END"
	} else {
		$secpasswd = ConvertTo-SecureString $password -AsPlainText -Force
		$creds = New-Object System.Management.Automation.PSCredential ($username, $secpasswd)
		invoke-sqlcmd -Credential $creds -Server $server -Query "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '$database') BEGIN CREATE database [$database] END"
	}
}

echo "update database..."


$scripts = @()
if (Test-Path -path ".\src\sql\types\*.sql") {
	gci -Recurse @(".\src\sql\types\*.sql") | % {
		$scripts += $_.FullName
	}
}
if (Test-Path -path ".\src\sql\migration\*.sql") {
	gci -Recurse @(".\src\sql\migration\*.sql") | % {
		$scripts += $_.FullName
	}
}
if (Test-Path -path ".\src\sql\table\*.sql") {
	gci -Recurse @(".\src\sql\table\*.sql") | % {
		$scripts += $_.FullName
	}
}
if (Test-Path -path ".\src\sql\function\*.sql") {
	gci -Recurse @(".\src\sql\function\*.sql") | % {
		$scripts += $_.FullName
	}
}
if (Test-Path -path ".\src\sql\proc\*.sql") {
	gci -Recurse @(".\src\sql\proc\*.sql") | % {
		$scripts += $_.FullName
	}
}
if (Test-Path -path ".\src\sql\trigger\*.sql") {
	gci -Recurse @(".\src\sql\trigger\*.sql") | % {
		$scripts += $_.FullName
	}
}
if (Test-Path -path ".\src\sql\view\*.sql") {
	gci -Recurse @(".\src\sql\view\*.sql") | % {
		$scripts += $_.FullName
	}
}
if (Test-Path -path ".\src\sql\data\*.sql") {
	gci -Recurse @(".\src\sql\data\*.sql") | % {
		$scripts += $_.FullName
	}
}
if (Test-Path -path ".\src\sql\release\*.sql") {
	gci -Recurse @(".\src\sql\release\*.sql") | % {
		$scripts += $_.FullName
	}
}

echo "Server: $Server"
echo "Database: $database"
echo "User: $username"

foreach ($script in $scripts) {
	echo "running $script"
	$log = $script -replace "(.*).sql(.*)", '$1.log$2'
	
	if ($username -eq "") {
		invoke-sqlcmd -Server "$server" -Database $database -inputFile "$script" | Out-File -FilePath "$log"
	} else {
		$secpasswd = ConvertTo-SecureString $password -AsPlainText -Force
		$creds = New-Object System.Management.Automation.PSCredential ($username, $secpasswd)
		
		invoke-sqlcmd -Credential $creds -Server $server -Database $database -inputFile "$script" | Out-File -FilePath "$log"
	}
	cat "$log"
}

echo "done"
