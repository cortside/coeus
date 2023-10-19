/****** Object:  Table [auth].[ClientCorsOrigins]    Script Date: 5/3/2017 10:35:21 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'ClientCorsOrigins')
CREATE TABLE [auth].[ClientCorsOrigins](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NOT NULL,
	[Origin] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_ClientCorsOrigins] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_ClientCorsOrigins_ClientId]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ClientCorsOrigins_ClientId' AND object_id = OBJECT_ID(N'[auth].[ClientCorsOrigins]'))
CREATE NONCLUSTERED INDEX [IX_ClientCorsOrigins_ClientId] ON [auth].[ClientCorsOrigins]
(
	[ClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab WHERE CONSTRAINT_SCHEMA = N'AUTH' and 
TABLE_NAME = N'ClientCorsOrigins' and CONSTRAINT_Name = N'FK_ClientCorsOrigins_Clients_ClientId' and CONSTRAINT_TYPE = N'FOREIGN KEY')
ALTER TABLE [auth].[ClientCorsOrigins]  WITH CHECK ADD  CONSTRAINT [FK_ClientCorsOrigins_Clients_ClientId] FOREIGN KEY([ClientId])
REFERENCES [auth].[Clients] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [auth].[ClientCorsOrigins] CHECK CONSTRAINT [FK_ClientCorsOrigins_Clients_ClientId]
GO


