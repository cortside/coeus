IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = 'audit' ) 
  BEGIN
	EXEC sp_executesql N'CREATE SCHEMA audit'
  END
GO

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

	ALTER TABLE [audit].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AUDIT_LOG_TRANSACTION_ID] FOREIGN KEY([AuditLogTransactionId])
	REFERENCES [audit].[AuditLogTransaction] ([AuditLogTransactionId])

	ALTER TABLE [audit].[AuditLog] CHECK CONSTRAINT [FK_AUDIT_LOG_TRANSACTION_ID]  
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

drop procedure #spAlterColumn_AuditLog
GO
