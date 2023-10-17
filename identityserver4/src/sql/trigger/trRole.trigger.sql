DROP TRIGGER IF EXISTS auth.trRole
GO

---
-- Trigger for Role that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trRole
	ON [auth].[Role]
	FOR UPDATE, DELETE
	AS
		BEGIN
	SET NOCOUNT ON

	DECLARE 
		@AuditLogTransactionId	int,
		@Inserted	    		int = 0,
 		@ROWS_COUNT				int

	SELECT @ROWS_COUNT=count(*) from inserted

    -- Check if this is an INSERT, UPDATE or DELETE Action.
    DECLARE @action as varchar(10);
    SET @action = 'INSERT';
    IF EXISTS(SELECT 1 FROM DELETED)
    BEGIN
        SET @action = 
            CASE
                WHEN EXISTS(SELECT 1 FROM INSERTED) THEN 'UPDATE'
                ELSE 'DELETE'
            END
        SELECT @ROWS_COUNT=count(*) from deleted
    END

	-- determine username
	DECLARE @UserName nvarchar(200);
	SELECT TOP 1 @UserName=[LastModifiedUserId] FROM inserted;

	-- insert parent transaction
	INSERT INTO audit.AuditLogTransaction (TableName, TableSchema, Action, HostName, ApplicationName, AuditLogin, AuditDate, AffectedRows, DatabaseName, UserId, TransactionId)
	values('Role', 'auth', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

	-- [RoleId]
	IF UPDATE([RoleId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[RoleId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[RoleId], NEW.[RoleId]), 0), '[[RoleId]] Is Null')),
				'[RoleId]',
				CONVERT(nvarchar(4000), OLD.[RoleId], 126),
				CONVERT(nvarchar(4000), NEW.[RoleId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[RoleId], NEW.[RoleId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[RoleId] = OLD.[RoleId] or (NEW.[RoleId] Is Null and OLD.[RoleId] Is Null))
			WHERE ((NEW.[RoleId] <> OLD.[RoleId]) 
					Or (NEW.[RoleId] Is Null And OLD.[RoleId] Is Not Null)
					Or (NEW.[RoleId] Is Not Null And OLD.[RoleId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Name]
	IF UPDATE([Name]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[RoleId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[RoleId], NEW.[RoleId]), 0), '[[RoleId]] Is Null')),
				'[Name]',
				CONVERT(nvarchar(4000), OLD.[Name], 126),
				CONVERT(nvarchar(4000), NEW.[Name], 126),
				convert(nvarchar(4000), COALESCE(OLD.[RoleId], NEW.[RoleId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[RoleId] = OLD.[RoleId] or (NEW.[RoleId] Is Null and OLD.[RoleId] Is Null))
			WHERE ((NEW.[Name] <> OLD.[Name]) 
					Or (NEW.[Name] Is Null And OLD.[Name] Is Not Null)
					Or (NEW.[Name] Is Not Null And OLD.[Name] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Description]
	IF UPDATE([Description]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[RoleId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[RoleId], NEW.[RoleId]), 0), '[[RoleId]] Is Null')),
				'[Description]',
				CONVERT(nvarchar(4000), OLD.[Description], 126),
				CONVERT(nvarchar(4000), NEW.[Description], 126),
				convert(nvarchar(4000), COALESCE(OLD.[RoleId], NEW.[RoleId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[RoleId] = OLD.[RoleId] or (NEW.[RoleId] Is Null and OLD.[RoleId] Is Null))
			WHERE ((NEW.[Description] <> OLD.[Description]) 
					Or (NEW.[Description] Is Null And OLD.[Description] Is Not Null)
					Or (NEW.[Description] Is Not Null And OLD.[Description] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreateUserId]
	IF UPDATE([CreateUserId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[RoleId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[RoleId], NEW.[RoleId]), 0), '[[RoleId]] Is Null')),
				'[CreateUserId]',
				CONVERT(nvarchar(4000), OLD.[CreateUserId], 126),
				CONVERT(nvarchar(4000), NEW.[CreateUserId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[RoleId], NEW.[RoleId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[RoleId] = OLD.[RoleId] or (NEW.[RoleId] Is Null and OLD.[RoleId] Is Null))
			WHERE ((NEW.[CreateUserId] <> OLD.[CreateUserId]) 
					Or (NEW.[CreateUserId] Is Null And OLD.[CreateUserId] Is Not Null)
					Or (NEW.[CreateUserId] Is Not Null And OLD.[CreateUserId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreateDate]
	IF UPDATE([CreateDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[RoleId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[RoleId], NEW.[RoleId]), 0), '[[RoleId]] Is Null')),
				'[CreateDate]',
				CONVERT(nvarchar(4000), OLD.[CreateDate], 126),
				CONVERT(nvarchar(4000), NEW.[CreateDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[RoleId], NEW.[RoleId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[RoleId] = OLD.[RoleId] or (NEW.[RoleId] Is Null and OLD.[RoleId] Is Null))
			WHERE ((NEW.[CreateDate] <> OLD.[CreateDate]) 
					Or (NEW.[CreateDate] Is Null And OLD.[CreateDate] Is Not Null)
					Or (NEW.[CreateDate] Is Not Null And OLD.[CreateDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LastModifiedUserId]
	IF UPDATE([LastModifiedUserId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[RoleId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[RoleId], NEW.[RoleId]), 0), '[[RoleId]] Is Null')),
				'[LastModifiedUserId]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedUserId], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedUserId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[RoleId], NEW.[RoleId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[RoleId] = OLD.[RoleId] or (NEW.[RoleId] Is Null and OLD.[RoleId] Is Null))
			WHERE ((NEW.[LastModifiedUserId] <> OLD.[LastModifiedUserId]) 
					Or (NEW.[LastModifiedUserId] Is Null And OLD.[LastModifiedUserId] Is Not Null)
					Or (NEW.[LastModifiedUserId] Is Not Null And OLD.[LastModifiedUserId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LastModifiedDate]
	IF UPDATE([LastModifiedDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[RoleId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[RoleId], NEW.[RoleId]), 0), '[[RoleId]] Is Null')),
				'[LastModifiedDate]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedDate], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[RoleId], NEW.[RoleId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[RoleId] = OLD.[RoleId] or (NEW.[RoleId] Is Null and OLD.[RoleId] Is Null))
			WHERE ((NEW.[LastModifiedDate] <> OLD.[LastModifiedDate]) 
					Or (NEW.[LastModifiedDate] Is Null And OLD.[LastModifiedDate] Is Not Null)
					Or (NEW.[LastModifiedDate] Is Not Null And OLD.[LastModifiedDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
