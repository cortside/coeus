DROP TRIGGER IF EXISTS dbo.trSubject
GO

---
-- Trigger for Subject that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trSubject
	ON [dbo].[Subject]
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
	set @username = current_user

	-- insert parent transaction
	INSERT INTO audit.AuditLogTransaction (TableName, TableSchema, Action, HostName, ApplicationName, AuditLogin, AuditDate, AffectedRows, DatabaseName, UserId, TransactionId)
	values('Subject', 'dbo', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

	-- [SubjectId]
	IF UPDATE([SubjectId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[SubjectId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[SubjectId], NEW.[SubjectId]), 0), '[[SubjectId]] Is Null')),
				'[SubjectId]',
				CONVERT(nvarchar(4000), OLD.[SubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[SubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[SubjectId], NEW.[SubjectId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[SubjectId] = OLD.[SubjectId] or (NEW.[SubjectId] Is Null and OLD.[SubjectId] Is Null))
			WHERE ((NEW.[SubjectId] <> OLD.[SubjectId]) 
					Or (NEW.[SubjectId] Is Null And OLD.[SubjectId] Is Not Null)
					Or (NEW.[SubjectId] Is Not Null And OLD.[SubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Name]
	IF UPDATE([Name]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[SubjectId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[SubjectId], NEW.[SubjectId]), 0), '[[SubjectId]] Is Null')),
				'[Name]',
				CONVERT(nvarchar(4000), OLD.[Name], 126),
				CONVERT(nvarchar(4000), NEW.[Name], 126),
				convert(nvarchar(4000), COALESCE(OLD.[SubjectId], NEW.[SubjectId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[SubjectId] = OLD.[SubjectId] or (NEW.[SubjectId] Is Null and OLD.[SubjectId] Is Null))
			WHERE ((NEW.[Name] <> OLD.[Name]) 
					Or (NEW.[Name] Is Null And OLD.[Name] Is Not Null)
					Or (NEW.[Name] Is Not Null And OLD.[Name] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [GivenName]
	IF UPDATE([GivenName]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[SubjectId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[SubjectId], NEW.[SubjectId]), 0), '[[SubjectId]] Is Null')),
				'[GivenName]',
				CONVERT(nvarchar(4000), OLD.[GivenName], 126),
				CONVERT(nvarchar(4000), NEW.[GivenName], 126),
				convert(nvarchar(4000), COALESCE(OLD.[SubjectId], NEW.[SubjectId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[SubjectId] = OLD.[SubjectId] or (NEW.[SubjectId] Is Null and OLD.[SubjectId] Is Null))
			WHERE ((NEW.[GivenName] <> OLD.[GivenName]) 
					Or (NEW.[GivenName] Is Null And OLD.[GivenName] Is Not Null)
					Or (NEW.[GivenName] Is Not Null And OLD.[GivenName] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [FamilyName]
	IF UPDATE([FamilyName]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[SubjectId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[SubjectId], NEW.[SubjectId]), 0), '[[SubjectId]] Is Null')),
				'[FamilyName]',
				CONVERT(nvarchar(4000), OLD.[FamilyName], 126),
				CONVERT(nvarchar(4000), NEW.[FamilyName], 126),
				convert(nvarchar(4000), COALESCE(OLD.[SubjectId], NEW.[SubjectId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[SubjectId] = OLD.[SubjectId] or (NEW.[SubjectId] Is Null and OLD.[SubjectId] Is Null))
			WHERE ((NEW.[FamilyName] <> OLD.[FamilyName]) 
					Or (NEW.[FamilyName] Is Null And OLD.[FamilyName] Is Not Null)
					Or (NEW.[FamilyName] Is Not Null And OLD.[FamilyName] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [UserPrincipalName]
	IF UPDATE([UserPrincipalName]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[SubjectId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[SubjectId], NEW.[SubjectId]), 0), '[[SubjectId]] Is Null')),
				'[UserPrincipalName]',
				CONVERT(nvarchar(4000), OLD.[UserPrincipalName], 126),
				CONVERT(nvarchar(4000), NEW.[UserPrincipalName], 126),
				convert(nvarchar(4000), COALESCE(OLD.[SubjectId], NEW.[SubjectId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[SubjectId] = OLD.[SubjectId] or (NEW.[SubjectId] Is Null and OLD.[SubjectId] Is Null))
			WHERE ((NEW.[UserPrincipalName] <> OLD.[UserPrincipalName]) 
					Or (NEW.[UserPrincipalName] Is Null And OLD.[UserPrincipalName] Is Not Null)
					Or (NEW.[UserPrincipalName] Is Not Null And OLD.[UserPrincipalName] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreatedDate]
	IF UPDATE([CreatedDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[SubjectId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[SubjectId], NEW.[SubjectId]), 0), '[[SubjectId]] Is Null')),
				'[CreatedDate]',
				CONVERT(nvarchar(4000), OLD.[CreatedDate], 126),
				CONVERT(nvarchar(4000), NEW.[CreatedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[SubjectId], NEW.[SubjectId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[SubjectId] = OLD.[SubjectId] or (NEW.[SubjectId] Is Null and OLD.[SubjectId] Is Null))
			WHERE ((NEW.[CreatedDate] <> OLD.[CreatedDate]) 
					Or (NEW.[CreatedDate] Is Null And OLD.[CreatedDate] Is Not Null)
					Or (NEW.[CreatedDate] Is Not Null And OLD.[CreatedDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
