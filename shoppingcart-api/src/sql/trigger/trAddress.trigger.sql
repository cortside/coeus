DROP TRIGGER IF EXISTS dbo.trAddress
GO

---
-- Trigger for Address that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trAddress
	ON [dbo].[Address]
	FOR UPDATE, DELETE
	AS
		BEGIN
	SET NOCOUNT ON

	DECLARE 
		@AuditLogTransactionId	bigint,
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
	values('Address', 'dbo', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

	-- [AddressId]
	IF UPDATE([AddressId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[AddressId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[AddressId], NEW.[AddressId]), 0), '[[AddressId]] Is Null')),
				'[AddressId]',
				CONVERT(nvarchar(4000), OLD.[AddressId], 126),
				CONVERT(nvarchar(4000), NEW.[AddressId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[AddressId], NEW.[AddressId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[AddressId] = OLD.[AddressId] or (NEW.[AddressId] Is Null and OLD.[AddressId] Is Null))
			WHERE ((NEW.[AddressId] <> OLD.[AddressId]) 
					Or (NEW.[AddressId] Is Null And OLD.[AddressId] Is Not Null)
					Or (NEW.[AddressId] Is Not Null And OLD.[AddressId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Street]
	IF UPDATE([Street]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[AddressId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[AddressId], NEW.[AddressId]), 0), '[[AddressId]] Is Null')),
				'[Street]',
				CONVERT(nvarchar(4000), OLD.[Street], 126),
				CONVERT(nvarchar(4000), NEW.[Street], 126),
				convert(nvarchar(4000), COALESCE(OLD.[AddressId], NEW.[AddressId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[AddressId] = OLD.[AddressId] or (NEW.[AddressId] Is Null and OLD.[AddressId] Is Null))
			WHERE ((NEW.[Street] <> OLD.[Street]) 
					Or (NEW.[Street] Is Null And OLD.[Street] Is Not Null)
					Or (NEW.[Street] Is Not Null And OLD.[Street] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [City]
	IF UPDATE([City]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[AddressId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[AddressId], NEW.[AddressId]), 0), '[[AddressId]] Is Null')),
				'[City]',
				CONVERT(nvarchar(4000), OLD.[City], 126),
				CONVERT(nvarchar(4000), NEW.[City], 126),
				convert(nvarchar(4000), COALESCE(OLD.[AddressId], NEW.[AddressId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[AddressId] = OLD.[AddressId] or (NEW.[AddressId] Is Null and OLD.[AddressId] Is Null))
			WHERE ((NEW.[City] <> OLD.[City]) 
					Or (NEW.[City] Is Null And OLD.[City] Is Not Null)
					Or (NEW.[City] Is Not Null And OLD.[City] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [State]
	IF UPDATE([State]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[AddressId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[AddressId], NEW.[AddressId]), 0), '[[AddressId]] Is Null')),
				'[State]',
				CONVERT(nvarchar(4000), OLD.[State], 126),
				CONVERT(nvarchar(4000), NEW.[State], 126),
				convert(nvarchar(4000), COALESCE(OLD.[AddressId], NEW.[AddressId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[AddressId] = OLD.[AddressId] or (NEW.[AddressId] Is Null and OLD.[AddressId] Is Null))
			WHERE ((NEW.[State] <> OLD.[State]) 
					Or (NEW.[State] Is Null And OLD.[State] Is Not Null)
					Or (NEW.[State] Is Not Null And OLD.[State] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Country]
	IF UPDATE([Country]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[AddressId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[AddressId], NEW.[AddressId]), 0), '[[AddressId]] Is Null')),
				'[Country]',
				CONVERT(nvarchar(4000), OLD.[Country], 126),
				CONVERT(nvarchar(4000), NEW.[Country], 126),
				convert(nvarchar(4000), COALESCE(OLD.[AddressId], NEW.[AddressId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[AddressId] = OLD.[AddressId] or (NEW.[AddressId] Is Null and OLD.[AddressId] Is Null))
			WHERE ((NEW.[Country] <> OLD.[Country]) 
					Or (NEW.[Country] Is Null And OLD.[Country] Is Not Null)
					Or (NEW.[Country] Is Not Null And OLD.[Country] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ZipCode]
	IF UPDATE([ZipCode]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[AddressId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[AddressId], NEW.[AddressId]), 0), '[[AddressId]] Is Null')),
				'[ZipCode]',
				CONVERT(nvarchar(4000), OLD.[ZipCode], 126),
				CONVERT(nvarchar(4000), NEW.[ZipCode], 126),
				convert(nvarchar(4000), COALESCE(OLD.[AddressId], NEW.[AddressId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[AddressId] = OLD.[AddressId] or (NEW.[AddressId] Is Null and OLD.[AddressId] Is Null))
			WHERE ((NEW.[ZipCode] <> OLD.[ZipCode]) 
					Or (NEW.[ZipCode] Is Null And OLD.[ZipCode] Is Not Null)
					Or (NEW.[ZipCode] Is Not Null And OLD.[ZipCode] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreatedDate]
	IF UPDATE([CreatedDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[AddressId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[AddressId], NEW.[AddressId]), 0), '[[AddressId]] Is Null')),
				'[CreatedDate]',
				CONVERT(nvarchar(4000), OLD.[CreatedDate], 126),
				CONVERT(nvarchar(4000), NEW.[CreatedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[AddressId], NEW.[AddressId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[AddressId] = OLD.[AddressId] or (NEW.[AddressId] Is Null and OLD.[AddressId] Is Null))
			WHERE ((NEW.[CreatedDate] <> OLD.[CreatedDate]) 
					Or (NEW.[CreatedDate] Is Null And OLD.[CreatedDate] Is Not Null)
					Or (NEW.[CreatedDate] Is Not Null And OLD.[CreatedDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreatedSubjectId]
	IF UPDATE([CreatedSubjectId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[AddressId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[AddressId], NEW.[AddressId]), 0), '[[AddressId]] Is Null')),
				'[CreatedSubjectId]',
				CONVERT(nvarchar(4000), OLD.[CreatedSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[CreatedSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[AddressId], NEW.[AddressId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[AddressId] = OLD.[AddressId] or (NEW.[AddressId] Is Null and OLD.[AddressId] Is Null))
			WHERE ((NEW.[CreatedSubjectId] <> OLD.[CreatedSubjectId]) 
					Or (NEW.[CreatedSubjectId] Is Null And OLD.[CreatedSubjectId] Is Not Null)
					Or (NEW.[CreatedSubjectId] Is Not Null And OLD.[CreatedSubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LastModifiedDate]
	IF UPDATE([LastModifiedDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[AddressId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[AddressId], NEW.[AddressId]), 0), '[[AddressId]] Is Null')),
				'[LastModifiedDate]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedDate], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[AddressId], NEW.[AddressId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[AddressId] = OLD.[AddressId] or (NEW.[AddressId] Is Null and OLD.[AddressId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[AddressId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[AddressId], NEW.[AddressId]), 0), '[[AddressId]] Is Null')),
				'[LastModifiedSubjectId]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[AddressId], NEW.[AddressId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[AddressId] = OLD.[AddressId] or (NEW.[AddressId] Is Null and OLD.[AddressId] Is Null))
			WHERE ((NEW.[LastModifiedSubjectId] <> OLD.[LastModifiedSubjectId]) 
					Or (NEW.[LastModifiedSubjectId] Is Null And OLD.[LastModifiedSubjectId] Is Not Null)
					Or (NEW.[LastModifiedSubjectId] Is Not Null And OLD.[LastModifiedSubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
