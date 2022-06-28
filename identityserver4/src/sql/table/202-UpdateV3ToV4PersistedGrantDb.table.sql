BEGIN TRANSACTION

-- Alter Existing Tables

-- DeviceCodes
IF COL_LENGTH('[AUTH].DeviceCodes', 'SessionId') IS NULL
BEGIN
    ALTER TABLE [AUTH].DeviceCodes ADD SessionId NVARCHAR (100) NULL
END

IF COL_LENGTH('[AUTH].DeviceCodes', 'Description') IS NULL
BEGIN
    ALTER TABLE [AUTH].DeviceCodes ADD [Description] NVARCHAR (200) NULL
END


-- PersistedGrants
IF COL_LENGTH('[AUTH].PersistedGrants', 'SessionId') IS NULL
BEGIN
    ALTER TABLE [AUTH].PersistedGrants ADD SessionId NVARCHAR (100) NULL
END

IF COL_LENGTH('[AUTH].PersistedGrants', 'Description') IS NULL
BEGIN	
    ALTER TABLE [AUTH].PersistedGrants ADD [Description] NVARCHAR (200) NULL
END

IF COL_LENGTH('[AUTH].PersistedGrants', 'ConsumedTime') IS NULL
BEGIN	
    ALTER TABLE [AUTH].PersistedGrants ADD ConsumedTime DATETIME2 (7) NULL
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_PersistedGrants_SubjectId_SessionId_Type' AND object_id = OBJECT_ID('[AUTH].PersistedGrants'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PersistedGrants_SubjectId_SessionId_Type ON [AUTH].PersistedGrants(SubjectId ASC, SessionId ASC, Type ASC);
END


COMMIT TRANSACTION
