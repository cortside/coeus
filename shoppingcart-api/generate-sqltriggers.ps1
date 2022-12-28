<#
.SYNOPSIS
    Generate sql scripts for triggers using a pristine database
    with all migrations applied
#>
[cmdletBinding()]
Param()

$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';

# this script is not safe to run on any machine other than a local dev machine, because of db deletion
$server = "(LocalDB)\MSSQLLocalDB"
$username=""
$password=""
if ((Test-Path 'env:MSSQL_SERVER') -and $server -eq "(LocalDB)\MSSQLLocalDB") {
	$server = $env:MSSQL_SERVER

	if ((Test-Path 'env:MSSQL_USER')) {
		$username = $env:MSSQL_USER
	}
	if ((Test-Path 'env:MSSQL_PASSWORD')) {
		$password = $env:MSSQL_PASSWORD
	}
}
echo "Server: $server"

$triggergenDbName = "GenerateSqlTriggers"

Write-Output @("
##########
# 1. Refresh database named $triggergenDbName so that all migrations are applied for pristine state
# 2. Generate new trigger sql scripts
##########
")

if ($PSScriptRoot.Contains(' ')) { 
    throw "Your working directory has a space in the path, which is not supported.  Wise up and move to C:\work\Acme.ShoppingCart! And have a wonderful work day!"
    exit
}

try {
    Import-Module SqlServer -ErrorAction Stop
}  catch {
    Write-Output "Installing SqlServer module for powershell"
    Install-PackageProvider -Name NuGet -Force
    Install-Module -Name SqlServer -AllowClobber -Force
    Import-Module SqlServer
}

try {   
	if ($username -eq "") {
		Write-Output "Verifying SqlServer accessible at $server"
		invoke-sqlcmd -ServerInstance $server -Query "select 'invoke-sqlcmd successful' AS SqlServerStatus" -QueryTimeout 5 -ConnectionTimeout 5 -ErrorAction Stop
	} else {
		Write-Output "Verifying SqlServer accessible at $server with user $username"
		invoke-sqlcmd -ServerInstance $server -username $username -password $password -Query "select 'invoke-sqlcmd successful' AS SqlServerStatus" -QueryTimeout 5 -ConnectionTimeout 5 -ErrorAction Stop
	}
} catch {
    throw "Problem connecting to SqlServer at $server. Please confirm up and running and try again."
}

$deleteLocalDbQuery = @"
use [master]
go
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'$triggergenDbName')
Begin
alter database [$triggergenDbName] set single_user with rollback immediate;
DROP DATABASE [$triggergenDbName];
End
"@

Write-Host "deleting $triggergenDbName on $server, if it exists"
if ($username -eq "") {
	invoke-sqlcmd -ServerInstance $server -Query $deleteLocalDbQuery -ErrorAction Stop
} else {
	invoke-sqlcmd -ServerInstance $server -username $username -password $password -Query $deleteLocalDbQuery -ErrorAction Stop
}

Write-Host "creating $triggergenDbName and applying all migrations"
& "$PSScriptRoot/update-database.ps1" -CreateDatabase -database $triggergenDbName

$triggerTemplate = @"
DROP TRIGGER IF EXISTS {{triggerName}}
GO

---
-- Trigger for {{table}} that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER {{triggerName}}
	ON {{qualifiedName}}
	FOR UPDATE, DELETE
	AS
		BEGIN
	SET NOCOUNT ON

	DECLARE 
		@AuditLogTransactionId	int,
		@Inserted	    		int = 0,
 		@ROWS_COUNT				int

	SELECT @ROWS_COUNT=count(*) from inserted

    -- Check if this is an INSERT, UPDATE or DELETE Action.
    DECLARE @action as varchar(10);
    SET @action = 'INSERT';
    IF EXISTS(SELECT 1 FROM DELETED)
    BEGIN
        SET @action = 
            CASE
                WHEN EXISTS(SELECT 1 FROM INSERTED) THEN 'UPDATE'
                ELSE 'DELETE'
            END
    END

	-- determine username
	DECLARE @UserName nvarchar(200);
	{{lastModifiedUser}}

	-- insert parent transaction
	INSERT INTO audit.AuditLogTransaction (TableName, TableSchema, Action, HostName, ApplicationName, AuditLogin, AuditDate, AffectedRows, DatabaseName, UserId, TransactionId)
	values('{{table}}', '{{schema}}', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()
{{columns}}
END
GO
"@

$columnTemplate = @"
	-- {{column}}
	IF UPDATE({{column}}) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[{{pk}}]='+CONVERT(nvarchar(4000), IsNull(OLD.{{pk}}, NEW.{{pk}}), 0), '[{{pk}}] Is Null')),
				'{{column}}',
				CONVERT(nvarchar(4000), OLD.{{column}}, 126),
				CONVERT(nvarchar(4000), NEW.{{column}}, 126),
				convert(nvarchar(4000), COALESCE(OLD.{{pk}}, NEW.{{pk}}, null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.{{pk}} = OLD.{{pk}} or (NEW.{{pk}} Is Null and OLD.{{pk}} Is Null))
			WHERE ((NEW.{{column}} <> OLD.{{column}}) 
					Or (NEW.{{column}} Is Null And OLD.{{column}} Is Not Null)
					Or (NEW.{{column}} Is Not Null And OLD.{{column}} Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

"@

Write-Output "generate trigger scripts"
$sql = @"
SELECT QUOTENAME(t.TABLE_SCHEMA)+'.'+QUOTENAME(t.TABLE_NAME) QualifiedName, t.TABLE_SCHEMA TableSchema, t.TABLE_NAME TableName, coalesce(c.COLUMN_NAME, '') LastModifiedUserColumn, pk.COLUMN_NAME PrimaryKeyColumn
FROM INFORMATION_SCHEMA.TABLES t
left join (
	SELECT TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, ROW_NUMBER() OVER(PARTITION BY TABLE_SCHEMA, TABLE_NAME ORDER BY COLUMN_NAME) AS RowNumber
	FROM INFORMATION_SCHEMA.COLUMNS
	where COLUMN_NAME in ('LastModifiedUserId', 'ModifiedBy', 'LastModifiedSubjectId')
) c on c.TABLE_NAME=t.TABLE_NAME and c.TABLE_SCHEMA=t.TABLE_SCHEMA and c.RowNumber=1
left join (
	SELECT tc.TABLE_SCHEMA, tc.TABLE_NAME, ccu.COLUMN_NAME
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
	JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON tc.CONSTRAINT_NAME = ccu.Constraint_name
	WHERE tc.CONSTRAINT_TYPE = 'Primary Key' 
) pk on pk.TABLE_NAME=t.TABLE_NAME and pk.TABLE_SCHEMA=t.TABLE_SCHEMA
WHERE t.TABLE_NAME NOT IN ('__EFMigrationsHistory', 'Audit', 'AuditLog', 'AuditLogs', 'AuditLogTransaction', 'sysdiagrams', 'DiagnosticLog')
"@

if ($username -eq "") {
	$tables = Invoke-Sqlcmd -Query $sql -ServerInstance $server -Database $triggergenDbName
} else {
	$tables = Invoke-Sqlcmd -Query $sql -ServerInstance $server -username $username -password $password -Database $triggergenDbName
}

foreach ($table in $tables) {
	echo "$($table.QualifiedName) $($table.TableName)"
	$triggerName = "tr$($table.TableName)"

	if ($table.LastModifiedUserColumn -ne "") {
		$lastmodifiedusercolumn = "SELECT TOP 1 @UserName=[$($table.LastModifiedUserColumn)] FROM inserted;"		
	} else {
		$lastmodifiedusercolumn = "set @username = current_user"
	}

	$sql = @"
SELECT COLUMN_NAME ColumnName
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = '$($table.TableSchema)' and TABLE_NAME = '$($table.TableName)'
	and DATA_TYPE not in ('text', 'ntext', 'image', 'xml', 'varbinary')
ORDER BY ORDINAL_POSITION
"@

	if ($username -eq "") {
		$columns = Invoke-Sqlcmd -Query $sql -ServerInstance $server -Database $triggergenDbName
	} else {
		$columns = Invoke-Sqlcmd -Query $sql -ServerInstance $server -username $username -password $password -Database $triggergenDbName
	}

	$cols = ""
	foreach ($column in $columns) {
		$col = "$columnTemplate"
		$col = $col -replace "{{column}}", "[$($column.ColumnName)]"
		$col = $col -replace "{{pk}}", "[$($table.PrimaryKeyColumn)]"
		
		$cols = "$cols`r`n$col"
	}

	$content = "$triggerTemplate"
	$content = $content -replace "{{triggerName}}", $triggerName
	$content = $content -replace "{{lastModifiedUser}}", $lastModifiedUserColumn
	$content = $content -replace "{{qualifiedName}}", $table.QualifiedName
	$content = $content -replace "{{schema}}", $table.TableSchema
	$content = $content -replace "{{table}}", $table.TableName
	$content = $content -replace "{{columns}}", $cols

	$filename = "src/sql/trigger/tr$($table.TableName).trigger.sql"
	$content | Out-File -encoding UTF8 $filename
}

git status

Write-Output @("
##########
# Complete. Updated files for commit listed above. 
#
# Please examine diff to ensure changes are expected.
##########
")
