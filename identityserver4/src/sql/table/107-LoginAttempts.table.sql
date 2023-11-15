SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE #spAlterColumn_LoginAttempts
    @table varchar(100),
    @column varchar(100),
    @type varchar(50),
    @required bit
AS
if exists (select * from information_schema.columns where TABLE_SCHEMA='auth' and table_name=@table and column_name=@column)
begin
	declare @nullstring varchar(8)
	set @nullstring = case when @required=0 then 'null' else 'not null' end
	exec('alter table [auth].[' + @table + '] alter column [' + @column + '] ' + @type + ' ' + @nullstring)
end
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'LoginAttempts')
CREATE TABLE [auth].[LoginAttempts] (
	Id Int IDENTITY(1,1) NOT NULL,
    AttemptedOn DateTime NOT NULL,
    Successful bit NOT NULL,
    UserId uniqueidentifier NOT NULL
);
GO

if not exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='Id')
  BEGIN
	ALTER TABLE [auth].[LoginAttempts] ADD
	    Id Int IDENTITY(1,1) NOT NULL
  END
GO


if exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='Id') and not exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='Id' and is_nullable='NO')
  BEGIN
	exec #spAlterColumn_LoginAttempts 'LoginAttempts', 'Id', 'Int', 1
  END
GO

if not exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='AttemptedOn')
  BEGIN
	ALTER TABLE [auth].[LoginAttempts] ADD
	    AttemptedOn DateTime NOT NULL
  END
GO


if exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='AttemptedOn') and not exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='AttemptedOn' and is_nullable='NO')
  BEGIN
	exec #spAlterColumn_LoginAttempts 'LoginAttempts', 'AttemptedOn', 'DateTime', 1
  END
GO

if not exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='Successful')
  BEGIN
	ALTER TABLE [auth].[LoginAttempts] ADD
	    Successful Bit NOT NULL
  END
GO


if exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='Successful') and not exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='Successful' and is_nullable='NO')
  BEGIN
	exec #spAlterColumn_LoginAttempts 'LoginAttempts', 'Successful', 'Bit', 1
  END
GO

if not exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='UserId')
  BEGIN
	ALTER TABLE [auth].[LoginAttempts] ADD
	    UserId Bit NOT NULL
  END
GO

if not exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='IpAddress')
  BEGIN
	ALTER TABLE [auth].[LoginAttempts] ADD
	    IpAddress varchar(50) NULL
  END
GO

if exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='UserId') and not exists(select * from information_schema.columns where TABLE_SCHEMA = 'auth' and table_name='LoginAttempts' and column_name='UserId' and is_nullable='NO')
  BEGIN
	exec #spAlterColumn_LoginAttempts 'LoginAttempts', 'UserId', 'uniqueidentifier', 1
  END
GO

if not exists (select * from INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab where CONSTRAINT_SCHEMA = 'auth' and TABLE_NAME = 'LoginAttempts' and CONSTRAINT_Name = 'PK_LoginAttempts' and CONSTRAINT_TYPE = 'PRIMARY KEY')
ALTER TABLE [auth].[LoginAttempts] WITH NOCHECK ADD
	CONSTRAINT PK_LoginAttempts PRIMARY KEY CLUSTERED
	(
		Id
	)
GO

if not exists (select * from INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab where CONSTRAINT_SCHEMA = 'auth' and TABLE_NAME = 'LoginAttempts' and CONSTRAINT_Name = 'FK_LoginAttempts_UserId' and CONSTRAINT_TYPE = 'FOREIGN KEY')
ALTER TABLE [auth].[LoginAttempts] ADD
	CONSTRAINT FK_LoginAttempts_UserId FOREIGN KEY
	(
		UserId
	)
	REFERENCES [auth].[User]
	(
		UserId
	)
GO

drop procedure #spAlterColumn_LoginAttempts
GO

