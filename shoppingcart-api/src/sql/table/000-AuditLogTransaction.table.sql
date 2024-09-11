IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE   schema_name = 'audit' ) 
  BEGIN
	EXEC sp_executesql N'CREATE SCHEMA audit'
  END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[audit].[AuditLogTransaction]') AND type in (N'U'))
  BEGIN
	CREATE TABLE [audit].[AuditLogTransaction](
		[AuditLogTransactionId] [int] IDENTITY(1,1) NOT NULL,
		[DatabaseName] [nvarchar](128) NOT NULL DEFAULT (db_name()),
		[TableName] [nvarchar](261) NOT NULL,
		[TableSchema] [nvarchar](261) NOT NULL,
		Action varchar(10) NOT NULL,
		[HostName] [varchar](128) NOT NULL,
		[ApplicationName] [varchar](128) NOT NULL,
		[AuditLogin] [varchar](128) NOT NULL,
		[AuditDate] [datetime] NOT NULL,
		[AffectedRows] [int] NOT NULL,
		[UserId] varchar(100) NULL,
		TransactionId bigint NOT NULL
	PRIMARY KEY CLUSTERED 
	(
		[AuditLogTransactionId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
	) 
  END
GO

DROP INDEX IF EXISTS NC_auditdate ON [Audit].[AuditLogTransaction]
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

declare @pk varchar(100)
select @pk = schema_name+'.'+table_name+'.'+pk_name
from (
    select schema_name(tab.schema_id) as [schema_name], pk.[name] as pk_name, tab.[name] as table_name
    from sys.tables tab
    inner join sys.indexes pk on tab.object_id = pk.object_id 
        and pk.is_primary_key = 1
    where schema_name(tab.schema_id)='audit'
        and tab.[name]='AuditLogTransaction'
    --order by schema_name(tab.schema_id), pk.[name]
) x

if (@pk != 'audit.AuditLogTransaction.PK_AuditLogTransaction')
  BEGIN
    EXEC sp_rename @pk, 'PK_AuditLogTransaction'
  END
