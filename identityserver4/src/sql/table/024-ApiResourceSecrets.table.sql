   IF (NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'ApiResourceSecrets'))
BEGIN
    CREATE TABLE [AUTH].ApiResourceSecrets (
        Id              INT             IDENTITY (1, 1) NOT NULL,
        [Description]   NVARCHAR (1000) NULL,
        [Value]         NVARCHAR (4000) NOT NULL,
        Expiration      DATETIME2 (7)   NULL,
        [Type]          NVARCHAR (250)  NOT NULL,
        Created         DATETIME2 (7)   NOT NULL,
        ApiResourceId   INT             NOT NULL,
        CONSTRAINT PK_ApiResourceSecrets PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_ApiResourceSecrets_ApiResources_ApiResourceId FOREIGN KEY (ApiResourceId) REFERENCES [AUTH].ApiResources (Id) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX IX_ApiResourceSecrets_ApiResourceId ON [AUTH].ApiResourceSecrets(ApiResourceId ASC);
END

IF NOT EXISTS(SELECT * FROM SYS.OBJECTS WHERE TYPE = 'D' AND NAME = 'DF_ApiResourceSecrets_Created')
  BEGIN
    ALTER TABLE [auth].ApiResourceSecrets ADD CONSTRAINT DF_ApiResourceSecrets_Created DEFAULT GetUtcDate() FOR Created
  END
