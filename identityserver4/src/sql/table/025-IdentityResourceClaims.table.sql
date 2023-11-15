IF (NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'IdentityResourceClaims'))
BEGIN
    CREATE TABLE [AUTH].IdentityResourceClaims (
        Id                 INT            IDENTITY (1, 1) NOT NULL,
        [Type]             NVARCHAR (200) NOT NULL,
        IdentityResourceId INT            NOT NULL,
        CONSTRAINT PK_IdentityResourceClaims PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_IdentityResourceClaims_IdentityResources_IdentityResourceId FOREIGN KEY (IdentityResourceId) REFERENCES [AUTH].IdentityResources (Id) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_IdentityResourceClaims_IdentityResourceId] ON [AUTH].IdentityResourceClaims(IdentityResourceId ASC);
END
