IF (NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'IdentityResourceProperties'))
BEGIN
    CREATE TABLE [AUTH].IdentityResourceProperties (
        Id                   INT             IDENTITY (1, 1) NOT NULL,
        [Key]                NVARCHAR (250)  NOT NULL,
        [Value]              NVARCHAR (2000) NOT NULL,
        IdentityResourceId   INT             NOT NULL,
        CONSTRAINT PK_IdentityResourceProperties PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_IdentityResourceProperties_IdentityResources_IdentityResourceId FOREIGN KEY (IdentityResourceId) REFERENCES [AUTH].IdentityResources (Id) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX IX_IdentityResourceProperties_IdentityResourceId ON [AUTH].IdentityResourceProperties(IdentityResourceId ASC);
END
