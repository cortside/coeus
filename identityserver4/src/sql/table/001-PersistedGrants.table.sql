/****** Object:  Table [auth].[PersistedGrants]    Script Date: 5/3/2017 10:40:58 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'PersistedGrants')
CREATE TABLE [auth].[PersistedGrants](
	[Key] [nvarchar](200) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[ClientId] [nvarchar](200) NOT NULL,
	[CreationTime] [datetime2](7) NOT NULL,
	[Data] [nvarchar](max) NOT NULL,
	[Expiration] [datetime2](7) NULL,
	[SubjectId] [nvarchar](200) NULL,
 CONSTRAINT [PK_PersistedGrants] PRIMARY KEY CLUSTERED 
(
	[Key] ASC,
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

IF COL_LENGTH('[AUTH].PersistedGrants', 'SessionId') IS NULL
BEGIN
    ALTER TABLE [AUTH].PersistedGrants ADD SessionId NVARCHAR (100) NULL
END

IF COL_LENGTH('[AUTH].PersistedGrants', 'Description') IS NULL
BEGIN	
    ALTER TABLE [AUTH].PersistedGrants ADD [Description] NVARCHAR (200) NULL
END

IF COL_LENGTH('[AUTH].PersistedGrants', 'ConsumedTime') IS NULL
BEGIN	
    ALTER TABLE [AUTH].PersistedGrants ADD ConsumedTime DATETIME2 (7) NULL
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_PersistedGrants_SubjectId' AND object_id = OBJECT_ID('[auth].[PersistedGrants]'))
    DROP INDEX [IX_PersistedGrants_SubjectId] ON [AUTH].[PersistedGrants];
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_PersistedGrants_SubjectId_ClientId' AND object_id = OBJECT_ID('[auth].[PersistedGrants]'))
    DROP INDEX [IX_PersistedGrants_SubjectId_ClientId] ON [AUTH].[PersistedGrants];
GO

if exists (SELECT * FROM information_schema.table_constraints WHERE constraint_type = 'PRIMARY KEY' AND table_name = 'PersistedGrants')
    ALTER TABLE [AUTH].[PersistedGrants] DROP CONSTRAINT [PK_PersistedGrants];
GO

--select * from dbo.sysobjects where OBJECTPROPERTY(id, N'IsPrimaryKey') = 1
--sp_help "auth.persistedgrants"


ALTER SCHEMA [auth] TRANSFER [AUTH].[PersistedGrants];
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'PK_PersistedGrants') and OBJECTPROPERTY(id, N'IsPrimaryKey') = 1)
    ALTER TABLE [auth].[PersistedGrants] ADD CONSTRAINT [PK_PersistedGrants] PRIMARY KEY ([Key]);
GO

/****** Object:  Index [IX_PersistedGrants_SubjectId_ClientId]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_PersistedGrants_SubjectId_ClientId' AND object_id = OBJECT_ID('[auth].[PersistedGrants]'))
CREATE NONCLUSTERED INDEX [IX_PersistedGrants_SubjectId_ClientId] ON [auth].[PersistedGrants]
(
	[SubjectId] ASC,
	[ClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_PersistedGrants_SubjectId_ClientId_Type]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_PersistedGrants_SubjectId_ClientId_Type' AND object_id = OBJECT_ID('[auth].[PersistedGrants]'))
CREATE NONCLUSTERED INDEX [IX_PersistedGrants_SubjectId_ClientId_Type] ON [auth].[PersistedGrants]
(
	[SubjectId] ASC,
	[ClientId] ASC,
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [IX_PersistedGrants_SubjectId]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'IX_PersistedGrants_SubjectId' AND object_id = OBJECT_ID('[auth].[PersistedGrants]'))
CREATE NONCLUSTERED INDEX [IX_PersistedGrants_SubjectId] ON [auth].[PersistedGrants]
(
	[SubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_PersistedGrants_SubjectId_SessionId_Type' AND object_id = OBJECT_ID('[AUTH].PersistedGrants'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PersistedGrants_SubjectId_SessionId_Type ON [AUTH].PersistedGrants(SubjectId ASC, SessionId ASC, Type ASC);
END
