PRINT 'Before TRY'
BEGIN TRY
	BEGIN TRAN
	PRINT 'First Statement in the TRY block'
BEGIN TRANSACTION;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    ALTER TABLE [dbo].[Address] DROP CONSTRAINT [FK_Address_Subject_CreateSubjectId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    ALTER TABLE [dbo].[Customer] DROP CONSTRAINT [FK_Customer_Subject_CreateSubjectId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    ALTER TABLE [dbo].[Order] DROP CONSTRAINT [FK_Order_Subject_CreateSubjectId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    ALTER TABLE [dbo].[OrderItem] DROP CONSTRAINT [FK_OrderItem_Subject_CreateSubjectId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    EXEC sp_rename N'[dbo].[OrderItem].[CreateSubjectId]', N'CreatedSubjectId', N'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    EXEC sp_rename N'[dbo].[OrderItem].[IX_OrderItem_CreateSubjectId]', N'IX_OrderItem_CreatedSubjectId', N'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    EXEC sp_rename N'[dbo].[Order].[CreateSubjectId]', N'CreatedSubjectId', N'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    EXEC sp_rename N'[dbo].[Order].[IX_Order_CreateSubjectId]', N'IX_Order_CreatedSubjectId', N'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    EXEC sp_rename N'[dbo].[Customer].[CreateSubjectId]', N'CreatedSubjectId', N'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    EXEC sp_rename N'[dbo].[Customer].[IX_Customer_CreateSubjectId]', N'IX_Customer_CreatedSubjectId', N'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    EXEC sp_rename N'[dbo].[Address].[CreateSubjectId]', N'CreatedSubjectId', N'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    EXEC sp_rename N'[dbo].[Address].[IX_Address_CreateSubjectId]', N'IX_Address_CreatedSubjectId', N'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    ALTER TABLE [dbo].[Outbox] ADD [PublishCount] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    ALTER TABLE [dbo].[Address] ADD CONSTRAINT [FK_Address_Subject_CreatedSubjectId] FOREIGN KEY ([CreatedSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    ALTER TABLE [dbo].[Customer] ADD CONSTRAINT [FK_Customer_Subject_CreatedSubjectId] FOREIGN KEY ([CreatedSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_Order_Subject_CreatedSubjectId] FOREIGN KEY ([CreatedSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    ALTER TABLE [dbo].[OrderItem] ADD CONSTRAINT [FK_OrderItem_Subject_CreatedSubjectId] FOREIGN KEY ([CreatedSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241105173555_PublishCount'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241105173555_PublishCount', N'8.0.8');
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
