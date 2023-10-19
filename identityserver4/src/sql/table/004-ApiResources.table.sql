/****** Object:  Table [auth].[ApiResources]    Script Date: 5/3/2017 10:23:43 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'ApiResources')
CREATE TABLE [auth].[ApiResources](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[DisplayName] [nvarchar](200) NULL,
	[Enabled] [bit] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_ApiResources] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApiResources_Name]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ApiResources_Name' AND object_id = OBJECT_ID(N'[auth].[ApiResources]'))
CREATE UNIQUE NONCLUSTERED INDEX [IX_ApiResources_Name] ON [auth].[ApiResources]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'ApiResources'
    AND COLUMN_NAME = 'Created')
ALTER TABLE [auth].[ApiResources] ADD [Created] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'ApiResources'
    AND COLUMN_NAME = 'LastAccessed')
ALTER TABLE [auth].[ApiResources] ADD [LastAccessed] datetime2 NULL;

GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'ApiResources'
    AND COLUMN_NAME = 'NonEditable')
ALTER TABLE [auth].[ApiResources] ADD [NonEditable] bit NOT NULL DEFAULT 0;

GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'ApiResources'
    AND COLUMN_NAME = 'Updated')
ALTER TABLE [auth].[ApiResources] ADD [Updated] datetime2 NULL;

GO

IF COL_LENGTH('[AUTH].ApiResources', 'AllowedAccessTokenSigningAlgorithms') IS NULL
BEGIN
	ALTER TABLE [AUTH].ApiResources ADD AllowedAccessTokenSigningAlgorithms NVARCHAR (100) NULL
END

IF COL_LENGTH('[AUTH].ApiResources', 'ShowInDiscoveryDocument') IS NULL
BEGIN
	ALTER TABLE [AUTH].ApiResources ADD ShowInDiscoveryDocument BIT DEFAULT 0 NOT NULL;
END

GO
