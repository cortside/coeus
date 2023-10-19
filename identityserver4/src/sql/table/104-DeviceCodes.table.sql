IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'DeviceCodes')
CREATE TABLE [auth].[DeviceCodes] (
    [UserCode] nvarchar(200) NOT NULL,
    [DeviceCode] nvarchar(200) NOT NULL,
    [SubjectId] nvarchar(200) NULL,
    [ClientId] nvarchar(200) NOT NULL,
    [CreationTime] datetime2 NOT NULL,
    [Expiration] datetime2 NOT NULL,
    [Data] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_DeviceCodes] PRIMARY KEY ([UserCode])
);

GO

IF COL_LENGTH('[AUTH].DeviceCodes', 'SessionId') IS NULL
BEGIN
    ALTER TABLE [AUTH].DeviceCodes ADD SessionId NVARCHAR (100) NULL
END

IF COL_LENGTH('[AUTH].DeviceCodes', 'Description') IS NULL
BEGIN
    ALTER TABLE [AUTH].DeviceCodes ADD [Description] NVARCHAR (200) NULL
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_DeviceCodes_DeviceCode' AND object_id = OBJECT_ID(N'[auth].[DeviceCodes]'))
CREATE UNIQUE INDEX [IX_DeviceCodes_DeviceCode] ON [auth].[DeviceCodes] ([DeviceCode]);

GO
