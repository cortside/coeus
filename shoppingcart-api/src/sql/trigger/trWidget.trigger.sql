DROP TRIGGER IF EXISTS trWidget
GO

---
-- Trigger for Widget that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trWidget
	ON [dbo].[Widget]
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
    END

	-- determine username
	DECLARE @UserName nvarchar(200);
	SELECT TOP 1 @UserName=[LastModifiedSubjectId] FROM inserted;

	-- insert parent transaction
	INSERT INTO audit.AuditLogTransaction (TableName, TableSchema, Action, HostName, ApplicationName, AuditLogin, AuditDate, AffectedRows, DatabaseName, UserId, TransactionId)
	values('Widget', 'dbo', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

		-- [WidgetId]
	IF UPDATE([WidgetId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[WidgetId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[WidgetId], NEW.[WidgetId]), 0), '[[WidgetId]] Is Null')),
				'[WidgetId]',
				CONVERT(nvarchar(4000), OLD.[WidgetId], 126),
				CONVERT(nvarchar(4000), NEW.[WidgetId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[WidgetId], NEW.[WidgetId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[WidgetId] = OLD.[WidgetId] or (NEW.[WidgetId] Is Null and OLD.[WidgetId] Is Null))
			WHERE ((NEW.[WidgetId] <> OLD.[WidgetId]) 
					Or (NEW.[WidgetId] Is Null And OLD.[WidgetId] Is Not Null)
					Or (NEW.[WidgetId] Is Not Null And OLD.[WidgetId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreatedDate]
	IF UPDATE([CreatedDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[WidgetId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[WidgetId], NEW.[WidgetId]), 0), '[[WidgetId]] Is Null')),
				'[CreatedDate]',
				CONVERT(nvarchar(4000), OLD.[CreatedDate], 126),
				CONVERT(nvarchar(4000), NEW.[CreatedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[WidgetId], NEW.[WidgetId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[WidgetId] = OLD.[WidgetId] or (NEW.[WidgetId] Is Null and OLD.[WidgetId] Is Null))
			WHERE ((NEW.[CreatedDate] <> OLD.[CreatedDate]) 
					Or (NEW.[CreatedDate] Is Null And OLD.[CreatedDate] Is Not Null)
					Or (NEW.[CreatedDate] Is Not Null And OLD.[CreatedDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreateSubjectId]
	IF UPDATE([CreateSubjectId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[WidgetId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[WidgetId], NEW.[WidgetId]), 0), '[[WidgetId]] Is Null')),
				'[CreateSubjectId]',
				CONVERT(nvarchar(4000), OLD.[CreateSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[CreateSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[WidgetId], NEW.[WidgetId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[WidgetId] = OLD.[WidgetId] or (NEW.[WidgetId] Is Null and OLD.[WidgetId] Is Null))
			WHERE ((NEW.[CreateSubjectId] <> OLD.[CreateSubjectId]) 
					Or (NEW.[CreateSubjectId] Is Null And OLD.[CreateSubjectId] Is Not Null)
					Or (NEW.[CreateSubjectId] Is Not Null And OLD.[CreateSubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LastModifiedDate]
	IF UPDATE([LastModifiedDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[WidgetId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[WidgetId], NEW.[WidgetId]), 0), '[[WidgetId]] Is Null')),
				'[LastModifiedDate]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedDate], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[WidgetId], NEW.[WidgetId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[WidgetId] = OLD.[WidgetId] or (NEW.[WidgetId] Is Null and OLD.[WidgetId] Is Null))
			WHERE ((NEW.[LastModifiedDate] <> OLD.[LastModifiedDate]) 
					Or (NEW.[LastModifiedDate] Is Null And OLD.[LastModifiedDate] Is Not Null)
					Or (NEW.[LastModifiedDate] Is Not Null And OLD.[LastModifiedDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LastModifiedSubjectId]
	IF UPDATE([LastModifiedSubjectId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[WidgetId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[WidgetId], NEW.[WidgetId]), 0), '[[WidgetId]] Is Null')),
				'[LastModifiedSubjectId]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[WidgetId], NEW.[WidgetId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[WidgetId] = OLD.[WidgetId] or (NEW.[WidgetId] Is Null and OLD.[WidgetId] Is Null))
			WHERE ((NEW.[LastModifiedSubjectId] <> OLD.[LastModifiedSubjectId]) 
					Or (NEW.[LastModifiedSubjectId] Is Null And OLD.[LastModifiedSubjectId] Is Not Null)
					Or (NEW.[LastModifiedSubjectId] Is Not Null And OLD.[LastModifiedSubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Text]
	IF UPDATE([Text]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[WidgetId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[WidgetId], NEW.[WidgetId]), 0), '[[WidgetId]] Is Null')),
				'[Text]',
				CONVERT(nvarchar(4000), OLD.[Text], 126),
				CONVERT(nvarchar(4000), NEW.[Text], 126),
				convert(nvarchar(4000), COALESCE(OLD.[WidgetId], NEW.[WidgetId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[WidgetId] = OLD.[WidgetId] or (NEW.[WidgetId] Is Null and OLD.[WidgetId] Is Null))
			WHERE ((NEW.[Text] <> OLD.[Text]) 
					Or (NEW.[Text] Is Null And OLD.[Text] Is Not Null)
					Or (NEW.[Text] Is Not Null And OLD.[Text] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Width]
	IF UPDATE([Width]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[WidgetId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[WidgetId], NEW.[WidgetId]), 0), '[[WidgetId]] Is Null')),
				'[Width]',
				CONVERT(nvarchar(4000), OLD.[Width], 126),
				CONVERT(nvarchar(4000), NEW.[Width], 126),
				convert(nvarchar(4000), COALESCE(OLD.[WidgetId], NEW.[WidgetId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[WidgetId] = OLD.[WidgetId] or (NEW.[WidgetId] Is Null and OLD.[WidgetId] Is Null))
			WHERE ((NEW.[Width] <> OLD.[Width]) 
					Or (NEW.[Width] Is Null And OLD.[Width] Is Not Null)
					Or (NEW.[Width] Is Not Null And OLD.[Width] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Height]
	IF UPDATE([Height]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[WidgetId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[WidgetId], NEW.[WidgetId]), 0), '[[WidgetId]] Is Null')),
				'[Height]',
				CONVERT(nvarchar(4000), OLD.[Height], 126),
				CONVERT(nvarchar(4000), NEW.[Height], 126),
				convert(nvarchar(4000), COALESCE(OLD.[WidgetId], NEW.[WidgetId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[WidgetId] = OLD.[WidgetId] or (NEW.[WidgetId] Is Null and OLD.[WidgetId] Is Null))
			WHERE ((NEW.[Height] <> OLD.[Height]) 
					Or (NEW.[Height] Is Null And OLD.[Height] Is Not Null)
					Or (NEW.[Height] Is Not Null And OLD.[Height] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END



	--IF @Inserted = 0
	--	BEGIN
	--	    -- believed to be contributing to deadlocks
	--		-- DELETE FROM audit.AuditLogTransaction WHERE AuditLogTransactionId = @AuditLogTransactionId
	--	END
END
GO
