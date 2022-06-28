IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'auth' AND TABLE_NAME = 'IdentityProperties')
CREATE TABLE [auth].[IdentityProperties] (
    [Id] int NOT NULL IDENTITY,
    [Key] nvarchar(250) NOT NULL,
    [Value] nvarchar(2000) NOT NULL,
    [IdentityResourceId] int NOT NULL,
    CONSTRAINT [PK_IdentityProperties] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_IdentityProperties_IdentityResources_IdentityResourceId] FOREIGN KEY ([IdentityResourceId]) REFERENCES [auth].[IdentityResources] ([Id]) ON DELETE CASCADE
);

GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'IX_IdentityProperties_IdentityResourceId' AND object_id = OBJECT_ID(N'[auth].[IdentityProperties]'))
CREATE INDEX [IX_IdentityProperties_IdentityResourceId] ON [auth].[IdentityProperties] ([IdentityResourceId]);

GO