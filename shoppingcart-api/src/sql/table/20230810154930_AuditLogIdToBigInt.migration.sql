PRINT 'Before TRY'
BEGIN TRY
	BEGIN TRAN
	PRINT 'First Statement in the TRY block'
BEGIN TRANSACTION;
IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230810154930_AuditLogIdToBigInt')
BEGIN
	IF (SELECT OBJECTPROPERTY(OBJECT_ID(N'Audit.AuditLog'),'TableHasPrimaryKey')) = 1
	BEGIN
		DECLARE @PrimaryKeyName VARCHAR(50)
		SELECT @PrimaryKeyName = CONSTRAINT_NAME 
		FROM   INFORMATION_SCHEMA.TABLE_CONSTRAINTS
		WHERE  TABLE_NAME = 'AuditLog'
		   AND TABLE_SCHEMA = 'audit'
		   AND CONSTRAINT_TYPE = 'PRIMARY KEY' 
		DECLARE @Command VARCHAR(500)
		SELECT @Command = 'ALTER TABLE [Audit].[AuditLog] DROP CONSTRAINT ' + @PrimaryKeyName
		EXECUTE (@Command)
	END
    ALTER TABLE [Audit].[AuditLog] ALTER COLUMN AuditLogId BIGINT
	
	IF (SELECT OBJECTPROPERTY(OBJECT_ID(N'Audit.AuditLog'),'TableHasPrimaryKey')) = 0
	BEGIN
		ALTER TABLE [audit].[AuditLog] ADD PRIMARY KEY CLUSTERED 
		(
			[AuditLogId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	END
END;
IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230810154930_AuditLogIdToBigInt')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230810154930_AuditLogIdToBigInt', N'6.0.16');
END;
COMMIT;
	PRINT 'Last Statement in the TRY block'
	COMMIT TRAN
END TRY
BEGIN CATCH
    PRINT 'In CATCH Block'
    IF(@@TRANCOUNT > 0)
        ROLLBACK TRAN;
    THROW; -- Raise error to the client.
END CATCH
PRINT 'After END CATCH'
GO