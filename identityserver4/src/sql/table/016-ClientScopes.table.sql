/****** Object:  Table [auth].[ClientScopes]    Script Date: 5/3/2017 10:36:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'ClientScopes')
CREATE TABLE [auth].[ClientScopes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NOT NULL,
	[Scope] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_ClientScopes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_ClientScopes_ClientId]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = 'IX_ClientScopes_ClientId' AND object_id = OBJECT_ID(N'[auth].[ClientScopes]'))
CREATE NONCLUSTERED INDEX [IX_ClientScopes_ClientId] ON [auth].[ClientScopes]
(
	[ClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab WHERE CONSTRAINT_SCHEMA = N'AUTH' and 
TABLE_NAME = N'ClientScopes' and CONSTRAINT_Name = N'FK_ClientScopes_Clients_ClientId' and CONSTRAINT_TYPE = N'FOREIGN KEY')
ALTER TABLE [auth].[ClientScopes]  WITH CHECK ADD  CONSTRAINT [FK_ClientScopes_Clients_ClientId] FOREIGN KEY([ClientId])
REFERENCES [auth].[Clients] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [auth].[ClientScopes] CHECK CONSTRAINT [FK_ClientScopes_Clients_ClientId]
GO

