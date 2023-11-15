IF (NOT EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_NAME = 'ApiResourceProperties'))
BEGIN
    CREATE TABLE [AUTH].[ApiResourceProperties] (
        [Id]            INT             IDENTITY (1, 1) NOT NULL,
        [Key]           NVARCHAR (250)  NOT NULL,
        [Value]         NVARCHAR (2000) NOT NULL,
        ApiResourceId   INT             NOT NULL,
        CONSTRAINT PK_ApiResourceProperties PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_ApiResourceProperties_ApiResources_ApiResourceId FOREIGN KEY (ApiResourceId) REFERENCES [AUTH].ApiResources (Id) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_ApiResourceProperties_ApiResourceId] ON [AUTH].[ApiResourceProperties]([ApiResourceId] ASC);
END
