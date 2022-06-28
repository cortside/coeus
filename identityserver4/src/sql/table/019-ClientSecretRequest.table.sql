/****** Object:  Table [auth].[ClientSecretRequest]    Script Date: 09/20/2021 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'ClientSecretRequest')
CREATE TABLE [auth].[ClientSecretRequest](
	[ClientSecretRequestId] [uniqueidentifier] NOT NULL,
	[ClientId] [int] NOT NULL,
	[Token] [uniqueidentifier] NOT NULL,
	[SmsVerificationCode] varchar(6) NULL,
	[RequestExpirationDate] [datetime2] NOT NULL,
	[SmsVerificationExpiration] [datetime2] NULL,
	[RemainingSmsVerificationAttempts] INT NOT NULL,
	[CreateDate] [datetime2] NOT NULL,
	[CreateUserId] [uniqueidentifier] NOT NULL,
	[LastModifiedDate] [datetime2] NOT NULL,
	[LastModifiedUserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ClientSecretRequest] PRIMARY KEY CLUSTERED 
(
	[ClientSecretRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_ClientSecrets_ClientId]    Script Date: 09/20/2021 ******/
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ClientSecrets_ClientId' AND object_id = OBJECT_ID(N'[auth].[ClientSecretRequest]'))
CREATE NONCLUSTERED INDEX [IX_ClientSecrets_ClientId] ON [auth].[ClientSecretRequest]
(
	[ClientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab WHERE CONSTRAINT_SCHEMA = N'AUTH' and 
TABLE_NAME = N'ClientSecretRequest' and CONSTRAINT_Name = N'FK_ClientSecretRequest_Clients_ClientId' and CONSTRAINT_TYPE = N'FOREIGN KEY')
ALTER TABLE [auth].[ClientSecretRequest]  WITH CHECK ADD  CONSTRAINT [FK_ClientSecretRequest_Clients_ClientId] FOREIGN KEY([ClientId])
REFERENCES [auth].[Clients] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [auth].[ClientSecretRequest] CHECK CONSTRAINT [FK_ClientSecretRequest_Clients_ClientId]
GO
