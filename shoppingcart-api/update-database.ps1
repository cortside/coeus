Param
(
	[Parameter(Mandatory = $false)][string]$server = "localhost",
	[Parameter(Mandatory=$false)][string]$database = "ShoppingCart",
	[Parameter(Mandatory = $false)][string]$username = "",
	[Parameter(Mandatory = $false)][string]$password = "",
	[Parameter(Mandatory = $false)][string]$ConnectionString = "",
	[Parameter(Mandatory = $false)][switch]$UseIntegratedSecurity,
	[Parameter(Mandatory = $false)][switch]$CreateDatabase
)

$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';


try {
	# ErrorAction must be Stop in order to trigger catch
	Import-Module SqlServer -ErrorAction Stop
}
catch {
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
	Install-PackageProvider -Name NuGet -Force
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
	}
 else {
		Write-Output "using integrated security"
	}
}

if ($CreateDatabase.IsPresent) {
	Write-Output "Creating database..."
	if ($username -eq "") {
		invoke-sqlcmd -Server "$server" -Query "CREATE database $database"
	}
 else {
		$secpasswd = ConvertTo-SecureString $password -AsPlainText -Force
		$creds = New-Object System.Management.Automation.PSCredential ($username, $secpasswd)
		invoke-sqlcmd -Credential $creds -Server $server -Query "CREATE database $database"
	}
}

echo "update database..."


$scripts = @()
# gci -Recurse @(".\src\sql\migration\*.sql") | % {
	# $scripts += $_.FullName
# }
gci -Recurse @(".\src\sql\table\*.sql") | % {
	$scripts += $_.FullName
}
gci -Recurse @(".\src\sql\data\*.sql") | % {
	$scripts += $_.FullName
}
gci -Recurse @(".\src\sql\proc\*.sql") | % {
	$scripts += $_.FullName
}
gci -Recurse @(".\src\sql\trigger\*.sql") | % {
	$scripts += $_.FullName
}
# gci -Recurse @(".\src\sql\view\*.sql") | % {
	# $scripts += $_.FullName
# }
#gci -Recurse @(".\src\sql\release\*.sql") | % {
#	$scripts += $_.FullName
#}


echo "Server: $Server"
echo "Database: $database"
echo "User: $username"

foreach ($script in $scripts) {
	echo "running $script"
	$log = $script -replace "(.*).sql(.*)", '$1.log$2'
	
	if ($username -eq "") {
		invoke-sqlcmd -Server "$server" -Database $database -inputFile "$script" | Out-File -FilePath "$log"
	}
 else {
		$secpasswd = ConvertTo-SecureString $password -AsPlainText -Force
		$creds = New-Object System.Management.Automation.PSCredential ($username, $secpasswd)
		
		invoke-sqlcmd -Credential $creds -Server $server -Database $database -inputFile "$script" | Out-File -FilePath "$log"
	}
	cat "$log"
}

echo "done"
