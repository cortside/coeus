/****** Object:  Table [auth].[ApiScopes]    Script Date: 5/3/2017 10:24:01 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'ApiScopes')
CREATE TABLE [auth].[ApiScopes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApiResourceId] [int] NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[DisplayName] [nvarchar](200) NULL,
	[Emphasize] [bit] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Required] [bit] NOT NULL,
	[ShowInDiscoveryDocument] [bit] NOT NULL,
 CONSTRAINT [PK_ApiScopes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApiScopes_ApiResourceId]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ApiScopes_ApiResourceId' AND object_id = OBJECT_ID(N'[auth].[ApiScopes]'))
CREATE NONCLUSTERED INDEX [IX_ApiScopes_ApiResourceId] ON [auth].[ApiScopes]
(
	[ApiResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApiScopes_Name]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ApiScopes_Name' AND object_id = OBJECT_ID(N'[auth].[ApiScopes]'))
CREATE UNIQUE NONCLUSTERED INDEX [IX_ApiScopes_Name] ON [auth].[ApiScopes]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF COL_LENGTH('[AUTH].ApiScopes', 'Enabled') IS NULL
BEGIN
    ALTER TABLE [AUTH].ApiScopes ADD Enabled BIT DEFAULT 1 NOT NULL;
END

GO
