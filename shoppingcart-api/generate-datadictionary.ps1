Param (
	[Parameter(Mandatory = $false)][string]$server = "",
	[Parameter(Mandatory = $false)][string]$database = "master",
	[Parameter(Mandatory = $false)][string]$username = "",
	[Parameter(Mandatory = $false)][string]$password = "",
	[Parameter(Mandatory = $false)][string]$ConnectionString = "",
	[Parameter(Mandatory = $false)][switch]$UseIntegratedSecurity
)

Function Execute-Sql {
	Param (
        [Parameter(Mandatory = $true)] [string] $database,
        [Parameter(Mandatory = $true)] [string] $sql
    )
	
	$error.clear(); 
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

echo "Server:   $server"
echo "Database: $database"
echo "User:     $username"

$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';

try {
	# ErrorAction must be Stop in order to trigger catch
	Import-Module SqlServer -ErrorAction Stop
} catch {
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
	Install-PackageProvider -Name PowershellGet -Force -Scope CurrentUser
	Install-Module -Name SqlServer -AllowClobber -Force -Scope CurrentUser
	Import-Module SqlServer
}

	# Write-Output "Rebuilding database..."
	# Execute-Sql -database "master" -sql "IF EXISTS(SELECT * FROM sys.databases WHERE name = '$database') BEGIN alter database [$database] set single_user with rollback immediate; DROP database [$database] END"

$sql = @"
SELECT name, database_id, create_date 
FROM sys.databases
WHERE name not in ('master', 'model', 'msdb', 'tempdb')
ORDER BY name
"@

echo "generating datadictionary"
$databases = Execute-Sql -database $database -sql $sql

$databases | measure-object

$filename = "$PSScriptRoot\DataDictionary.csv"
Write-Output "exporting $filename"
"Database,Schema,Table Name,Table Description,Column Name,Position,Column Description,Data Type, Nullable" | Out-File $filename -Encoding utf8

foreach ($db in $databases) {
	echo $db.name
	
	$sql = @"
SELECT col.TABLE_CATALOG, col.TABLE_SCHEMA, col.TABLE_NAME, tprop.value TABLE_DESCRIPTION, COLUMN_NAME, ORDINAL_POSITION, prop.value AS [COLUMN_DESCRIPTION],
	case col.DATA_TYPE	
		when 'decimal' then cast (col.DATA_TYPE as varchar) + ' ('+cast (NUMERIC_PRECISION as varchar) + ' , '+cast (NUMERIC_SCALE as varchar) +')'
		when 'varchar' then cast (col.DATA_TYPE as varchar) + ' ('+cast (CHARACTER_OCTET_LENGTH as varchar) + ')'
		when 'nvarchar' then cast (col.DATA_TYPE as varchar)+' ('+case when CHARACTER_OCTET_LENGTH<0 then 'MAX' else cast (CHARACTER_OCTET_LENGTH as varchar) end + ')'
		else col.DATA_TYPE
	end DATA_TYPE, col.IS_NULLABLE
FROM INFORMATION_SCHEMA.TABLES AS tbl
INNER JOIN INFORMATION_SCHEMA.COLUMNS AS col ON col.TABLE_NAME = tbl.TABLE_NAME
INNER JOIN sys.columns AS sc ON sc.object_id = object_id(tbl.table_schema + '.' + tbl.table_name) AND sc.NAME = col.COLUMN_NAME
LEFT JOIN sys.extended_properties prop ON prop.major_id = sc.object_id AND prop.minor_id = sc.column_id AND prop.NAME = 'MS_Description'
LEFT JOIN sys.extended_properties tprop ON tprop.major_id = sc.object_id AND tprop.minor_id=0 AND tprop.NAME = 'MS_Description'
WHERE tbl.TABLE_TYPE = 'BASE TABLE'
	and tbl.TABLE_NAME not in ('__EFMigrationsHistory') -- EF managed tables
ORDER BY col.TABLE_CATALOG, col.TABLE_SCHEMA, col.TABLE_NAME, col.ORDINAL_POSITION
"@

	$rows = Execute-Sql -database $db.name -sql $sql
	foreach ($row in $rows) {
		$output = @"
"$($row.TABLE_CATALOG)","$($row.TABLE_SCHEMA)","$($row.TABLE_NAME)","$($row.TABLE_DESCRIPTION)","$($row.COLUMN_NAME)","$($row.ORDINAL_POSITION)","$($row.COLUMN_DESCRIPTION)","$($row.DATA_TYPE)","$($row.IS_NULLABLE)"
"@		
		$output | Out-File $filename -Encoding utf8 -Append
	}
}

echo "done"
