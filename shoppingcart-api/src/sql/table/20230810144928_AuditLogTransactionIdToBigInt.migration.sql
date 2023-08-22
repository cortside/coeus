PRINT 'Before TRY'
BEGIN TRY
	BEGIN TRAN
	PRINT 'First Statement in the TRY block'
BEGIN TRANSACTION;
IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230810144928_AuditLogTransactionIdToBigInt')
BEGIN
	IF (SELECT OBJECTPROPERTY(OBJECT_ID(N'Audit.AuditLogTransaction'),'TableHasPrimaryKey')) = 1
	BEGIN
		ALTER TABLE [audit].[AuditLog] DROP CONSTRAINT FK_AUDIT_LOG_TRANSACTION_ID
		DROP INDEX [audit].[AuditLog].[IDX_AuditLog_AuditLogTransactionId]
		DECLARE @PrimaryKeyName VARCHAR(50)
		SELECT @PrimaryKeyName = CONSTRAINT_NAME 
		FROM   INFORMATION_SCHEMA.TABLE_CONSTRAINTS
		WHERE  TABLE_NAME = 'AuditLogTransaction'
		   AND TABLE_SCHEMA = 'audit'
		   AND CONSTRAINT_TYPE = 'PRIMARY KEY' 
		DECLARE @Command VARCHAR(500)
		SELECT @Command = 'ALTER TABLE [Audit].[AuditLogTransaction] DROP CONSTRAINT ' + @PrimaryKeyName
		EXECUTE (@Command)
	END
    ALTER TABLE [Audit].[AuditLogTransaction] ALTER COLUMN AuditLogTransactionId BIGINT
	ALTER TABLE [Audit].[AuditLog] ALTER COLUMN AuditLogTransactionId BIGINT
	
	IF (SELECT OBJECTPROPERTY(OBJECT_ID(N'Audit.AuditLogTransaction'),'TableHasPrimaryKey')) = 0
	BEGIN
		ALTER TABLE [audit].[AuditLogTransaction] ADD PRIMARY KEY CLUSTERED 
		(
			[AuditLogTransactionId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		
		ALTER TABLE [audit].[AuditLog]  WITH CHECK ADD  CONSTRAINT [FK_AUDIT_LOG_TRANSACTION_ID] FOREIGN KEY([AuditLogTransactionId])
		REFERENCES [audit].[AuditLogTransaction] ([AuditLogTransactionId])
		CREATE NONCLUSTERED INDEX [IDX_AuditLog_AuditLogTransactionId] ON [audit].[AuditLog]
		(
			[AuditLogTransactionId] ASC
		)
	END
END;
IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230810144928_AuditLogTransactionIdToBigInt')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230810144928_AuditLogTransactionIdToBigInt', N'6.0.16');
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