DROP TRIGGER IF EXISTS AUTH.trClientIdPRestrictions
GO

---
-- Trigger for ClientIdPRestrictions that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trClientIdPRestrictions
	ON [AUTH].[ClientIdPRestrictions]
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
	values('ClientIdPRestrictions', 'AUTH', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
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

	-- [ClientId]
	IF UPDATE([ClientId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[ClientId]',
				CONVERT(nvarchar(4000), OLD.[ClientId], 126),
				CONVERT(nvarchar(4000), NEW.[ClientId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[ClientId] <> OLD.[ClientId]) 
					Or (NEW.[ClientId] Is Null And OLD.[ClientId] Is Not Null)
					Or (NEW.[ClientId] Is Not Null And OLD.[ClientId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Provider]
	IF UPDATE([Provider]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[Provider]',
				CONVERT(nvarchar(4000), OLD.[Provider], 126),
				CONVERT(nvarchar(4000), NEW.[Provider], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[Provider] <> OLD.[Provider]) 
					Or (NEW.[Provider] Is Null And OLD.[Provider] Is Not Null)
					Or (NEW.[Provider] Is Not Null And OLD.[Provider] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END



	--IF @Inserted = 0
	--	BEGIN
	--	    -- believed to be contributing to deadlocks
	--		-- DELETE FROM audit.AuditLogTransaction WHERE AuditLogTransactionId = @AuditLogTransactionId
	--	END
END
GO
