PRINT 'Before TRY'
BEGIN TRY
	BEGIN TRAN
	PRINT 'First Statement in the TRY block'
BEGIN TRANSACTION;

DECLARE @description AS sql_variant;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Username (upn claim)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Subject', 'COLUMN', N'UserPrincipalName';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Subject primary key';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Subject', 'COLUMN', N'Name';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Subject primary key';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Subject', 'COLUMN', N'GivenName';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Subject Surname ()';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Subject', 'COLUMN', N'FamilyName';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Date and time entity was created';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Subject', 'COLUMN', N'CreatedDate';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Subject primary key';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Subject', 'COLUMN', N'SubjectId';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Date and time entity was last modified';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'OrderItem', 'COLUMN', N'LastModifiedDate';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Date and time entity was created';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'OrderItem', 'COLUMN', N'CreatedDate';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Date and time entity was last modified';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Order', 'COLUMN', N'LastModifiedDate';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Date and time entity was created';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Order', 'COLUMN', N'CreatedDate';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    ALTER TABLE [dbo].[Order] ADD [LastNotified] datetime2 NULL;
    SET @description = N'Date customer was last notified for order';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Order', 'COLUMN', N'LastNotified';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Date and time entity was last modified';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Customer', 'COLUMN', N'LastModifiedDate';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Date and time entity was created';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Customer', 'COLUMN', N'CreatedDate';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Date and time entity was last modified';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Address', 'COLUMN', N'LastModifiedDate';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    SET @description = N'Date and time entity was created';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'Address', 'COLUMN', N'CreatedDate';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230531231521_AddOrderLastNotified')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230531231521_AddOrderLastNotified', N'6.0.16');
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
