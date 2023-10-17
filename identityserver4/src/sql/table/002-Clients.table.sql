/****** Object:  Table [auth].[Clients]    Script Date: 5/3/2017 10:36:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'Clients')
CREATE TABLE auth.[Clients] (
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [AbsoluteRefreshTokenLifetime] int NOT NULL,
    [AccessTokenLifetime] int NOT NULL,
    [AccessTokenType] int NOT NULL,
    [AllowAccessTokensViaBrowser] bit NOT NULL,
    [AllowOfflineAccess] bit NOT NULL,
    [AllowPlainTextPkce] bit NOT NULL,
    [AllowRememberConsent] bit NOT NULL,
    [AlwaysIncludeUserClaimsInIdToken] bit NOT NULL,
    [AlwaysSendClientClaims] bit NOT NULL,
    [AuthorizationCodeLifetime] int NOT NULL,
    [BackChannelLogoutSessionRequired] bit NOT NULL,
    [BackChannelLogoutUri] nvarchar(2000) NULL,
    [ClientClaimsPrefix] nvarchar(200) NULL,
    [ClientId] nvarchar(200) NOT NULL,
    [ClientName] nvarchar(200) NULL,
    [ClientUri] nvarchar(2000) NULL,
    [ConsentLifetime] int NULL,
    [Description] nvarchar(1000) NULL,
    [EnableLocalLogin] bit NOT NULL,
    [Enabled] bit NOT NULL,
    [FrontChannelLogoutSessionRequired] bit NOT NULL,
    [FrontChannelLogoutUri] nvarchar(2000) NULL,
    [IdentityTokenLifetime] int NOT NULL,
    [IncludeJwtId] bit NOT NULL,
    [LogoUri] nvarchar(2000) NULL,
    [PairWiseSubjectSalt] nvarchar(200) NULL,
    [LogoutSessionRequired] [bit] NOT NULL,
    [LogoutUri] [nvarchar](max) NULL,
    [PrefixClientClaims] [bit] NOT NULL,
    [ProtocolType] nvarchar(200) NOT NULL,
    [RefreshTokenExpiration] int NOT NULL,
    [RefreshTokenUsage] int NOT NULL,
    [RequireClientSecret] bit NOT NULL,
    [RequireConsent] bit NOT NULL,
    [RequirePkce] bit NOT NULL,
    [SlidingRefreshTokenLifetime] int NOT NULL,
    [UpdateAccessTokenClaimsOnRefresh] bit NOT NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Clients_ClientId]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_Clients_ClientId' AND object_id = OBJECT_ID(N'[auth].[Clients]'))
CREATE UNIQUE NONCLUSTERED INDEX [IX_Clients_ClientId] ON [auth].[Clients]
(
	[ClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO



DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[auth].[Clients]') AND [c].[name] = N'LogoUri');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [auth].[Clients] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [auth].[Clients] ALTER COLUMN [LogoUri] nvarchar(2000) NULL;

GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'Clients'
    AND COLUMN_NAME = 'Created')
ALTER TABLE [auth].[Clients] ADD [Created] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'Clients'
    AND COLUMN_NAME = 'DeviceCodeLifetime')
ALTER TABLE [auth].[Clients] ADD [DeviceCodeLifetime] int NOT NULL DEFAULT 0;

GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'Clients'
    AND COLUMN_NAME = 'LastAccessed')
ALTER TABLE [auth].[Clients] ADD [LastAccessed] datetime2 NULL;

GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'Clients'
    AND COLUMN_NAME = 'Updated')
ALTER TABLE [auth].[Clients] ADD [Updated] datetime2 NULL;

GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'Clients'
    AND COLUMN_NAME = 'UserCodeType')
ALTER TABLE [auth].[Clients] ADD [UserCodeType] nvarchar(100) NULL;

GO

IF NOT EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'Clients'
    AND COLUMN_NAME = 'UserSsoLifetime')
ALTER TABLE [auth].[Clients] ADD [UserSsoLifetime] int NULL;

GO

IF EXISTS(
  SELECT *
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    TABLE_NAME = 'Clients'
    AND COLUMN_NAME = 'PrefixClientClaims')
EXEC sp_rename N'[auth].[Clients].[PrefixClientClaims]', N'NonEditable', N'COLUMN';

GO

IF COL_LENGTH('[AUTH].Clients', 'AllowedIdentityTokenSigningAlgorithms') IS NULL
BEGIN
	ALTER TABLE [AUTH].Clients ADD AllowedIdentityTokenSigningAlgorithms NVARCHAR (100) NULL
END

IF COL_LENGTH('[AUTH].Clients', 'RequireRequestObject') IS NULL
BEGIN
    ALTER TABLE [AUTH].Clients ADD RequireRequestObject BIT DEFAULT 0 NOT NULL;
END

GO
