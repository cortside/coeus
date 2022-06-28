/****** Object:  Table [auth].[IdentityClaims]    Script Date: 5/3/2017 10:40:47 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'IdentityClaims')
CREATE TABLE [auth].[IdentityClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdentityResourceId] [int] NOT NULL,
	[Type] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_IdentityClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_IdentityClaims_IdentityResourceId]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_IdentityClaims_IdentityResourceId' AND object_id = OBJECT_ID(N'[auth].[IdentityClaims]'))
CREATE NONCLUSTERED INDEX [IX_IdentityClaims_IdentityResourceId] ON [auth].[IdentityClaims]
(
	[IdentityResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab WHERE CONSTRAINT_SCHEMA = N'AUTH' and 
TABLE_NAME = N'IdentityClaims' and CONSTRAINT_Name = N'FK_IdentityClaims_IdentityResources_IdentityResourceId' and CONSTRAINT_TYPE = N'FOREIGN KEY')
ALTER TABLE [auth].[IdentityClaims]  WITH CHECK ADD  CONSTRAINT [FK_IdentityClaims_IdentityResources_IdentityResourceId] FOREIGN KEY([IdentityResourceId])
REFERENCES [auth].[IdentityResources] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [auth].[IdentityClaims] CHECK CONSTRAINT [FK_IdentityClaims_IdentityResources_IdentityResourceId]
GO


