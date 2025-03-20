PRINT 'Before TRY'
BEGIN TRY
	BEGIN TRAN
	PRINT 'First Statement in the TRY block'
BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250319155940_OutboxRemainingAttempts'
)
BEGIN
    DROP INDEX [IX_Status_LockId_ScheduleDate] ON [dbo].[Outbox];
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Outbox]') AND [c].[name] = N'LockId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Outbox] DROP CONSTRAINT [' + @var + '];');
    ALTER TABLE [dbo].[Outbox] ALTER COLUMN [LockId] uniqueidentifier NULL;
    CREATE INDEX [IX_Status_LockId_ScheduleDate] ON [dbo].[Outbox] ([Status], [LockId], [ScheduledDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250319155940_OutboxRemainingAttempts'
)
BEGIN
    ALTER TABLE [dbo].[Outbox] ADD [RemainingAttempts] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250319155940_OutboxRemainingAttempts'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250319155940_OutboxRemainingAttempts', N'9.0.3');
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
