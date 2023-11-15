IF COL_LENGTH('Outbox', 'OutboxId') IS NULL
BEGIN
ALTER TABLE [Outbox] DROP CONSTRAINT [PK_Outbox];
ALTER SCHEMA [dbo] TRANSFER [Outbox];
ALTER TABLE [dbo].[Outbox] ADD [OutboxId] int NOT NULL IDENTITY;
ALTER TABLE [dbo].[Outbox] ADD [LastModifiedDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
ALTER TABLE [dbo].[Outbox] ADD CONSTRAINT [PK_Outbox] PRIMARY KEY ([OutboxId]);
CREATE UNIQUE INDEX [IX_Outbox_MessageId] ON [dbo].[Outbox] ([MessageId]);
END
