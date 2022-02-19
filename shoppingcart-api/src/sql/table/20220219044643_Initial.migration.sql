PRINT 'Before TRY'
BEGIN TRY
	BEGIN TRAN
	PRINT 'First Statement in the TRY block'
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

BEGIN TRANSACTION;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE TABLE [dbo].[Outbox] (
        [OutboxId] int NOT NULL IDENTITY,
        [MessageId] nvarchar(36) NOT NULL,
        [CorrelationId] nvarchar(250) NULL,
        [EventType] nvarchar(250) NOT NULL,
        [Topic] nvarchar(100) NOT NULL,
        [RoutingKey] nvarchar(100) NOT NULL,
        [Body] nvarchar(max) NOT NULL,
        [Status] nvarchar(10) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [ScheduledDate] datetime2 NOT NULL,
        [PublishedDate] datetime2 NULL,
        [LockId] nvarchar(36) NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Outbox] PRIMARY KEY ([OutboxId])
    );
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE TABLE [dbo].[Subject] (
        [SubjectId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NULL,
        [GivenName] nvarchar(100) NULL,
        [FamilyName] nvarchar(100) NULL,
        [UserPrincipalName] nvarchar(100) NULL,
        [CreatedDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Subject] PRIMARY KEY ([SubjectId])
    );
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE TABLE [dbo].[Address] (
        [AddressId] int NOT NULL IDENTITY,
        [Street] nvarchar(max) NULL,
        [City] nvarchar(max) NULL,
        [State] nvarchar(max) NULL,
        [Country] nvarchar(max) NULL,
        [ZipCode] nvarchar(max) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreateSubjectId] uniqueidentifier NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [LastModifiedSubjectId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Address] PRIMARY KEY ([AddressId]),
        CONSTRAINT [FK_Address_Subject_CreateSubjectId] FOREIGN KEY ([CreateSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId]),
        CONSTRAINT [FK_Address_Subject_LastModifiedSubjectId] FOREIGN KEY ([LastModifiedSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId])
    );
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE TABLE [dbo].[Customer] (
        [CustomerId] int NOT NULL IDENTITY,
        [CustomerResourceId] uniqueidentifier NOT NULL,
        [FirstName] nvarchar(50) NULL,
        [LastName] nvarchar(50) NULL,
        [Email] nvarchar(250) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreateSubjectId] uniqueidentifier NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [LastModifiedSubjectId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Customer] PRIMARY KEY ([CustomerId]),
        CONSTRAINT [FK_Customer_Subject_CreateSubjectId] FOREIGN KEY ([CreateSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId]),
        CONSTRAINT [FK_Customer_Subject_LastModifiedSubjectId] FOREIGN KEY ([LastModifiedSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId])
    );
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE TABLE [dbo].[Order] (
        [OrderId] int NOT NULL IDENTITY,
        [OrderResourceId] uniqueidentifier NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [CustomerId] int NULL,
        [AddressId] int NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreateSubjectId] uniqueidentifier NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [LastModifiedSubjectId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Order] PRIMARY KEY ([OrderId]),
        CONSTRAINT [FK_Order_Address_AddressId] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[Address] ([AddressId]),
        CONSTRAINT [FK_Order_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId]),
        CONSTRAINT [FK_Order_Subject_CreateSubjectId] FOREIGN KEY ([CreateSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId]),
        CONSTRAINT [FK_Order_Subject_LastModifiedSubjectId] FOREIGN KEY ([LastModifiedSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId])
    );
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE TABLE [dbo].[OrderItem] (
        [OrderItemId] int NOT NULL IDENTITY,
        [Sku] nvarchar(10) NULL,
        [Quantity] int NOT NULL,
        [UnitPrice] money NOT NULL,
        [OrderId] int NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreateSubjectId] uniqueidentifier NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [LastModifiedSubjectId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_OrderItem] PRIMARY KEY ([OrderItemId]),
        CONSTRAINT [FK_OrderItem_Order_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order] ([OrderId]),
        CONSTRAINT [FK_OrderItem_Subject_CreateSubjectId] FOREIGN KEY ([CreateSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId]),
        CONSTRAINT [FK_OrderItem_Subject_LastModifiedSubjectId] FOREIGN KEY ([LastModifiedSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId])
    );
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_Address_CreateSubjectId] ON [dbo].[Address] ([CreateSubjectId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_Address_LastModifiedSubjectId] ON [dbo].[Address] ([LastModifiedSubjectId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_Customer_CreateSubjectId] ON [dbo].[Customer] ([CreateSubjectId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_Customer_LastModifiedSubjectId] ON [dbo].[Customer] ([LastModifiedSubjectId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_Order_AddressId] ON [dbo].[Order] ([AddressId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_Order_CreateSubjectId] ON [dbo].[Order] ([CreateSubjectId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_Order_CustomerId] ON [dbo].[Order] ([CustomerId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_Order_LastModifiedSubjectId] ON [dbo].[Order] ([LastModifiedSubjectId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_OrderItem_CreateSubjectId] ON [dbo].[OrderItem] ([CreateSubjectId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_OrderItem_LastModifiedSubjectId] ON [dbo].[OrderItem] ([LastModifiedSubjectId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_OrderItem_OrderId] ON [dbo].[OrderItem] ([OrderId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE UNIQUE INDEX [IX_Outbox_MessageId] ON [dbo].[Outbox] ([MessageId]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_ScheduleDate_Status] ON [dbo].[Outbox] ([ScheduledDate], [Status]) INCLUDE ([EventType]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    CREATE INDEX [IX_Status_LockId_ScheduleDate] ON [dbo].[Outbox] ([Status], [LockId], [ScheduledDate]);
END;

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220219044643_Initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220219044643_Initial', N'6.0.2');
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
