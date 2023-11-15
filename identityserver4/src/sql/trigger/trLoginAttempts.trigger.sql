DROP TRIGGER IF EXISTS auth.trLoginAttempts
GO

---
-- Trigger for LoginAttempts that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trLoginAttempts
	ON [auth].[LoginAttempts]
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
	set @username = current_user

	-- insert parent transaction
	INSERT INTO audit.AuditLogTransaction (TableName, TableSchema, Action, HostName, ApplicationName, AuditLogin, AuditDate, AffectedRows, DatabaseName, UserId, TransactionId)
	values('LoginAttempts', 'auth', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

	-- [Id]
	IF UPDATE([Id]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[Id]',
				CONVERT(nvarchar(4000), OLD.[Id], 126),
				CONVERT(nvarchar(4000), NEW.[Id], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[Id] <> OLD.[Id]) 
					Or (NEW.[Id] Is Null And OLD.[Id] Is Not Null)
					Or (NEW.[Id] Is Not Null And OLD.[Id] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AttemptedOn]
	IF UPDATE([AttemptedOn]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AttemptedOn]',
				CONVERT(nvarchar(4000), OLD.[AttemptedOn], 126),
				CONVERT(nvarchar(4000), NEW.[AttemptedOn], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AttemptedOn] <> OLD.[AttemptedOn]) 
					Or (NEW.[AttemptedOn] Is Null And OLD.[AttemptedOn] Is Not Null)
					Or (NEW.[AttemptedOn] Is Not Null And OLD.[AttemptedOn] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [IpAddress]
	IF UPDATE([IpAddress]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[IpAddress]',
				CONVERT(nvarchar(4000), OLD.[IpAddress], 126),
				CONVERT(nvarchar(4000), NEW.[IpAddress], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[IpAddress] <> OLD.[IpAddress]) 
					Or (NEW.[IpAddress] Is Null And OLD.[IpAddress] Is Not Null)
					Or (NEW.[IpAddress] Is Not Null And OLD.[IpAddress] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Successful]
	IF UPDATE([Successful]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[Successful]',
				CONVERT(nvarchar(4000), OLD.[Successful], 126),
				CONVERT(nvarchar(4000), NEW.[Successful], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[Successful] <> OLD.[Successful]) 
					Or (NEW.[Successful] Is Null And OLD.[Successful] Is Not Null)
					Or (NEW.[Successful] Is Not Null And OLD.[Successful] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [UserId]
	IF UPDATE([UserId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[UserId]',
				CONVERT(nvarchar(4000), OLD.[UserId], 126),
				CONVERT(nvarchar(4000), NEW.[UserId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[UserId] <> OLD.[UserId]) 
					Or (NEW.[UserId] Is Null And OLD.[UserId] Is Not Null)
					Or (NEW.[UserId] Is Not Null And OLD.[UserId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
