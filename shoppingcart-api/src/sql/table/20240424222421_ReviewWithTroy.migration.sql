PRINT 'Before TRY'
BEGIN TRY
	BEGIN TRAN
	PRINT 'First Statement in the TRY block'
BEGIN TRANSACTION;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240424222421_ReviewWithTroy'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[OrderItem]') AND [c].[name] = N'Sku');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[OrderItem] DROP CONSTRAINT [' + @var0 + '];');
    EXEC(N'UPDATE [dbo].[OrderItem] SET [Sku] = N'''' WHERE [Sku] IS NULL');
    ALTER TABLE [dbo].[OrderItem] ALTER COLUMN [Sku] nvarchar(10) NOT NULL;
    ALTER TABLE [dbo].[OrderItem] ADD DEFAULT N'' FOR [Sku];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240424222421_ReviewWithTroy'
)
BEGIN
    DROP INDEX [IX_OrderItem_OrderId] ON [dbo].[OrderItem];
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[OrderItem]') AND [c].[name] = N'OrderId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[OrderItem] DROP CONSTRAINT [' + @var1 + '];');
    EXEC(N'UPDATE [dbo].[OrderItem] SET [OrderId] = 0 WHERE [OrderId] IS NULL');
    ALTER TABLE [dbo].[OrderItem] ALTER COLUMN [OrderId] int NOT NULL;
    ALTER TABLE [dbo].[OrderItem] ADD DEFAULT 0 FOR [OrderId];
    DECLARE @description AS sql_variant;
    SET @description = N'FK to Order that the OrderItem belongs to';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'OrderItem', 'COLUMN', N'OrderId';
    CREATE INDEX [IX_OrderItem_OrderId] ON [dbo].[OrderItem] ([OrderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240424222421_ReviewWithTroy'
)
BEGIN
    DROP INDEX [IX_Order_CustomerId] ON [dbo].[Order];
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Order]') AND [c].[name] = N'CustomerId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Order] DROP CONSTRAINT [' + @var2 + '];');
    EXEC(N'UPDATE [dbo].[Order] SET [CustomerId] = 0 WHERE [CustomerId] IS NULL');
    ALTER TABLE [dbo].[Order] ALTER COLUMN [CustomerId] int NOT NULL;
    ALTER TABLE [dbo].[Order] ADD DEFAULT 0 FOR [CustomerId];
    CREATE INDEX [IX_Order_CustomerId] ON [dbo].[Order] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240424222421_ReviewWithTroy'
)
BEGIN
    DROP INDEX [IX_Order_AddressId] ON [dbo].[Order];
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[dbo].[Order]') AND [c].[name] = N'AddressId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Order] DROP CONSTRAINT [' + @var3 + '];');
    EXEC(N'UPDATE [dbo].[Order] SET [AddressId] = 0 WHERE [AddressId] IS NULL');
    ALTER TABLE [dbo].[Order] ALTER COLUMN [AddressId] int NOT NULL;
    ALTER TABLE [dbo].[Order] ADD DEFAULT 0 FOR [AddressId];
    CREATE INDEX [IX_Order_AddressId] ON [dbo].[Order] ([AddressId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240424222421_ReviewWithTroy'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240424222421_ReviewWithTroy', N'8.0.4');
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
