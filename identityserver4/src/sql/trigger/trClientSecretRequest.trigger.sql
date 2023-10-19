DROP TRIGGER IF EXISTS auth.trClientSecretRequest
GO

---
-- Trigger for ClientSecretRequest that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trClientSecretRequest
	ON [auth].[ClientSecretRequest]
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
	SELECT TOP 1 @UserName=[LastModifiedUserId] FROM inserted;

	-- insert parent transaction
	INSERT INTO audit.AuditLogTransaction (TableName, TableSchema, Action, HostName, ApplicationName, AuditLogin, AuditDate, AffectedRows, DatabaseName, UserId, TransactionId)
	values('ClientSecretRequest', 'auth', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
		CASE WHEN LEN(APP_NAME()) < 1 THEN ' ' ELSE APP_NAME() END,
		SUSER_SNAME(), GETDATE(), @ROWS_COUNT, db_name(), @UserName, CURRENT_TRANSACTION_ID()
	)
	Set @AuditLogTransactionId = SCOPE_IDENTITY()

	-- [ClientSecretRequestId]
	IF UPDATE([ClientSecretRequestId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ClientSecretRequestId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId]), 0), '[[ClientSecretRequestId]] Is Null')),
				'[ClientSecretRequestId]',
				CONVERT(nvarchar(4000), OLD.[ClientSecretRequestId], 126),
				CONVERT(nvarchar(4000), NEW.[ClientSecretRequestId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ClientSecretRequestId] = OLD.[ClientSecretRequestId] or (NEW.[ClientSecretRequestId] Is Null and OLD.[ClientSecretRequestId] Is Null))
			WHERE ((NEW.[ClientSecretRequestId] <> OLD.[ClientSecretRequestId]) 
					Or (NEW.[ClientSecretRequestId] Is Null And OLD.[ClientSecretRequestId] Is Not Null)
					Or (NEW.[ClientSecretRequestId] Is Not Null And OLD.[ClientSecretRequestId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreateUserId]
	IF UPDATE([CreateUserId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ClientSecretRequestId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId]), 0), '[[ClientSecretRequestId]] Is Null')),
				'[CreateUserId]',
				CONVERT(nvarchar(4000), OLD.[CreateUserId], 126),
				CONVERT(nvarchar(4000), NEW.[CreateUserId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ClientSecretRequestId] = OLD.[ClientSecretRequestId] or (NEW.[ClientSecretRequestId] Is Null and OLD.[ClientSecretRequestId] Is Null))
			WHERE ((NEW.[CreateUserId] <> OLD.[CreateUserId]) 
					Or (NEW.[CreateUserId] Is Null And OLD.[CreateUserId] Is Not Null)
					Or (NEW.[CreateUserId] Is Not Null And OLD.[CreateUserId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [CreateDate]
	IF UPDATE([CreateDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ClientSecretRequestId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId]), 0), '[[ClientSecretRequestId]] Is Null')),
				'[CreateDate]',
				CONVERT(nvarchar(4000), OLD.[CreateDate], 126),
				CONVERT(nvarchar(4000), NEW.[CreateDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ClientSecretRequestId] = OLD.[ClientSecretRequestId] or (NEW.[ClientSecretRequestId] Is Null and OLD.[ClientSecretRequestId] Is Null))
			WHERE ((NEW.[CreateDate] <> OLD.[CreateDate]) 
					Or (NEW.[CreateDate] Is Null And OLD.[CreateDate] Is Not Null)
					Or (NEW.[CreateDate] Is Not Null And OLD.[CreateDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LastModifiedUserId]
	IF UPDATE([LastModifiedUserId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ClientSecretRequestId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId]), 0), '[[ClientSecretRequestId]] Is Null')),
				'[LastModifiedUserId]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedUserId], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedUserId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ClientSecretRequestId] = OLD.[ClientSecretRequestId] or (NEW.[ClientSecretRequestId] Is Null and OLD.[ClientSecretRequestId] Is Null))
			WHERE ((NEW.[LastModifiedUserId] <> OLD.[LastModifiedUserId]) 
					Or (NEW.[LastModifiedUserId] Is Null And OLD.[LastModifiedUserId] Is Not Null)
					Or (NEW.[LastModifiedUserId] Is Not Null And OLD.[LastModifiedUserId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LastModifiedDate]
	IF UPDATE([LastModifiedDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ClientSecretRequestId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId]), 0), '[[ClientSecretRequestId]] Is Null')),
				'[LastModifiedDate]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedDate], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ClientSecretRequestId] = OLD.[ClientSecretRequestId] or (NEW.[ClientSecretRequestId] Is Null and OLD.[ClientSecretRequestId] Is Null))
			WHERE ((NEW.[LastModifiedDate] <> OLD.[LastModifiedDate]) 
					Or (NEW.[LastModifiedDate] Is Null And OLD.[LastModifiedDate] Is Not Null)
					Or (NEW.[LastModifiedDate] Is Not Null And OLD.[LastModifiedDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ClientId]
	IF UPDATE([ClientId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ClientSecretRequestId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId]), 0), '[[ClientSecretRequestId]] Is Null')),
				'[ClientId]',
				CONVERT(nvarchar(4000), OLD.[ClientId], 126),
				CONVERT(nvarchar(4000), NEW.[ClientId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ClientSecretRequestId] = OLD.[ClientSecretRequestId] or (NEW.[ClientSecretRequestId] Is Null and OLD.[ClientSecretRequestId] Is Null))
			WHERE ((NEW.[ClientId] <> OLD.[ClientId]) 
					Or (NEW.[ClientId] Is Null And OLD.[ClientId] Is Not Null)
					Or (NEW.[ClientId] Is Not Null And OLD.[ClientId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Token]
	IF UPDATE([Token]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ClientSecretRequestId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId]), 0), '[[ClientSecretRequestId]] Is Null')),
				'[Token]',
				CONVERT(nvarchar(4000), OLD.[Token], 126),
				CONVERT(nvarchar(4000), NEW.[Token], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ClientSecretRequestId] = OLD.[ClientSecretRequestId] or (NEW.[ClientSecretRequestId] Is Null and OLD.[ClientSecretRequestId] Is Null))
			WHERE ((NEW.[Token] <> OLD.[Token]) 
					Or (NEW.[Token] Is Null And OLD.[Token] Is Not Null)
					Or (NEW.[Token] Is Not Null And OLD.[Token] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [SmsVerificationCode]
	IF UPDATE([SmsVerificationCode]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ClientSecretRequestId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId]), 0), '[[ClientSecretRequestId]] Is Null')),
				'[SmsVerificationCode]',
				CONVERT(nvarchar(4000), OLD.[SmsVerificationCode], 126),
				CONVERT(nvarchar(4000), NEW.[SmsVerificationCode], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ClientSecretRequestId] = OLD.[ClientSecretRequestId] or (NEW.[ClientSecretRequestId] Is Null and OLD.[ClientSecretRequestId] Is Null))
			WHERE ((NEW.[SmsVerificationCode] <> OLD.[SmsVerificationCode]) 
					Or (NEW.[SmsVerificationCode] Is Null And OLD.[SmsVerificationCode] Is Not Null)
					Or (NEW.[SmsVerificationCode] Is Not Null And OLD.[SmsVerificationCode] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [RequestExpirationDate]
	IF UPDATE([RequestExpirationDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ClientSecretRequestId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId]), 0), '[[ClientSecretRequestId]] Is Null')),
				'[RequestExpirationDate]',
				CONVERT(nvarchar(4000), OLD.[RequestExpirationDate], 126),
				CONVERT(nvarchar(4000), NEW.[RequestExpirationDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ClientSecretRequestId] = OLD.[ClientSecretRequestId] or (NEW.[ClientSecretRequestId] Is Null and OLD.[ClientSecretRequestId] Is Null))
			WHERE ((NEW.[RequestExpirationDate] <> OLD.[RequestExpirationDate]) 
					Or (NEW.[RequestExpirationDate] Is Null And OLD.[RequestExpirationDate] Is Not Null)
					Or (NEW.[RequestExpirationDate] Is Not Null And OLD.[RequestExpirationDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [SmsVerificationExpiration]
	IF UPDATE([SmsVerificationExpiration]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ClientSecretRequestId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId]), 0), '[[ClientSecretRequestId]] Is Null')),
				'[SmsVerificationExpiration]',
				CONVERT(nvarchar(4000), OLD.[SmsVerificationExpiration], 126),
				CONVERT(nvarchar(4000), NEW.[SmsVerificationExpiration], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ClientSecretRequestId] = OLD.[ClientSecretRequestId] or (NEW.[ClientSecretRequestId] Is Null and OLD.[ClientSecretRequestId] Is Null))
			WHERE ((NEW.[SmsVerificationExpiration] <> OLD.[SmsVerificationExpiration]) 
					Or (NEW.[SmsVerificationExpiration] Is Null And OLD.[SmsVerificationExpiration] Is Not Null)
					Or (NEW.[SmsVerificationExpiration] Is Not Null And OLD.[SmsVerificationExpiration] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [RemainingSmsVerificationAttempts]
	IF UPDATE([RemainingSmsVerificationAttempts]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[ClientSecretRequestId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId]), 0), '[[ClientSecretRequestId]] Is Null')),
				'[RemainingSmsVerificationAttempts]',
				CONVERT(nvarchar(4000), OLD.[RemainingSmsVerificationAttempts], 126),
				CONVERT(nvarchar(4000), NEW.[RemainingSmsVerificationAttempts], 126),
				convert(nvarchar(4000), COALESCE(OLD.[ClientSecretRequestId], NEW.[ClientSecretRequestId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[ClientSecretRequestId] = OLD.[ClientSecretRequestId] or (NEW.[ClientSecretRequestId] Is Null and OLD.[ClientSecretRequestId] Is Null))
			WHERE ((NEW.[RemainingSmsVerificationAttempts] <> OLD.[RemainingSmsVerificationAttempts]) 
					Or (NEW.[RemainingSmsVerificationAttempts] Is Null And OLD.[RemainingSmsVerificationAttempts] Is Not Null)
					Or (NEW.[RemainingSmsVerificationAttempts] Is Not Null And OLD.[RemainingSmsVerificationAttempts] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
