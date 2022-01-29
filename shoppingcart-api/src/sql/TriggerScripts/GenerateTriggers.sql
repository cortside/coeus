sp_configure 'show advanced options', 1;
GO
RECONFIGURE;
GO
sp_configure 'Ole Automation Procedures', 1;
GO
RECONFIGURE;
GO

DECLARE @db VARCHAR(100);
SET @db = db_name();

DECLARE @updateTriggerCommand VARCHAR(MAX);
SET @updateTriggerCommand = 
'DROP TRIGGER IF EXISTS {{triggerName}}
GO

---
-- Trigger for {{table}} that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER {{triggerName}}
	ON ?
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
    SET @action = ''INSERT'';
    IF EXISTS(SELECT 1 FROM DELETED)
    BEGIN
        SET @action = 
            CASE
                WHEN EXISTS(SELECT 1 FROM INSERTED) THEN ''UPDATE''
                ELSE ''DELETE''
            END
    END

	-- determine username
	DECLARE @UserName nvarchar(200);
	{{lastModifiedUser}}

	-- insert parent transaction
	INSERT INTO audit.AuditLogTransaction (TableName, TableSchema, Action, HostName, ApplicationName, AuditLogin, AuditDate, AffectedRows, DatabaseName, UserId, TransactionId)
	values(''{{table}}'', ''{{schema}}'', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN '' '' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN '' '' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

	{{columns}}

	--IF @Inserted = 0
	--	BEGIN
	--	    -- believed to be contributing to deadlocks
	--		-- DELETE FROM audit.AuditLogTransaction WHERE AuditLogTransactionId = @AuditLogTransactionId
	--	END
END
GO
';

DECLARE @NAME VARCHAR(100)

DECLARE CUR CURSOR FOR
  SELECT QUOTENAME(TABLE_SCHEMA)+'.'+QUOTENAME(TABLE_NAME)
  FROM   INFORMATION_SCHEMA.TABLES
  WHERE  TABLE_NAME NOT IN ('__EFMigrationsHistory', 'Audit', 'AuditLog', 'AuditLogs', 'AuditLogTransaction', 'sysdiagrams', 'DiagnosticLog')

OPEN CUR

FETCH NEXT FROM CUR INTO @NAME

WHILE @@FETCH_STATUS = 0
  BEGIN
      --PRINT @NAME
	  declare @cmd NVARCHAR(MAX) = @updateTriggerCommand
	  DECLARE @TriggerName nvarchar(2000);
	  SET @TriggerName = 'tr' + PARSENAME(@NAME, 1);

	declare @lastModifiedUserColumn varchar(100)
	set @lastModifiedUserColumn = coalesce((SELECT top 1 COLUMN_NAME
	  FROM   INFORMATION_SCHEMA.COLUMNS
	  WHERE  TABLE_SCHEMA = PARSENAME(@NAME, 2) 
		AND TABLE_NAME = PARSENAME(@NAME, 1)
		AND COLUMN_NAME in ('LastModifiedUserId', 'ModifiedBy', 'LastModifiedSubjectId')),'')

	print '-----'
	print @lastModifiedUserColumn
	if (@lastModifiedUserColumn!='')
	  BEGIN
		set @lastModifiedUserColumn= 'SELECT TOP 1 @UserName=['+@lastModifiedUserColumn+'] FROM inserted;'
	  END
	ELSE
	  BEGIN
		set @lastModifiedUserColumn= 'SET @UserName = CURRENT_USER'
	  END

	  print @lastModifiedUserColumn
	  set @cmd = REPLACE(@cmd, '{{triggerName}}', @TriggerName)
	  set @cmd = REPLACE(@cmd, '{{lastModifiedUser}}', @lastModifiedUserColumn)
	  set @cmd = REPLACE(@cmd, '?', @NAME)
	  set @cmd = REPLACE(@cmd, '{{schema}}', PARSENAME(@NAME, 2))
	  set @cmd = REPLACE(@cmd, '{{table}}', PARSENAME(@NAME, 1))

declare @columnTemplate nvarchar(MAX) = '	-- {{column}}
	IF UPDATE({{column}}) OR @action in (''INSERT'', ''DELETE'')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull(''[{{pk}}]=''+CONVERT(nvarchar(4000), IsNull(OLD.{{pk}}, NEW.{{pk}}), 0), ''[{{pk}}] Is Null'')),
				''{{column}}'',
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

'
DECLARE @COLUMNS NVARCHAR(MAX) = ''

declare @pk varchar(256)
SELECT @pk = ccu.COLUMN_NAME
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON tc.CONSTRAINT_NAME = ccu.Constraint_name
WHERE tc.CONSTRAINT_TYPE = 'Primary Key' and tc.TABLE_SCHEMA = PARSENAME(@NAME, 2) and tc.TABLE_NAME = PARSENAME(@NAME, 1)

DECLARE @COLUMN VARCHAR(100)
DECLARE CCUR CURSOR FOR
  SELECT COLUMN_NAME
  FROM   INFORMATION_SCHEMA.COLUMNS
  WHERE  TABLE_SCHEMA = PARSENAME(@NAME, 2) and TABLE_NAME = PARSENAME(@NAME, 1)
	and DATA_TYPE not in ('text', 'ntext', 'image', 'xml', 'varbinary')

OPEN cCUR
FETCH NEXT FROM CCUR INTO @COLUMN

WHILE @@FETCH_STATUS = 0
  BEGIN
      --PRINT @COLUMN

	  declare @c nvarchar(MAX) = @columnTemplate
	  set @c = REPLACE(@c, '{{column}}', quotename(@COLUMN))
	  set @c = REPLACE(@c, '{{pk}}', quotename(@pk))
	  set @columns = @columns + @c

	  --print @c

      FETCH NEXT FROM CCUR INTO @COLUMN
  END

CLOSE CCUR
DEALLOCATE CCUR 

	  set @cmd = REPLACE(@cmd, '{{columns}}', @COLUMNs)

	  print @name
	  print datalength(@cmd)
	  --print @cmd
 	  --execute(@cmd)
	  --exec sp_executesql @cmd

	  declare @filename varchar(200)
	  set @filename = 'tr' + PARSENAME(@NAME, 1)+'.trigger.sql'
	  declare @path varchar(200)
    set @path = 'C:/work/cortside.webapistarter/src/sql/trigger/'
    -- check if sqlcmd/invoke-sqlcmd has set a variable for the path
    declare @sqlcmdPath varchar(200);
    set @sqlcmdPath = N'$(sqlcmdPath)'
    if (@sqlcmdPath not like '%(sqlcmdPath)%')
    begin 
    	set @path = @sqlcmdPath
    end

	  print @filename
	  exec spWriteStringToFile @cmd, @path, @filename

      FETCH NEXT FROM CUR INTO @NAME
  END

CLOSE CUR
DEALLOCATE CUR 
GO

