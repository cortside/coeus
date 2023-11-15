/****** Object:  Table [auth].[ClientSecrets]    Script Date: 5/3/2017 10:37:06 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'ClientSecrets')
CREATE TABLE [auth].[ClientSecrets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[Expiration] [datetime2](7) NULL,
	[Type] [nvarchar](250) NULL,
	[Value] [nvarchar](2000) NOT NULL,
 CONSTRAINT [PK_ClientSecrets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_ClientSecrets_ClientId]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ClientSecrets_ClientId' AND object_id = OBJECT_ID(N'[auth].[ClientSecrets]'))
CREATE NONCLUSTERED INDEX [IX_ClientSecrets_ClientId] ON [auth].[ClientSecrets]
(
	[ClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab WHERE CONSTRAINT_SCHEMA = N'AUTH' and 
TABLE_NAME = N'ClientSecrets' and CONSTRAINT_Name = N'FK_ClientSecrets_Clients_ClientId' and CONSTRAINT_TYPE = N'FOREIGN KEY')
ALTER TABLE [auth].[ClientSecrets]  WITH CHECK ADD  CONSTRAINT [FK_ClientSecrets_Clients_ClientId] FOREIGN KEY([ClientId])
REFERENCES [auth].[Clients] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [auth].[ClientSecrets] CHECK CONSTRAINT [FK_ClientSecrets_Clients_ClientId]
GO


DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[auth].[ClientSecrets]') AND [c].[name] = N'Value');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [auth].[ClientSecrets] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [auth].[ClientSecrets] ALTER COLUMN [Value] nvarchar(4000) NOT NULL;

GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[auth].[ClientSecrets]') AND [c].[name] = N'Type');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [auth].[ClientSecrets] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [auth].[ClientSecrets] ALTER COLUMN [Type] nvarchar(250) NOT NULL;

GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'ClientSecrets'
    AND COLUMN_NAME = 'Created')
ALTER TABLE [auth].[ClientSecrets] ADD [Created] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

GO
