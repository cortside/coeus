DROP TRIGGER IF EXISTS auth.trUserClaim
GO

---
-- Trigger for UserClaim that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trUserClaim
	ON [auth].[UserClaim]
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
	values('UserClaim', 'auth', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

	-- [UserClaimId]
	IF UPDATE([UserClaimId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserClaimId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserClaimId], NEW.[UserClaimId]), 0), '[[UserClaimId]] Is Null')),
				'[UserClaimId]',
				CONVERT(nvarchar(4000), OLD.[UserClaimId], 126),
				CONVERT(nvarchar(4000), NEW.[UserClaimId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserClaimId], NEW.[UserClaimId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserClaimId] = OLD.[UserClaimId] or (NEW.[UserClaimId] Is Null and OLD.[UserClaimId] Is Null))
			WHERE ((NEW.[UserClaimId] <> OLD.[UserClaimId]) 
					Or (NEW.[UserClaimId] Is Null And OLD.[UserClaimId] Is Not Null)
					Or (NEW.[UserClaimId] Is Not Null And OLD.[UserClaimId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [UserId]
	IF UPDATE([UserId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserClaimId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserClaimId], NEW.[UserClaimId]), 0), '[[UserClaimId]] Is Null')),
				'[UserId]',
				CONVERT(nvarchar(4000), OLD.[UserId], 126),
				CONVERT(nvarchar(4000), NEW.[UserId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserClaimId], NEW.[UserClaimId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserClaimId] = OLD.[UserClaimId] or (NEW.[UserClaimId] Is Null and OLD.[UserClaimId] Is Null))
			WHERE ((NEW.[UserId] <> OLD.[UserId]) 
					Or (NEW.[UserId] Is Null And OLD.[UserId] Is Not Null)
					Or (NEW.[UserId] Is Not Null And OLD.[UserId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ProviderName]
	IF UPDATE([ProviderName]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserClaimId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserClaimId], NEW.[UserClaimId]), 0), '[[UserClaimId]] Is Null')),
				'[ProviderName]',
				CONVERT(nvarchar(4000), OLD.[ProviderName], 126),
				CONVERT(nvarchar(4000), NEW.[ProviderName], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserClaimId], NEW.[UserClaimId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserClaimId] = OLD.[UserClaimId] or (NEW.[UserClaimId] Is Null and OLD.[UserClaimId] Is Null))
			WHERE ((NEW.[ProviderName] <> OLD.[ProviderName]) 
					Or (NEW.[ProviderName] Is Null And OLD.[ProviderName] Is Not Null)
					Or (NEW.[ProviderName] Is Not Null And OLD.[ProviderName] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Type]
	IF UPDATE([Type]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserClaimId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserClaimId], NEW.[UserClaimId]), 0), '[[UserClaimId]] Is Null')),
				'[Type]',
				CONVERT(nvarchar(4000), OLD.[Type], 126),
				CONVERT(nvarchar(4000), NEW.[Type], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserClaimId], NEW.[UserClaimId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserClaimId] = OLD.[UserClaimId] or (NEW.[UserClaimId] Is Null and OLD.[UserClaimId] Is Null))
			WHERE ((NEW.[Type] <> OLD.[Type]) 
					Or (NEW.[Type] Is Null And OLD.[Type] Is Not Null)
					Or (NEW.[Type] Is Not Null And OLD.[Type] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Value]
	IF UPDATE([Value]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserClaimId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserClaimId], NEW.[UserClaimId]), 0), '[[UserClaimId]] Is Null')),
				'[Value]',
				CONVERT(nvarchar(4000), OLD.[Value], 126),
				CONVERT(nvarchar(4000), NEW.[Value], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserClaimId], NEW.[UserClaimId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserClaimId] = OLD.[UserClaimId] or (NEW.[UserClaimId] Is Null and OLD.[UserClaimId] Is Null))
			WHERE ((NEW.[Value] <> OLD.[Value]) 
					Or (NEW.[Value] Is Null And OLD.[Value] Is Not Null)
					Or (NEW.[Value] Is Not Null And OLD.[Value] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
