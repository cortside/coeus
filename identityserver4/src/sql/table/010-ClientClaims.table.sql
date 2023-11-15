/****** Object:  Table [auth].[ClientClaims]    Script Date: 5/3/2017 10:34:51 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'ClientClaims')
CREATE TABLE [auth].[ClientClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientId] [int] NOT NULL,
	[Type] [nvarchar](250) NOT NULL,
	[Value] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_ClientClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_ClientClaims_ClientId]    Script Date: 1/24/2018 9:29:48 AM ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ClientClaims_ClientId' AND object_id = OBJECT_ID(N'[auth].[ClientClaims]'))
CREATE NONCLUSTERED INDEX [IX_ClientClaims_ClientId] ON [auth].[ClientClaims]
(
	[ClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab WHERE CONSTRAINT_SCHEMA = N'AUTH' and 
TABLE_NAME = N'ClientClaims' and CONSTRAINT_Name = N'FK_ClientClaims_Clients_ClientId' and CONSTRAINT_TYPE = N'FOREIGN KEY')
ALTER TABLE [auth].[ClientClaims]  WITH CHECK ADD  CONSTRAINT [FK_ClientClaims_Clients_ClientId] FOREIGN KEY([ClientId])
REFERENCES [auth].[Clients] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [auth].[ClientClaims] CHECK CONSTRAINT [FK_ClientClaims_Clients_ClientId]
GO

-- OD-10262 Enforce Uniqueness for Clients in Identity Server
-- Ensure the guids are unique in the ClientClaims table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='UNIQUE_ClientClaims_sub_Value' AND object_id = OBJECT_ID('[AUTH].[ClientClaims]'))
BEGIN
	CREATE UNIQUE INDEX [UNIQUE_ClientClaims_sub_Value]
	ON [AUTH].[ClientClaims]([Value])
	WHERE ([Type] = 'sub')
END
GO
