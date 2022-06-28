DROP TRIGGER IF EXISTS AUTH.trIdentityResourceProperties
GO

---
-- Trigger for IdentityResourceProperties that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trIdentityResourceProperties
	ON [AUTH].[IdentityResourceProperties]
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
	SET @UserName = CURRENT_USER

	-- insert parent transaction
	INSERT INTO audit.AuditLogTransaction (TableName, TableSchema, Action, HostName, ApplicationName, AuditLogin, AuditDate, AffectedRows, DatabaseName, UserId, TransactionId)
	values('IdentityResourceProperties', 'AUTH', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
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

	-- [Key]
	IF UPDATE([Key]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[Key]',
				CONVERT(nvarchar(4000), OLD.[Key], 126),
				CONVERT(nvarchar(4000), NEW.[Key], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[Key] <> OLD.[Key]) 
					Or (NEW.[Key] Is Null And OLD.[Key] Is Not Null)
					Or (NEW.[Key] Is Not Null And OLD.[Key] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Value]
	IF UPDATE([Value]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[Value]',
				CONVERT(nvarchar(4000), OLD.[Value], 126),
				CONVERT(nvarchar(4000), NEW.[Value], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[Value] <> OLD.[Value]) 
					Or (NEW.[Value] Is Null And OLD.[Value] Is Not Null)
					Or (NEW.[Value] Is Not Null And OLD.[Value] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [IdentityResourceId]
	IF UPDATE([IdentityResourceId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[IdentityResourceId]',
				CONVERT(nvarchar(4000), OLD.[IdentityResourceId], 126),
				CONVERT(nvarchar(4000), NEW.[IdentityResourceId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[IdentityResourceId] <> OLD.[IdentityResourceId]) 
					Or (NEW.[IdentityResourceId] Is Null And OLD.[IdentityResourceId] Is Not Null)
					Or (NEW.[IdentityResourceId] Is Not Null And OLD.[IdentityResourceId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END



	--IF @Inserted = 0
	--	BEGIN
	--	    -- believed to be contributing to deadlocks
	--		-- DELETE FROM audit.AuditLogTransaction WHERE AuditLogTransactionId = @AuditLogTransactionId
	--	END
END
GO
