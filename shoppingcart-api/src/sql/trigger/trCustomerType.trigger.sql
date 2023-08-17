DROP TRIGGER IF EXISTS dbo.trCustomerType
GO

---
-- Trigger for CustomerType that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trCustomerType
	ON [dbo].[CustomerType]
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
	SELECT TOP 1 @UserName=[LastModifiedSubjectId] FROM inserted;

	-- insert parent transaction
	INSERT INTO audit.AuditLogTransaction (TableName, TableSchema, Action, HostName, ApplicationName, AuditLogin, AuditDate, AffectedRows, DatabaseName, UserId, TransactionId)
	values('CustomerType', 'dbo', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

	-- [CustomerTypeId]
	IF UPDATE([CustomerTypeId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[CustomerTypeId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerTypeId], NEW.[CustomerTypeId]), 0), '[[CustomerTypeId]] Is Null')),
				'[CustomerTypeId]',
				CONVERT(nvarchar(4000), OLD.[CustomerTypeId], 126),
				CONVERT(nvarchar(4000), NEW.[CustomerTypeId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerTypeId], NEW.[CustomerTypeId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerTypeId] = OLD.[CustomerTypeId] or (NEW.[CustomerTypeId] Is Null and OLD.[CustomerTypeId] Is Null))
			WHERE ((NEW.[CustomerTypeId] <> OLD.[CustomerTypeId]) 
					Or (NEW.[CustomerTypeId] Is Null And OLD.[CustomerTypeId] Is Not Null)
					Or (NEW.[CustomerTypeId] Is Not Null And OLD.[CustomerTypeId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Name]
	IF UPDATE([Name]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[CustomerTypeId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerTypeId], NEW.[CustomerTypeId]), 0), '[[CustomerTypeId]] Is Null')),
				'[Name]',
				CONVERT(nvarchar(4000), OLD.[Name], 126),
				CONVERT(nvarchar(4000), NEW.[Name], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerTypeId], NEW.[CustomerTypeId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerTypeId] = OLD.[CustomerTypeId] or (NEW.[CustomerTypeId] Is Null and OLD.[CustomerTypeId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[CustomerTypeId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerTypeId], NEW.[CustomerTypeId]), 0), '[[CustomerTypeId]] Is Null')),
				'[Description]',
				CONVERT(nvarchar(4000), OLD.[Description], 126),
				CONVERT(nvarchar(4000), NEW.[Description], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerTypeId], NEW.[CustomerTypeId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerTypeId] = OLD.[CustomerTypeId] or (NEW.[CustomerTypeId] Is Null and OLD.[CustomerTypeId] Is Null))
			WHERE ((NEW.[Description] <> OLD.[Description]) 
					Or (NEW.[Description] Is Null And OLD.[Description] Is Not Null)
					Or (NEW.[Description] Is Not Null And OLD.[Description] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [TaxExempt]
	IF UPDATE([TaxExempt]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[CustomerTypeId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerTypeId], NEW.[CustomerTypeId]), 0), '[[CustomerTypeId]] Is Null')),
				'[TaxExempt]',
				CONVERT(nvarchar(4000), OLD.[TaxExempt], 126),
				CONVERT(nvarchar(4000), NEW.[TaxExempt], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerTypeId], NEW.[CustomerTypeId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerTypeId] = OLD.[CustomerTypeId] or (NEW.[CustomerTypeId] Is Null and OLD.[CustomerTypeId] Is Null))
			WHERE ((NEW.[TaxExempt] <> OLD.[TaxExempt]) 
					Or (NEW.[TaxExempt] Is Null And OLD.[TaxExempt] Is Not Null)
					Or (NEW.[TaxExempt] Is Not Null And OLD.[TaxExempt] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreatedDate]
	IF UPDATE([CreatedDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[CustomerTypeId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerTypeId], NEW.[CustomerTypeId]), 0), '[[CustomerTypeId]] Is Null')),
				'[CreatedDate]',
				CONVERT(nvarchar(4000), OLD.[CreatedDate], 126),
				CONVERT(nvarchar(4000), NEW.[CreatedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerTypeId], NEW.[CustomerTypeId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerTypeId] = OLD.[CustomerTypeId] or (NEW.[CustomerTypeId] Is Null and OLD.[CustomerTypeId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[CustomerTypeId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerTypeId], NEW.[CustomerTypeId]), 0), '[[CustomerTypeId]] Is Null')),
				'[CreateSubjectId]',
				CONVERT(nvarchar(4000), OLD.[CreateSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[CreateSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerTypeId], NEW.[CustomerTypeId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerTypeId] = OLD.[CustomerTypeId] or (NEW.[CustomerTypeId] Is Null and OLD.[CustomerTypeId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[CustomerTypeId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerTypeId], NEW.[CustomerTypeId]), 0), '[[CustomerTypeId]] Is Null')),
				'[LastModifiedDate]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedDate], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerTypeId], NEW.[CustomerTypeId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerTypeId] = OLD.[CustomerTypeId] or (NEW.[CustomerTypeId] Is Null and OLD.[CustomerTypeId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[CustomerTypeId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerTypeId], NEW.[CustomerTypeId]), 0), '[[CustomerTypeId]] Is Null')),
				'[LastModifiedSubjectId]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerTypeId], NEW.[CustomerTypeId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerTypeId] = OLD.[CustomerTypeId] or (NEW.[CustomerTypeId] Is Null and OLD.[CustomerTypeId] Is Null))
			WHERE ((NEW.[LastModifiedSubjectId] <> OLD.[LastModifiedSubjectId]) 
					Or (NEW.[LastModifiedSubjectId] Is Null And OLD.[LastModifiedSubjectId] Is Not Null)
					Or (NEW.[LastModifiedSubjectId] Is Not Null And OLD.[LastModifiedSubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
