/****** Object:  Table [auth].[ApiSecrets]    Script Date: 5/3/2017 10:24:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'ApiSecrets')
CREATE TABLE [auth].[ApiSecrets](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApiResourceId] [int] NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[Expiration] [datetime2](7) NULL,
	[Type] [nvarchar](250) NULL,
	[Value] [nvarchar](2000) NULL,
 CONSTRAINT [PK_ApiSecrets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApiSecrets_ApiResourceId]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ApiSecrets_ApiResourceId' AND object_id = OBJECT_ID(N'[auth].[ApiSecrets]'))
CREATE NONCLUSTERED INDEX [IX_ApiSecrets_ApiResourceId] ON [auth].[ApiSecrets]
(
	[ApiResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab WHERE CONSTRAINT_SCHEMA = N'AUTH' and 
TABLE_NAME = N'ApiSecrets' and CONSTRAINT_Name = N'FK_ApiSecrets_ApiResources_ApiResourceId' and CONSTRAINT_TYPE = N'FOREIGN KEY')
ALTER TABLE [auth].[ApiSecrets]  WITH CHECK ADD  CONSTRAINT [FK_ApiSecrets_ApiResources_ApiResourceId] FOREIGN KEY([ApiResourceId])
REFERENCES [auth].[ApiResources] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [auth].[ApiSecrets] CHECK CONSTRAINT [FK_ApiSecrets_ApiResources_ApiResourceId]
GO


DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[auth].[ApiSecrets]') AND [c].[name] = N'Value');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [auth].[ApiSecrets] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [auth].[ApiSecrets] ALTER COLUMN [Value] nvarchar(4000) NOT NULL;

GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[auth].[ApiSecrets]') AND [c].[name] = N'Type');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [auth].[ApiSecrets] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [auth].[ApiSecrets] ALTER COLUMN [Type] nvarchar(250) NOT NULL;

GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'ApiSecrets'
    AND COLUMN_NAME = 'Created')
ALTER TABLE [auth].[ApiSecrets] ADD [Created] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

GO

