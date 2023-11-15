DROP TRIGGER IF EXISTS AUTH.trPersistedGrants
GO

---
-- Trigger for PersistedGrants that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trPersistedGrants
	ON [AUTH].[PersistedGrants]
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
	values('PersistedGrants', 'AUTH', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

		-- [Key]
	IF UPDATE([Key]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Key]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Key], NEW.[Key]), 0), '[[Key]] Is Null')),
				'[Key]',
				CONVERT(nvarchar(4000), OLD.[Key], 126),
				CONVERT(nvarchar(4000), NEW.[Key], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Key], NEW.[Key], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Key] = OLD.[Key] or (NEW.[Key] Is Null and OLD.[Key] Is Null))
			WHERE ((NEW.[Key] <> OLD.[Key]) 
					Or (NEW.[Key] Is Null And OLD.[Key] Is Not Null)
					Or (NEW.[Key] Is Not Null And OLD.[Key] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Type]
	IF UPDATE([Type]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Key]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Key], NEW.[Key]), 0), '[[Key]] Is Null')),
				'[Type]',
				CONVERT(nvarchar(4000), OLD.[Type], 126),
				CONVERT(nvarchar(4000), NEW.[Type], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Key], NEW.[Key], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Key] = OLD.[Key] or (NEW.[Key] Is Null and OLD.[Key] Is Null))
			WHERE ((NEW.[Type] <> OLD.[Type]) 
					Or (NEW.[Type] Is Null And OLD.[Type] Is Not Null)
					Or (NEW.[Type] Is Not Null And OLD.[Type] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ClientId]
	IF UPDATE([ClientId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Key]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Key], NEW.[Key]), 0), '[[Key]] Is Null')),
				'[ClientId]',
				CONVERT(nvarchar(4000), OLD.[ClientId], 126),
				CONVERT(nvarchar(4000), NEW.[ClientId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Key], NEW.[Key], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Key] = OLD.[Key] or (NEW.[Key] Is Null and OLD.[Key] Is Null))
			WHERE ((NEW.[ClientId] <> OLD.[ClientId]) 
					Or (NEW.[ClientId] Is Null And OLD.[ClientId] Is Not Null)
					Or (NEW.[ClientId] Is Not Null And OLD.[ClientId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreationTime]
	IF UPDATE([CreationTime]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Key]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Key], NEW.[Key]), 0), '[[Key]] Is Null')),
				'[CreationTime]',
				CONVERT(nvarchar(4000), OLD.[CreationTime], 126),
				CONVERT(nvarchar(4000), NEW.[CreationTime], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Key], NEW.[Key], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Key] = OLD.[Key] or (NEW.[Key] Is Null and OLD.[Key] Is Null))
			WHERE ((NEW.[CreationTime] <> OLD.[CreationTime]) 
					Or (NEW.[CreationTime] Is Null And OLD.[CreationTime] Is Not Null)
					Or (NEW.[CreationTime] Is Not Null And OLD.[CreationTime] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Data]
	IF UPDATE([Data]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Key]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Key], NEW.[Key]), 0), '[[Key]] Is Null')),
				'[Data]',
				CONVERT(nvarchar(4000), OLD.[Data], 126),
				CONVERT(nvarchar(4000), NEW.[Data], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Key], NEW.[Key], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Key] = OLD.[Key] or (NEW.[Key] Is Null and OLD.[Key] Is Null))
			WHERE ((NEW.[Data] <> OLD.[Data]) 
					Or (NEW.[Data] Is Null And OLD.[Data] Is Not Null)
					Or (NEW.[Data] Is Not Null And OLD.[Data] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Expiration]
	IF UPDATE([Expiration]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Key]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Key], NEW.[Key]), 0), '[[Key]] Is Null')),
				'[Expiration]',
				CONVERT(nvarchar(4000), OLD.[Expiration], 126),
				CONVERT(nvarchar(4000), NEW.[Expiration], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Key], NEW.[Key], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Key] = OLD.[Key] or (NEW.[Key] Is Null and OLD.[Key] Is Null))
			WHERE ((NEW.[Expiration] <> OLD.[Expiration]) 
					Or (NEW.[Expiration] Is Null And OLD.[Expiration] Is Not Null)
					Or (NEW.[Expiration] Is Not Null And OLD.[Expiration] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [SubjectId]
	IF UPDATE([SubjectId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Key]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Key], NEW.[Key]), 0), '[[Key]] Is Null')),
				'[SubjectId]',
				CONVERT(nvarchar(4000), OLD.[SubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[SubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Key], NEW.[Key], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Key] = OLD.[Key] or (NEW.[Key] Is Null and OLD.[Key] Is Null))
			WHERE ((NEW.[SubjectId] <> OLD.[SubjectId]) 
					Or (NEW.[SubjectId] Is Null And OLD.[SubjectId] Is Not Null)
					Or (NEW.[SubjectId] Is Not Null And OLD.[SubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [SessionId]
	IF UPDATE([SessionId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Key]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Key], NEW.[Key]), 0), '[[Key]] Is Null')),
				'[SessionId]',
				CONVERT(nvarchar(4000), OLD.[SessionId], 126),
				CONVERT(nvarchar(4000), NEW.[SessionId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Key], NEW.[Key], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Key] = OLD.[Key] or (NEW.[Key] Is Null and OLD.[Key] Is Null))
			WHERE ((NEW.[SessionId] <> OLD.[SessionId]) 
					Or (NEW.[SessionId] Is Null And OLD.[SessionId] Is Not Null)
					Or (NEW.[SessionId] Is Not Null And OLD.[SessionId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Description]
	IF UPDATE([Description]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Key]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Key], NEW.[Key]), 0), '[[Key]] Is Null')),
				'[Description]',
				CONVERT(nvarchar(4000), OLD.[Description], 126),
				CONVERT(nvarchar(4000), NEW.[Description], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Key], NEW.[Key], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Key] = OLD.[Key] or (NEW.[Key] Is Null and OLD.[Key] Is Null))
			WHERE ((NEW.[Description] <> OLD.[Description]) 
					Or (NEW.[Description] Is Null And OLD.[Description] Is Not Null)
					Or (NEW.[Description] Is Not Null And OLD.[Description] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ConsumedTime]
	IF UPDATE([ConsumedTime]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Key]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Key], NEW.[Key]), 0), '[[Key]] Is Null')),
				'[ConsumedTime]',
				CONVERT(nvarchar(4000), OLD.[ConsumedTime], 126),
				CONVERT(nvarchar(4000), NEW.[ConsumedTime], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Key], NEW.[Key], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Key] = OLD.[Key] or (NEW.[Key] Is Null and OLD.[Key] Is Null))
			WHERE ((NEW.[ConsumedTime] <> OLD.[ConsumedTime]) 
					Or (NEW.[ConsumedTime] Is Null And OLD.[ConsumedTime] Is Not Null)
					Or (NEW.[ConsumedTime] Is Not Null And OLD.[ConsumedTime] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END



	--IF @Inserted = 0
	--	BEGIN
	--	    -- believed to be contributing to deadlocks
	--		-- DELETE FROM audit.AuditLogTransaction WHERE AuditLogTransactionId = @AuditLogTransactionId
	--	END
END
GO
