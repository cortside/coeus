DROP TRIGGER IF EXISTS trOrder
GO

---
-- Trigger for Order that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trOrder
	ON [dbo].[Order]
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
	values('Order', 'dbo', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()
	
	-- [OrderId]
	IF UPDATE([OrderId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderId], NEW.[OrderId]), 0), '[[OrderId]] Is Null')),
				'[OrderId]',
				CONVERT(nvarchar(4000), OLD.[OrderId], 126),
				CONVERT(nvarchar(4000), NEW.[OrderId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderId], NEW.[OrderId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderId] = OLD.[OrderId] or (NEW.[OrderId] Is Null and OLD.[OrderId] Is Null))
			WHERE ((NEW.[OrderId] <> OLD.[OrderId]) 
					Or (NEW.[OrderId] Is Null And OLD.[OrderId] Is Not Null)
					Or (NEW.[OrderId] Is Not Null And OLD.[OrderId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [OrderResourceId]
	IF UPDATE([OrderResourceId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderId], NEW.[OrderId]), 0), '[[OrderId]] Is Null')),
				'[OrderResourceId]',
				CONVERT(nvarchar(4000), OLD.[OrderResourceId], 126),
				CONVERT(nvarchar(4000), NEW.[OrderResourceId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderId], NEW.[OrderId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderId] = OLD.[OrderId] or (NEW.[OrderId] Is Null and OLD.[OrderId] Is Null))
			WHERE ((NEW.[OrderResourceId] <> OLD.[OrderResourceId]) 
					Or (NEW.[OrderResourceId] Is Null And OLD.[OrderResourceId] Is Not Null)
					Or (NEW.[OrderResourceId] Is Not Null And OLD.[OrderResourceId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Status]
	IF UPDATE([Status]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderId], NEW.[OrderId]), 0), '[[OrderId]] Is Null')),
				'[Status]',
				CONVERT(nvarchar(4000), OLD.[Status], 126),
				CONVERT(nvarchar(4000), NEW.[Status], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderId], NEW.[OrderId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderId] = OLD.[OrderId] or (NEW.[OrderId] Is Null and OLD.[OrderId] Is Null))
			WHERE ((NEW.[Status] <> OLD.[Status]) 
					Or (NEW.[Status] Is Null And OLD.[Status] Is Not Null)
					Or (NEW.[Status] Is Not Null And OLD.[Status] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CustomerId]
	IF UPDATE([CustomerId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderId], NEW.[OrderId]), 0), '[[OrderId]] Is Null')),
				'[CustomerId]',
				CONVERT(nvarchar(4000), OLD.[CustomerId], 126),
				CONVERT(nvarchar(4000), NEW.[CustomerId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderId], NEW.[OrderId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderId] = OLD.[OrderId] or (NEW.[OrderId] Is Null and OLD.[OrderId] Is Null))
			WHERE ((NEW.[CustomerId] <> OLD.[CustomerId]) 
					Or (NEW.[CustomerId] Is Null And OLD.[CustomerId] Is Not Null)
					Or (NEW.[CustomerId] Is Not Null And OLD.[CustomerId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AddressId]
	IF UPDATE([AddressId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderId], NEW.[OrderId]), 0), '[[OrderId]] Is Null')),
				'[AddressId]',
				CONVERT(nvarchar(4000), OLD.[AddressId], 126),
				CONVERT(nvarchar(4000), NEW.[AddressId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderId], NEW.[OrderId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderId] = OLD.[OrderId] or (NEW.[OrderId] Is Null and OLD.[OrderId] Is Null))
			WHERE ((NEW.[AddressId] <> OLD.[AddressId]) 
					Or (NEW.[AddressId] Is Null And OLD.[AddressId] Is Not Null)
					Or (NEW.[AddressId] Is Not Null And OLD.[AddressId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreatedDate]
	IF UPDATE([CreatedDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderId], NEW.[OrderId]), 0), '[[OrderId]] Is Null')),
				'[CreatedDate]',
				CONVERT(nvarchar(4000), OLD.[CreatedDate], 126),
				CONVERT(nvarchar(4000), NEW.[CreatedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderId], NEW.[OrderId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderId] = OLD.[OrderId] or (NEW.[OrderId] Is Null and OLD.[OrderId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[OrderId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderId], NEW.[OrderId]), 0), '[[OrderId]] Is Null')),
				'[CreateSubjectId]',
				CONVERT(nvarchar(4000), OLD.[CreateSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[CreateSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderId], NEW.[OrderId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderId] = OLD.[OrderId] or (NEW.[OrderId] Is Null and OLD.[OrderId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[OrderId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderId], NEW.[OrderId]), 0), '[[OrderId]] Is Null')),
				'[LastModifiedDate]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedDate], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderId], NEW.[OrderId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderId] = OLD.[OrderId] or (NEW.[OrderId] Is Null and OLD.[OrderId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[OrderId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderId], NEW.[OrderId]), 0), '[[OrderId]] Is Null')),
				'[LastModifiedSubjectId]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderId], NEW.[OrderId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderId] = OLD.[OrderId] or (NEW.[OrderId] Is Null and OLD.[OrderId] Is Null))
			WHERE ((NEW.[LastModifiedSubjectId] <> OLD.[LastModifiedSubjectId]) 
					Or (NEW.[LastModifiedSubjectId] Is Null And OLD.[LastModifiedSubjectId] Is Not Null)
					Or (NEW.[LastModifiedSubjectId] Is Not Null And OLD.[LastModifiedSubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
