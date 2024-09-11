drop procedure if exists #spAlterColumn_AuditLog
GO

IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'audit' ) 
  BEGIN
	EXEC sp_executesql N'CREATE SCHEMA audit'
  END
GO

--drop table if exists audit.AuditLog
--drop procedure if exists #spAlterColumn_AuditLog

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[audit].[AuditLog]') AND type in (N'U'))
  BEGIN
	CREATE TABLE [audit].[AuditLog](
		[AuditLogId] [int] IDENTITY(1,1) NOT NULL,
		[AuditLogTransactionId] [int] NOT NULL,
		[PrimaryKey] [nvarchar](1500) NOT NULL,
		[ColumnName] [nvarchar](128) NOT NULL,
		[OldValue] [nvarchar](max) NULL,
		[NewValue] [nvarchar](max) NULL,
		[Key1] [nvarchar](500) NULL,
		[Key2] [nvarchar](500) NULL,
		[Key3] [nvarchar](500) NULL,
		[Key4] [nvarchar](500) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[AuditLogId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
	) TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [audit].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AuditLog_TransactionId] FOREIGN KEY([AuditLogTransactionId])
	REFERENCES [audit].[AuditLogTransaction] ([AuditLogTransactionId])

	ALTER TABLE [audit].[AuditLog] CHECK CONSTRAINT [FK_AuditLog_TransactionId]  
  END
GO

CREATE PROCEDURE #spAlterColumn_AuditLog
    @table varchar(100),
    @column varchar(100),
    @type varchar(50),
    @required bit
AS
if exists (select * from syscolumns where name=@column and id=object_id(@table))
begin
	declare @nullstring varchar(8)
	set @nullstring = case when @required=0 then 'null' else 'not null' end
	exec('alter table [' + @table + '] alter column [' + @column + '] ' + @type + ' ' + @nullstring)
end
GO

if exists(select * from information_schema.columns where table_name='AuditLog' and column_name='OldValue') 
  BEGIN
	exec #spAlterColumn_AuditLog 'AuditLog', 'OldValue', 'NVarChar(MAX)', 0
  END
GO

if exists(select * from information_schema.columns where table_name='AuditLog' and column_name='NewValue') 
  BEGIN
	exec #spAlterColumn_AuditLog 'AuditLog', 'NewValue', 'NVarChar(MAX)', 0
  END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IDX_AuditLog_AuditLogTransactionId' AND object_id = OBJECT_ID('audit.AuditLog'))
  BEGIN
    CREATE NONCLUSTERED INDEX [IDX_AuditLog_AuditLogTransactionId] ON [audit].[AuditLog]
    (
	    [AuditLogTransactionId] ASC
    )
  END
GO

DECLARE @PrimaryKeyName VARCHAR(50)
DECLARE @Command VARCHAR(500)

IF NOT EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AuditLog' AND TABLE_SCHEMA = 'audit' AND COLUMN_NAME = 'AuditLogId' AND DATA_TYPE = 'bigint')
  BEGIN
	IF (SELECT OBJECTPROPERTY(OBJECT_ID(N'Audit.AuditLogTransaction'),'TableHasPrimaryKey')) = 1
	BEGIN
		ALTER TABLE [audit].[AuditLog] DROP CONSTRAINT IF EXISTS FK_AuditLog_TransactionId
		ALTER TABLE [audit].[AuditLog] DROP CONSTRAINT IF EXISTS FK_AUDIT_LOG_TRANSACTION_ID
		DROP INDEX [audit].[AuditLog].[IDX_AuditLog_AuditLogTransactionId]
		--DECLARE @PrimaryKeyName VARCHAR(50)
		SELECT @PrimaryKeyName = CONSTRAINT_NAME 
		FROM   INFORMATION_SCHEMA.TABLE_CONSTRAINTS
		WHERE  TABLE_NAME = 'AuditLogTransaction'
		   AND TABLE_SCHEMA = 'audit'
		   AND CONSTRAINT_TYPE = 'PRIMARY KEY' 
		--DECLARE @Command VARCHAR(500)
		SELECT @Command = 'ALTER TABLE [Audit].[AuditLogTransaction] DROP CONSTRAINT ' + @PrimaryKeyName
		EXECUTE (@Command)
	END

    DROP INDEX IF EXISTS [IDX_AuditLogTransaction_AuditDate] ON [Audit].[AuditLogTransaction]
    DROP INDEX IF EXISTS [IDX_AuditLogTransaction_TableName] ON [Audit].[AuditLogTransaction]

    ALTER TABLE [Audit].[AuditLogTransaction] ALTER COLUMN AuditLogTransactionId BIGINT
    DROP INDEX IF EXISTS IDX_AuditLog_AuditLogTransactionId ON [Audit].[AuditLog]
    DROP INDEX IF EXISTS IDX_AuditLog_ColumnName ON [Audit].[AuditLog]
    
    ALTER TABLE [audit].[AuditLog] DROP CONSTRAINT IF EXISTS [FK_AuditLog_AuditLogTransactionId]
    ALTER TABLE [audit].[AuditLog] DROP CONSTRAINT IF EXISTS [FK_AuditLog_TransactionId]
	ALTER TABLE [Audit].[AuditLog] ALTER COLUMN AuditLogTransactionId BIGINT

	IF (SELECT OBJECTPROPERTY(OBJECT_ID(N'Audit.AuditLogTransaction'),'TableHasPrimaryKey')) = 0
	BEGIN
		ALTER TABLE [audit].[AuditLogTransaction] ADD PRIMARY KEY CLUSTERED 
		(
			[AuditLogTransactionId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

    	ALTER TABLE [audit].[AuditLog] WITH NOCHECK ADD CONSTRAINT [FK_AuditLog_AuditLogTransactionId] FOREIGN KEY([AuditLogTransactionId]) REFERENCES [audit].[AuditLogTransaction] ([AuditLogTransactionId])
		CREATE NONCLUSTERED INDEX [IDX_AuditLog_AuditLogTransactionId] ON [audit].[AuditLog]
		(
			[AuditLogTransactionId] ASC
		)
	END

	IF (SELECT OBJECTPROPERTY(OBJECT_ID(N'Audit.AuditLog'),'TableHasPrimaryKey')) = 1
	BEGIN
		--DECLARE @PrimaryKeyName VARCHAR(50)
		SELECT @PrimaryKeyName = CONSTRAINT_NAME 
		FROM   INFORMATION_SCHEMA.TABLE_CONSTRAINTS
		WHERE  TABLE_NAME = 'AuditLog'
		   AND TABLE_SCHEMA = 'audit'
		   AND CONSTRAINT_TYPE = 'PRIMARY KEY' 
		--DECLARE @Command VARCHAR(500)
		SELECT @Command = 'ALTER TABLE [Audit].[AuditLog] DROP CONSTRAINT ' + @PrimaryKeyName
		EXECUTE (@Command)
	END
    ALTER TABLE [Audit].[AuditLog] ALTER COLUMN AuditLogId BIGINT
	
	IF (SELECT OBJECTPROPERTY(OBJECT_ID(N'Audit.AuditLog'),'TableHasPrimaryKey')) = 0
	BEGIN
		ALTER TABLE [audit].[AuditLog] ADD PRIMARY KEY CLUSTERED 
		(
			[AuditLogId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	END
  END

drop procedure #spAlterColumn_AuditLog
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IDX_AuditLog_ColumnName' AND object_id = OBJECT_ID('audit.AuditLog'))
  BEGIN
    CREATE NONCLUSTERED INDEX [IDX_AuditLog_ColumnName] ON [audit].[AuditLog]
    (
	    [ColumnName] ASC
    )
  END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IDX_AuditLog_AuditLogTransactionId' AND object_id = OBJECT_ID('audit.AuditLog'))
  BEGIN
    CREATE INDEX [IDX_AuditLog_AuditLogTransactionId] ON [audit].[AuditLog]
    (
	    [AuditLogTransactionId]  ASC
    )
  END
GO

declare @pk varchar(100)
select @pk = schema_name+'.'+table_name+'.'+pk_name
from (
    select schema_name(tab.schema_id) as [schema_name], pk.[name] as pk_name, tab.[name] as table_name
    from sys.tables tab
    inner join sys.indexes pk on tab.object_id = pk.object_id 
        and pk.is_primary_key = 1
    where schema_name(tab.schema_id)='audit'
        and tab.[name]='AuditLog'
) x

if (@pk != 'audit.AuditLog.PK_AuditLog')
  BEGIN   
    EXEC sp_rename @pk, 'PK_AuditLog'
  END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IDX_AuditLogTransaction_AuditDate' AND object_id = OBJECT_ID('audit.AuditLogTransaction'))
  BEGIN
    CREATE INDEX [IDX_AuditLogTransaction_AuditDate] ON [audit].[AuditLogTransaction]
    (
	    [AuditDate], [AuditLogTransactionId]  ASC
    )
  END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IDX_AuditLogTransaction_TableName' AND object_id = OBJECT_ID('audit.AuditLogTransaction'))
  BEGIN
    CREATE INDEX [IDX_AuditLogTransaction_TableName] ON [audit].[AuditLogTransaction]
    (
	    [TableName], [AuditLogTransactionId]  ASC
    )
  END
GO
