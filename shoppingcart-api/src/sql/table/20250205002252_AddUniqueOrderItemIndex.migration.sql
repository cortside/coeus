PRINT 'Before TRY'
BEGIN TRY
	BEGIN TRAN
	PRINT 'First Statement in the TRY block'
BEGIN TRANSACTION;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250205002252_AddUniqueOrderItemIndex'
)
BEGIN
    DROP INDEX [IX_OrderItem_OrderId] ON [dbo].[OrderItem];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250205002252_AddUniqueOrderItemIndex'
)
BEGIN
    CREATE INDEX [IX_Status_LastModifiedDate] ON [dbo].[Outbox] ([Status], [LastModifiedDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250205002252_AddUniqueOrderItemIndex'
)
BEGIN
    CREATE UNIQUE INDEX [IX_OrderItem_OrderId_ItemId] ON [dbo].[OrderItem] ([OrderId], [ItemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250205002252_AddUniqueOrderItemIndex'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250205002252_AddUniqueOrderItemIndex', N'8.0.11');
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
