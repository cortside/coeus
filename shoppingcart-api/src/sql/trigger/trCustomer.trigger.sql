DROP TRIGGER IF EXISTS trCustomer
GO

---
-- Trigger for Customer that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trCustomer
	ON [dbo].[Customer]
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
	values('Customer', 'dbo', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

	-- [CustomerId]
	IF UPDATE([CustomerId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[CustomerId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerId], NEW.[CustomerId]), 0), '[[CustomerId]] Is Null')),
				'[CustomerId]',
				CONVERT(nvarchar(4000), OLD.[CustomerId], 126),
				CONVERT(nvarchar(4000), NEW.[CustomerId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerId], NEW.[CustomerId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerId] = OLD.[CustomerId] or (NEW.[CustomerId] Is Null and OLD.[CustomerId] Is Null))
			WHERE ((NEW.[CustomerId] <> OLD.[CustomerId]) 
					Or (NEW.[CustomerId] Is Null And OLD.[CustomerId] Is Not Null)
					Or (NEW.[CustomerId] Is Not Null And OLD.[CustomerId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CustomerResourceId]
	IF UPDATE([CustomerResourceId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[CustomerId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerId], NEW.[CustomerId]), 0), '[[CustomerId]] Is Null')),
				'[CustomerResourceId]',
				CONVERT(nvarchar(4000), OLD.[CustomerResourceId], 126),
				CONVERT(nvarchar(4000), NEW.[CustomerResourceId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerId], NEW.[CustomerId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerId] = OLD.[CustomerId] or (NEW.[CustomerId] Is Null and OLD.[CustomerId] Is Null))
			WHERE ((NEW.[CustomerResourceId] <> OLD.[CustomerResourceId]) 
					Or (NEW.[CustomerResourceId] Is Null And OLD.[CustomerResourceId] Is Not Null)
					Or (NEW.[CustomerResourceId] Is Not Null And OLD.[CustomerResourceId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [FirstName]
	IF UPDATE([FirstName]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[CustomerId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerId], NEW.[CustomerId]), 0), '[[CustomerId]] Is Null')),
				'[FirstName]',
				CONVERT(nvarchar(4000), OLD.[FirstName], 126),
				CONVERT(nvarchar(4000), NEW.[FirstName], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerId], NEW.[CustomerId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerId] = OLD.[CustomerId] or (NEW.[CustomerId] Is Null and OLD.[CustomerId] Is Null))
			WHERE ((NEW.[FirstName] <> OLD.[FirstName]) 
					Or (NEW.[FirstName] Is Null And OLD.[FirstName] Is Not Null)
					Or (NEW.[FirstName] Is Not Null And OLD.[FirstName] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LastName]
	IF UPDATE([LastName]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[CustomerId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerId], NEW.[CustomerId]), 0), '[[CustomerId]] Is Null')),
				'[LastName]',
				CONVERT(nvarchar(4000), OLD.[LastName], 126),
				CONVERT(nvarchar(4000), NEW.[LastName], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerId], NEW.[CustomerId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerId] = OLD.[CustomerId] or (NEW.[CustomerId] Is Null and OLD.[CustomerId] Is Null))
			WHERE ((NEW.[LastName] <> OLD.[LastName]) 
					Or (NEW.[LastName] Is Null And OLD.[LastName] Is Not Null)
					Or (NEW.[LastName] Is Not Null And OLD.[LastName] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Email]
	IF UPDATE([Email]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[CustomerId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerId], NEW.[CustomerId]), 0), '[[CustomerId]] Is Null')),
				'[Email]',
				CONVERT(nvarchar(4000), OLD.[Email], 126),
				CONVERT(nvarchar(4000), NEW.[Email], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerId], NEW.[CustomerId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerId] = OLD.[CustomerId] or (NEW.[CustomerId] Is Null and OLD.[CustomerId] Is Null))
			WHERE ((NEW.[Email] <> OLD.[Email]) 
					Or (NEW.[Email] Is Null And OLD.[Email] Is Not Null)
					Or (NEW.[Email] Is Not Null And OLD.[Email] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreatedDate]
	IF UPDATE([CreatedDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[CustomerId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerId], NEW.[CustomerId]), 0), '[[CustomerId]] Is Null')),
				'[CreatedDate]',
				CONVERT(nvarchar(4000), OLD.[CreatedDate], 126),
				CONVERT(nvarchar(4000), NEW.[CreatedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerId], NEW.[CustomerId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerId] = OLD.[CustomerId] or (NEW.[CustomerId] Is Null and OLD.[CustomerId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[CustomerId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerId], NEW.[CustomerId]), 0), '[[CustomerId]] Is Null')),
				'[CreateSubjectId]',
				CONVERT(nvarchar(4000), OLD.[CreateSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[CreateSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerId], NEW.[CustomerId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerId] = OLD.[CustomerId] or (NEW.[CustomerId] Is Null and OLD.[CustomerId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[CustomerId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerId], NEW.[CustomerId]), 0), '[[CustomerId]] Is Null')),
				'[LastModifiedDate]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedDate], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerId], NEW.[CustomerId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerId] = OLD.[CustomerId] or (NEW.[CustomerId] Is Null and OLD.[CustomerId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[CustomerId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[CustomerId], NEW.[CustomerId]), 0), '[[CustomerId]] Is Null')),
				'[LastModifiedSubjectId]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[CustomerId], NEW.[CustomerId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[CustomerId] = OLD.[CustomerId] or (NEW.[CustomerId] Is Null and OLD.[CustomerId] Is Null))
			WHERE ((NEW.[LastModifiedSubjectId] <> OLD.[LastModifiedSubjectId]) 
					Or (NEW.[LastModifiedSubjectId] Is Null And OLD.[LastModifiedSubjectId] Is Not Null)
					Or (NEW.[LastModifiedSubjectId] Is Not Null And OLD.[LastModifiedSubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
