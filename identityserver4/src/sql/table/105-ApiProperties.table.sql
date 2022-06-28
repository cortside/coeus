IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'ApiProperties')
CREATE TABLE [auth].[ApiProperties] (
    [Id] int NOT NULL IDENTITY,
    [Key] nvarchar(250) NOT NULL,
    [Value] nvarchar(2000) NOT NULL,
    [ApiResourceId] int NOT NULL,
    CONSTRAINT [PK_ApiProperties] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ApiProperties_ApiResources_ApiResourceId] FOREIGN KEY ([ApiResourceId]) REFERENCES [auth].[ApiResources] ([Id]) ON DELETE CASCADE
);

GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_ApiProperties_ApiResourceId' AND object_id = OBJECT_ID(N'[auth].[ApiProperties]'))
CREATE INDEX [IX_ApiProperties_ApiResourceId] ON [auth].[ApiProperties] ([ApiResourceId]);

GO