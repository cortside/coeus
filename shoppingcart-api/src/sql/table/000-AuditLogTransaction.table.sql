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
