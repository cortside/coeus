DROP TRIGGER IF EXISTS AUTH.trDeviceCodes
GO

---
-- Trigger for DeviceCodes that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trDeviceCodes
	ON [AUTH].[DeviceCodes]
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
	values('DeviceCodes', 'AUTH', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

		-- [UserCode]
	IF UPDATE([UserCode]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserCode]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserCode], NEW.[UserCode]), 0), '[[UserCode]] Is Null')),
				'[UserCode]',
				CONVERT(nvarchar(4000), OLD.[UserCode], 126),
				CONVERT(nvarchar(4000), NEW.[UserCode], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserCode], NEW.[UserCode], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserCode] = OLD.[UserCode] or (NEW.[UserCode] Is Null and OLD.[UserCode] Is Null))
			WHERE ((NEW.[UserCode] <> OLD.[UserCode]) 
					Or (NEW.[UserCode] Is Null And OLD.[UserCode] Is Not Null)
					Or (NEW.[UserCode] Is Not Null And OLD.[UserCode] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [DeviceCode]
	IF UPDATE([DeviceCode]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserCode]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserCode], NEW.[UserCode]), 0), '[[UserCode]] Is Null')),
				'[DeviceCode]',
				CONVERT(nvarchar(4000), OLD.[DeviceCode], 126),
				CONVERT(nvarchar(4000), NEW.[DeviceCode], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserCode], NEW.[UserCode], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserCode] = OLD.[UserCode] or (NEW.[UserCode] Is Null and OLD.[UserCode] Is Null))
			WHERE ((NEW.[DeviceCode] <> OLD.[DeviceCode]) 
					Or (NEW.[DeviceCode] Is Null And OLD.[DeviceCode] Is Not Null)
					Or (NEW.[DeviceCode] Is Not Null And OLD.[DeviceCode] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [SubjectId]
	IF UPDATE([SubjectId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserCode]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserCode], NEW.[UserCode]), 0), '[[UserCode]] Is Null')),
				'[SubjectId]',
				CONVERT(nvarchar(4000), OLD.[SubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[SubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserCode], NEW.[UserCode], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserCode] = OLD.[UserCode] or (NEW.[UserCode] Is Null and OLD.[UserCode] Is Null))
			WHERE ((NEW.[SubjectId] <> OLD.[SubjectId]) 
					Or (NEW.[SubjectId] Is Null And OLD.[SubjectId] Is Not Null)
					Or (NEW.[SubjectId] Is Not Null And OLD.[SubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ClientId]
	IF UPDATE([ClientId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserCode]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserCode], NEW.[UserCode]), 0), '[[UserCode]] Is Null')),
				'[ClientId]',
				CONVERT(nvarchar(4000), OLD.[ClientId], 126),
				CONVERT(nvarchar(4000), NEW.[ClientId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserCode], NEW.[UserCode], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserCode] = OLD.[UserCode] or (NEW.[UserCode] Is Null and OLD.[UserCode] Is Null))
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
				convert(nvarchar(1500), IsNull('[[UserCode]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserCode], NEW.[UserCode]), 0), '[[UserCode]] Is Null')),
				'[CreationTime]',
				CONVERT(nvarchar(4000), OLD.[CreationTime], 126),
				CONVERT(nvarchar(4000), NEW.[CreationTime], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserCode], NEW.[UserCode], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserCode] = OLD.[UserCode] or (NEW.[UserCode] Is Null and OLD.[UserCode] Is Null))
			WHERE ((NEW.[CreationTime] <> OLD.[CreationTime]) 
					Or (NEW.[CreationTime] Is Null And OLD.[CreationTime] Is Not Null)
					Or (NEW.[CreationTime] Is Not Null And OLD.[CreationTime] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Expiration]
	IF UPDATE([Expiration]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserCode]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserCode], NEW.[UserCode]), 0), '[[UserCode]] Is Null')),
				'[Expiration]',
				CONVERT(nvarchar(4000), OLD.[Expiration], 126),
				CONVERT(nvarchar(4000), NEW.[Expiration], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserCode], NEW.[UserCode], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserCode] = OLD.[UserCode] or (NEW.[UserCode] Is Null and OLD.[UserCode] Is Null))
			WHERE ((NEW.[Expiration] <> OLD.[Expiration]) 
					Or (NEW.[Expiration] Is Null And OLD.[Expiration] Is Not Null)
					Or (NEW.[Expiration] Is Not Null And OLD.[Expiration] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Data]
	IF UPDATE([Data]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserCode]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserCode], NEW.[UserCode]), 0), '[[UserCode]] Is Null')),
				'[Data]',
				CONVERT(nvarchar(4000), OLD.[Data], 126),
				CONVERT(nvarchar(4000), NEW.[Data], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserCode], NEW.[UserCode], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserCode] = OLD.[UserCode] or (NEW.[UserCode] Is Null and OLD.[UserCode] Is Null))
			WHERE ((NEW.[Data] <> OLD.[Data]) 
					Or (NEW.[Data] Is Null And OLD.[Data] Is Not Null)
					Or (NEW.[Data] Is Not Null And OLD.[Data] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [SessionId]
	IF UPDATE([SessionId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserCode]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserCode], NEW.[UserCode]), 0), '[[UserCode]] Is Null')),
				'[SessionId]',
				CONVERT(nvarchar(4000), OLD.[SessionId], 126),
				CONVERT(nvarchar(4000), NEW.[SessionId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserCode], NEW.[UserCode], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserCode] = OLD.[UserCode] or (NEW.[UserCode] Is Null and OLD.[UserCode] Is Null))
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
				convert(nvarchar(1500), IsNull('[[UserCode]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserCode], NEW.[UserCode]), 0), '[[UserCode]] Is Null')),
				'[Description]',
				CONVERT(nvarchar(4000), OLD.[Description], 126),
				CONVERT(nvarchar(4000), NEW.[Description], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserCode], NEW.[UserCode], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserCode] = OLD.[UserCode] or (NEW.[UserCode] Is Null and OLD.[UserCode] Is Null))
			WHERE ((NEW.[Description] <> OLD.[Description]) 
					Or (NEW.[Description] Is Null And OLD.[Description] Is Not Null)
					Or (NEW.[Description] Is Not Null And OLD.[Description] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END



	--IF @Inserted = 0
	--	BEGIN
	--	    -- believed to be contributing to deadlocks
	--		-- DELETE FROM audit.AuditLogTransaction WHERE AuditLogTransactionId = @AuditLogTransactionId
	--	END
END
GO
