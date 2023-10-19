DROP TRIGGER IF EXISTS auth.trUserRole
GO

---
-- Trigger for UserRole that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trUserRole
	ON [auth].[UserRole]
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
	values('UserRole', 'auth', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

	-- [UserId]
	IF UPDATE([UserId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[UserId]',
				CONVERT(nvarchar(4000), OLD.[UserId], 126),
				CONVERT(nvarchar(4000), NEW.[UserId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[UserId] <> OLD.[UserId]) 
					Or (NEW.[UserId] Is Null And OLD.[UserId] Is Not Null)
					Or (NEW.[UserId] Is Not Null And OLD.[UserId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [RoleId]
	IF UPDATE([RoleId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[RoleId]',
				CONVERT(nvarchar(4000), OLD.[RoleId], 126),
				CONVERT(nvarchar(4000), NEW.[RoleId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[RoleId] <> OLD.[RoleId]) 
					Or (NEW.[RoleId] Is Null And OLD.[RoleId] Is Not Null)
					Or (NEW.[RoleId] Is Not Null And OLD.[RoleId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
