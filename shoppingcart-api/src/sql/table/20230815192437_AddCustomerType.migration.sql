PRINT 'Before TRY'
BEGIN TRY
	BEGIN TRAN
	PRINT 'First Statement in the TRY block'
BEGIN TRANSACTION;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815192437_AddCustomerType')
BEGIN
    ALTER TABLE [dbo].[Customer] ADD [CustomerTypeId] int NULL;
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815192437_AddCustomerType')
BEGIN
    CREATE TABLE [dbo].[CustomerType] (
        [CustomerTypeId] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NULL,
        [Description] nvarchar(100) NULL,
        [TaxExempt] bit NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreateSubjectId] uniqueidentifier NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [LastModifiedSubjectId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CustomerType] PRIMARY KEY ([CustomerTypeId]),
        CONSTRAINT [FK_CustomerType_Subject_CreateSubjectId] FOREIGN KEY ([CreateSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId]),
        CONSTRAINT [FK_CustomerType_Subject_LastModifiedSubjectId] FOREIGN KEY ([LastModifiedSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId])
    );
    DECLARE @description AS sql_variant;
    SET @description = N'Customer types';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'CustomerType';
    SET @description = N'Date and time entity was created';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'CustomerType', 'COLUMN', N'CreatedDate';
    SET @description = N'Date and time entity was last modified';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', N'dbo', 'TABLE', N'CustomerType', 'COLUMN', N'LastModifiedDate';
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815192437_AddCustomerType')
BEGIN
    CREATE INDEX [IX_Customer_CustomerTypeId] ON [dbo].[Customer] ([CustomerTypeId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815192437_AddCustomerType')
BEGIN
    CREATE INDEX [IX_CustomerType_CreateSubjectId] ON [dbo].[CustomerType] ([CreateSubjectId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815192437_AddCustomerType')
BEGIN
    CREATE INDEX [IX_CustomerType_LastModifiedSubjectId] ON [dbo].[CustomerType] ([LastModifiedSubjectId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815192437_AddCustomerType')
BEGIN
    ALTER TABLE [dbo].[Customer] ADD CONSTRAINT [FK_Customer_CustomerType_CustomerTypeId] FOREIGN KEY ([CustomerTypeId]) REFERENCES [dbo].[CustomerType] ([CustomerTypeId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230815192437_AddCustomerType')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230815192437_AddCustomerType', N'6.0.20');
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
