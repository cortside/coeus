DROP TRIGGER IF EXISTS trOrderItem
GO

---
-- Trigger for OrderItem that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trOrderItem
	ON [dbo].[OrderItem]
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
	values('OrderItem', 'dbo', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

		-- [OrderItemId]
	IF UPDATE([OrderItemId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderItemId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderItemId], NEW.[OrderItemId]), 0), '[[OrderItemId]] Is Null')),
				'[OrderItemId]',
				CONVERT(nvarchar(4000), OLD.[OrderItemId], 126),
				CONVERT(nvarchar(4000), NEW.[OrderItemId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderItemId], NEW.[OrderItemId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderItemId] = OLD.[OrderItemId] or (NEW.[OrderItemId] Is Null and OLD.[OrderItemId] Is Null))
			WHERE ((NEW.[OrderItemId] <> OLD.[OrderItemId]) 
					Or (NEW.[OrderItemId] Is Null And OLD.[OrderItemId] Is Not Null)
					Or (NEW.[OrderItemId] Is Not Null And OLD.[OrderItemId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Sku]
	IF UPDATE([Sku]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderItemId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderItemId], NEW.[OrderItemId]), 0), '[[OrderItemId]] Is Null')),
				'[Sku]',
				CONVERT(nvarchar(4000), OLD.[Sku], 126),
				CONVERT(nvarchar(4000), NEW.[Sku], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderItemId], NEW.[OrderItemId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderItemId] = OLD.[OrderItemId] or (NEW.[OrderItemId] Is Null and OLD.[OrderItemId] Is Null))
			WHERE ((NEW.[Sku] <> OLD.[Sku]) 
					Or (NEW.[Sku] Is Null And OLD.[Sku] Is Not Null)
					Or (NEW.[Sku] Is Not Null And OLD.[Sku] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Quantity]
	IF UPDATE([Quantity]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderItemId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderItemId], NEW.[OrderItemId]), 0), '[[OrderItemId]] Is Null')),
				'[Quantity]',
				CONVERT(nvarchar(4000), OLD.[Quantity], 126),
				CONVERT(nvarchar(4000), NEW.[Quantity], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderItemId], NEW.[OrderItemId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderItemId] = OLD.[OrderItemId] or (NEW.[OrderItemId] Is Null and OLD.[OrderItemId] Is Null))
			WHERE ((NEW.[Quantity] <> OLD.[Quantity]) 
					Or (NEW.[Quantity] Is Null And OLD.[Quantity] Is Not Null)
					Or (NEW.[Quantity] Is Not Null And OLD.[Quantity] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [UnitPrice]
	IF UPDATE([UnitPrice]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderItemId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderItemId], NEW.[OrderItemId]), 0), '[[OrderItemId]] Is Null')),
				'[UnitPrice]',
				CONVERT(nvarchar(4000), OLD.[UnitPrice], 126),
				CONVERT(nvarchar(4000), NEW.[UnitPrice], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderItemId], NEW.[OrderItemId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderItemId] = OLD.[OrderItemId] or (NEW.[OrderItemId] Is Null and OLD.[OrderItemId] Is Null))
			WHERE ((NEW.[UnitPrice] <> OLD.[UnitPrice]) 
					Or (NEW.[UnitPrice] Is Null And OLD.[UnitPrice] Is Not Null)
					Or (NEW.[UnitPrice] Is Not Null And OLD.[UnitPrice] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [OrderId]
	IF UPDATE([OrderId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderItemId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderItemId], NEW.[OrderItemId]), 0), '[[OrderItemId]] Is Null')),
				'[OrderId]',
				CONVERT(nvarchar(4000), OLD.[OrderId], 126),
				CONVERT(nvarchar(4000), NEW.[OrderId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderItemId], NEW.[OrderItemId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderItemId] = OLD.[OrderItemId] or (NEW.[OrderItemId] Is Null and OLD.[OrderItemId] Is Null))
			WHERE ((NEW.[OrderId] <> OLD.[OrderId]) 
					Or (NEW.[OrderId] Is Null And OLD.[OrderId] Is Not Null)
					Or (NEW.[OrderId] Is Not Null And OLD.[OrderId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreatedDate]
	IF UPDATE([CreatedDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[OrderItemId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderItemId], NEW.[OrderItemId]), 0), '[[OrderItemId]] Is Null')),
				'[CreatedDate]',
				CONVERT(nvarchar(4000), OLD.[CreatedDate], 126),
				CONVERT(nvarchar(4000), NEW.[CreatedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderItemId], NEW.[OrderItemId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderItemId] = OLD.[OrderItemId] or (NEW.[OrderItemId] Is Null and OLD.[OrderItemId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[OrderItemId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderItemId], NEW.[OrderItemId]), 0), '[[OrderItemId]] Is Null')),
				'[CreateSubjectId]',
				CONVERT(nvarchar(4000), OLD.[CreateSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[CreateSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderItemId], NEW.[OrderItemId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderItemId] = OLD.[OrderItemId] or (NEW.[OrderItemId] Is Null and OLD.[OrderItemId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[OrderItemId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderItemId], NEW.[OrderItemId]), 0), '[[OrderItemId]] Is Null')),
				'[LastModifiedDate]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedDate], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderItemId], NEW.[OrderItemId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderItemId] = OLD.[OrderItemId] or (NEW.[OrderItemId] Is Null and OLD.[OrderItemId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[OrderItemId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[OrderItemId], NEW.[OrderItemId]), 0), '[[OrderItemId]] Is Null')),
				'[LastModifiedSubjectId]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[OrderItemId], NEW.[OrderItemId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[OrderItemId] = OLD.[OrderItemId] or (NEW.[OrderItemId] Is Null and OLD.[OrderItemId] Is Null))
			WHERE ((NEW.[LastModifiedSubjectId] <> OLD.[LastModifiedSubjectId]) 
					Or (NEW.[LastModifiedSubjectId] Is Null And OLD.[LastModifiedSubjectId] Is Not Null)
					Or (NEW.[LastModifiedSubjectId] Is Not Null And OLD.[LastModifiedSubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END



	--IF @Inserted = 0
	--	BEGIN
	--	    -- believed to be contributing to deadlocks
	--		-- DELETE FROM audit.AuditLogTransaction WHERE AuditLogTransactionId = @AuditLogTransactionId
	--	END
END
GO
