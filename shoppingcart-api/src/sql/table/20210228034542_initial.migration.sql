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


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210228034542_initial')
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


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210228034542_initial')
BEGIN
    CREATE TABLE [dbo].[Widget] (
        [WidgetId] int NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreateSubjectId] uniqueidentifier NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [LastModifiedSubjectId] uniqueidentifier NOT NULL,
        [Text] nvarchar(100) NULL,
        [Width] int NOT NULL,
        [Height] int NOT NULL,
        CONSTRAINT [PK_Widget] PRIMARY KEY ([WidgetId]),
        CONSTRAINT [FK_Widget_Subject_CreateSubjectId] FOREIGN KEY ([CreateSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId]),
        CONSTRAINT [FK_Widget_Subject_LastModifiedSubjectId] FOREIGN KEY ([LastModifiedSubjectId]) REFERENCES [dbo].[Subject] ([SubjectId])
    );
END;


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210228034542_initial')
BEGIN
    CREATE INDEX [IX_Widget_CreateSubjectId] ON [dbo].[Widget] ([CreateSubjectId]);
END;


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210228034542_initial')
BEGIN
    CREATE INDEX [IX_Widget_LastModifiedSubjectId] ON [dbo].[Widget] ([LastModifiedSubjectId]);
END;


IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210228034542_initial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210228034542_initial', N'3.1.12');
END;


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
