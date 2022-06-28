BEGIN TRANSACTION

--Add New Tables

-- Add ApiResourceScopes
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

-- Add ApiScopeProperties
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





-- Add Renamed Tables

-- ApiResourceClaims
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


-- ApiResourceProperties
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



-- Add ApiResourceSecrets
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

-- IdentityResourceClaims
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

-- IdentityResourceProperties
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

-- Migrate Existing Data

--ApiClaims -> ApiResourceClaims
SET IDENTITY_INSERT [AUTH].ApiResourceClaims ON;

INSERT INTO [AUTH].ApiResourceClaims (Id, [Type], ApiResourceId)
SELECT Id, [Type], ApiResourceId FROM [AUTH].ApiClaims ac
WHERE NOT EXISTS (SELECT * FROM [Auth].ApiResourceClaims arc WHERE arc.ApiResourceId = ac.ApiResourceId)

SET IDENTITY_INSERT [AUTH].ApiResourceClaims OFF;

--ApiProperties -> ApiResourceProperties
SET IDENTITY_INSERT [AUTH].ApiResourceProperties ON;  

INSERT INTO [AUTH].ApiResourceProperties (Id, [Key], [Value], ApiResourceId)
SELECT Id, [Key], [Value], ApiResourceId FROM [AUTH].ApiProperties ap
WHERE NOT EXISTS (SELECT * FROM [Auth].ApiResourceProperties arp WHERE arp.ApiResourceId = ap.ApiResourceId)

SET IDENTITY_INSERT [AUTH].ApiResourceProperties OFF;

--ApiSecrets -> ApiResourceSecrets
SET IDENTITY_INSERT [AUTH].ApiResourceSecrets ON;  

INSERT INTO [AUTH].ApiResourceSecrets (Id, [Description], [Value], Expiration, [Type], Created, ApiResourceId)
SELECT Id, [Description], [Value], Expiration, [Type], Created, ApiResourceId FROM [AUTH].ApiSecrets apisecrets
WHERE NOT EXISTS (SELECT * FROM [Auth].ApiResourceSecrets ars WHERE ars.ApiResourceId = apisecrets.ApiResourceId)

SET IDENTITY_INSERT [AUTH].ApiResourceSecrets OFF;  

--IdentityClaims -> IdentityResourceClaims
SET IDENTITY_INSERT [AUTH].IdentityResourceClaims ON;  

INSERT INTO [AUTH].IdentityResourceClaims (Id, [Type], IdentityResourceId)
SELECT Id, [Type], IdentityResourceId FROM [AUTH].IdentityClaims ic
WHERE NOT EXISTS (SELECT * FROM [Auth].IdentityResourceClaims irc WHERE irc.IdentityResourceId = ic.IdentityResourceId)

SET IDENTITY_INSERT [AUTH].IdentityResourceClaims OFF;  


--IdentityProperties -> IdentityResourceProperties
SET IDENTITY_INSERT [AUTH].IdentityResourceProperties ON;  

INSERT INTO [AUTH].IdentityResourceProperties (Id, [Key], [Value], IdentityResourceId)
SELECT Id, [Key], [Value], IdentityResourceId FROM [AUTH].IdentityProperties ip
WHERE NOT EXISTS (SELECT * FROM [Auth].IdentityResourceProperties irp WHERE irp.IdentityResourceId = ip.IdentityResourceId)

SET IDENTITY_INSERT [AUTH].IdentityResourceProperties OFF;  

-- ApiScopes -> ApiResourceScopes

INSERT INTO [AUTH].ApiResourceScopes ([Scope], [ApiResourceId])
SELECT [Name], [ApiResourceId] FROM [AUTH].ApiScopes apiscopes
WHERE NOT EXISTS (SELECT * FROM [Auth].ApiResourceScopes ars WHERE ars.ApiResourceId = apiscopes.ApiResourceId)

-- Alter Existing Tables

-- ApiResources
IF COL_LENGTH('[AUTH].ApiResources', 'AllowedAccessTokenSigningAlgorithms') IS NULL
BEGIN
	ALTER TABLE [AUTH].ApiResources ADD AllowedAccessTokenSigningAlgorithms NVARCHAR (100) NULL
END

IF COL_LENGTH('[AUTH].ApiResources', 'ShowInDiscoveryDocument') IS NULL
BEGIN
	ALTER TABLE [AUTH].ApiResources ADD ShowInDiscoveryDocument BIT DEFAULT 0 NOT NULL;
END


-- ApiScopeClaims

--ALTER TABLE [AUTH].ApiScopeClaims DROP CONSTRAINT FK_ApiScopeClaims_ApiScopes_ApiScopeId
	
--DROP INDEX IX_ApiScopeClaims_ApiScopeId ON [AUTH].ApiScopeClaims

IF COL_LENGTH('[AUTH].[ApiScopeClaims]', 'ScopeId') IS NULL
BEGIN
	EXEC SP_RENAME '[AUTH].[ApiScopeClaims].[ApiScopeId]', 'ScopeId', 'COLUMN';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ApiScopeClaims_ScopeId' AND object_id = OBJECT_ID('[AUTH].ApiScopeClaims'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_ApiScopeClaims_ScopeId ON [AUTH].ApiScopeClaims(ScopeId ASC);
END
	
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_ApiScopeClaims_ApiScopes_ScopeId')
BEGIN
	ALTER TABLE [AUTH].ApiScopeClaims ADD CONSTRAINT FK_ApiScopeClaims_ApiScopes_ScopeId FOREIGN KEY (ScopeId) REFERENCES [AUTH].ApiScopes (Id) ON DELETE NO ACTION
END
	
-- ApiScopes
-- IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_ApiScopes_ApiResources_ApiResourceId')
-- BEGIN
	-- -- ALTER TABLE [AUTH].ApiScopes DROP CONSTRAINT FK_ApiScopes_ApiResources_ApiResourceId
-- END
	
IF EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ApiScopes_ApiResourceId' AND object_id = OBJECT_ID('[AUTH].ApiScopes'))
BEGIN
	DROP INDEX IX_ApiScopes_ApiResourceId ON [AUTH].ApiScopes
END

IF COL_LENGTH('[AUTH].ApiScopes', 'Enabled') IS NULL
BEGIN
    ALTER TABLE [AUTH].ApiScopes ADD Enabled BIT DEFAULT 1 NOT NULL;
END

-- Clients
IF COL_LENGTH('[AUTH].Clients', 'AllowedIdentityTokenSigningAlgorithms') IS NULL
BEGIN
	ALTER TABLE [AUTH].Clients ADD AllowedIdentityTokenSigningAlgorithms NVARCHAR (100) NULL
END

IF COL_LENGTH('[AUTH].Clients', 'RequireRequestObject') IS NULL
BEGIN
    ALTER TABLE [AUTH].Clients ADD RequireRequestObject BIT DEFAULT 0 NOT NULL;
END

-- Delete Old Tables

--DROP TABLE [AUTH].ApiClaims
--DROP TABLE [AUTH].ApiProperties
--DROP TABLE [AUTH].ApiSecrets
--DROP TABLE [AUTH].IdentityClaims
--DROP TABLE [AUTH].IdentityProperties

COMMIT TRANSACTION
