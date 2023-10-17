IF (NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'ApiResourceClaims'))
BEGIN
    CREATE TABLE [AUTH].[ApiResourceClaims] (
        Id              INT            IDENTITY (1, 1) NOT NULL,
        [Type]          NVARCHAR (200) NOT NULL,
        ApiResourceId   INT            NOT NULL,
        CONSTRAINT PK_ApiResourceClaims PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_ApiResourceClaims_ApiResources_ApiResourceId FOREIGN KEY (ApiResourceId) REFERENCES [AUTH].ApiResources (Id) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_ApiResourceClaims_ApiResourceId] ON [AUTH].ApiResourceClaims([ApiResourceId] ASC);
END
