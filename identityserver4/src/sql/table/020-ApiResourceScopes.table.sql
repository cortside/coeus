IF (NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'ApiResourceScopes'))
BEGIN
    CREATE TABLE [AUTH].ApiResourceScopes (
        Id             INT            IDENTITY (1, 1) NOT NULL,
        Scope          NVARCHAR (200) NOT NULL,
        ApiResourceId  INT            NOT NULL,
        CONSTRAINT PK_ApiResourceScopes PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_ApiResourceScopes_ApiResources_ApiResourceId FOREIGN KEY (ApiResourceId) REFERENCES [AUTH].[ApiResources] (Id) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX IX_ApiResourceScopes_ApiResourceId
    ON [AUTH].ApiResourceScopes(ApiResourceId ASC);
END
