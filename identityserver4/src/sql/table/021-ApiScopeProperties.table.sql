IF (NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'ApiScopeProperties'))
BEGIN
    CREATE TABLE [AUTH].ApiScopeProperties (
        Id        INT             IDENTITY (1, 1) NOT NULL,
        [Key]     NVARCHAR (250)  NOT NULL,
        [Value]   NVARCHAR (2000) NOT NULL,
        ScopeId   INT             NOT NULL,
        CONSTRAINT PK_ApiScopeProperties PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_ApiScopeProperties_ApiScopes_ScopeId FOREIGN KEY (ScopeId) REFERENCES [AUTH].ApiScopes (Id) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX IX_ApiScopeProperties_ScopeId ON [AUTH].ApiScopeProperties(ScopeId ASC);
END
