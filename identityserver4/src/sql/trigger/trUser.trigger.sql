DROP TRIGGER IF EXISTS auth.trUser
GO

---
-- Trigger for User that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trUser
	ON [auth].[User]
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
	values('User', 'auth', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
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

	-- [CreateUserId]
	IF UPDATE([CreateUserId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[CreateUserId]',
				CONVERT(nvarchar(4000), OLD.[CreateUserId], 126),
				CONVERT(nvarchar(4000), NEW.[CreateUserId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[CreateDate]',
				CONVERT(nvarchar(4000), OLD.[CreateDate], 126),
				CONVERT(nvarchar(4000), NEW.[CreateDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[LastModifiedUserId]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedUserId], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedUserId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
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
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[LastModifiedDate]',
				CONVERT(nvarchar(4000), OLD.[LastModifiedDate], 126),
				CONVERT(nvarchar(4000), NEW.[LastModifiedDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[LastModifiedDate] <> OLD.[LastModifiedDate]) 
					Or (NEW.[LastModifiedDate] Is Null And OLD.[LastModifiedDate] Is Not Null)
					Or (NEW.[LastModifiedDate] Is Not Null And OLD.[LastModifiedDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Username]
	IF UPDATE([Username]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[Username]',
				CONVERT(nvarchar(4000), OLD.[Username], 126),
				CONVERT(nvarchar(4000), NEW.[Username], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[Username] <> OLD.[Username]) 
					Or (NEW.[Username] Is Null And OLD.[Username] Is Not Null)
					Or (NEW.[Username] Is Not Null And OLD.[Username] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Password]
	IF UPDATE([Password]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[Password]',
				CONVERT(nvarchar(4000), OLD.[Password], 126),
				CONVERT(nvarchar(4000), NEW.[Password], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[Password] <> OLD.[Password]) 
					Or (NEW.[Password] Is Null And OLD.[Password] Is Not Null)
					Or (NEW.[Password] Is Not Null And OLD.[Password] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Salt]
	IF UPDATE([Salt]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[Salt]',
				CONVERT(nvarchar(4000), OLD.[Salt], 126),
				CONVERT(nvarchar(4000), NEW.[Salt], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[Salt] <> OLD.[Salt]) 
					Or (NEW.[Salt] Is Null And OLD.[Salt] Is Not Null)
					Or (NEW.[Salt] Is Not Null And OLD.[Salt] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [UserStatus]
	IF UPDATE([UserStatus]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[UserStatus]',
				CONVERT(nvarchar(4000), OLD.[UserStatus], 126),
				CONVERT(nvarchar(4000), NEW.[UserStatus], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[UserStatus] <> OLD.[UserStatus]) 
					Or (NEW.[UserStatus] Is Null And OLD.[UserStatus] Is Not Null)
					Or (NEW.[UserStatus] Is Not Null And OLD.[UserStatus] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [EffectiveDate]
	IF UPDATE([EffectiveDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[EffectiveDate]',
				CONVERT(nvarchar(4000), OLD.[EffectiveDate], 126),
				CONVERT(nvarchar(4000), NEW.[EffectiveDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[EffectiveDate] <> OLD.[EffectiveDate]) 
					Or (NEW.[EffectiveDate] Is Null And OLD.[EffectiveDate] Is Not Null)
					Or (NEW.[EffectiveDate] Is Not Null And OLD.[EffectiveDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ExpirationDate]
	IF UPDATE([ExpirationDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[ExpirationDate]',
				CONVERT(nvarchar(4000), OLD.[ExpirationDate], 126),
				CONVERT(nvarchar(4000), NEW.[ExpirationDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[ExpirationDate] <> OLD.[ExpirationDate]) 
					Or (NEW.[ExpirationDate] Is Null And OLD.[ExpirationDate] Is Not Null)
					Or (NEW.[ExpirationDate] Is Not Null And OLD.[ExpirationDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LoginCount]
	IF UPDATE([LoginCount]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[LoginCount]',
				CONVERT(nvarchar(4000), OLD.[LoginCount], 126),
				CONVERT(nvarchar(4000), NEW.[LoginCount], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[LoginCount] <> OLD.[LoginCount]) 
					Or (NEW.[LoginCount] Is Null And OLD.[LoginCount] Is Not Null)
					Or (NEW.[LoginCount] Is Not Null And OLD.[LoginCount] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LastLogin]
	IF UPDATE([LastLogin]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[LastLogin]',
				CONVERT(nvarchar(4000), OLD.[LastLogin], 126),
				CONVERT(nvarchar(4000), NEW.[LastLogin], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[LastLogin] <> OLD.[LastLogin]) 
					Or (NEW.[LastLogin] Is Null And OLD.[LastLogin] Is Not Null)
					Or (NEW.[LastLogin] Is Not Null And OLD.[LastLogin] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LastLoginIPAddress]
	IF UPDATE([LastLoginIPAddress]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[LastLoginIPAddress]',
				CONVERT(nvarchar(4000), OLD.[LastLoginIPAddress], 126),
				CONVERT(nvarchar(4000), NEW.[LastLoginIPAddress], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[LastLoginIPAddress] <> OLD.[LastLoginIPAddress]) 
					Or (NEW.[LastLoginIPAddress] Is Null And OLD.[LastLoginIPAddress] Is Not Null)
					Or (NEW.[LastLoginIPAddress] Is Not Null And OLD.[LastLoginIPAddress] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LockedReason]
	IF UPDATE([LockedReason]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[LockedReason]',
				CONVERT(nvarchar(4000), OLD.[LockedReason], 126),
				CONVERT(nvarchar(4000), NEW.[LockedReason], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[LockedReason] <> OLD.[LockedReason]) 
					Or (NEW.[LockedReason] Is Null And OLD.[LockedReason] Is Not Null)
					Or (NEW.[LockedReason] Is Not Null And OLD.[LockedReason] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [TermsOfUseAcceptanceDate]
	IF UPDATE([TermsOfUseAcceptanceDate]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[TermsOfUseAcceptanceDate]',
				CONVERT(nvarchar(4000), OLD.[TermsOfUseAcceptanceDate], 126),
				CONVERT(nvarchar(4000), NEW.[TermsOfUseAcceptanceDate], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[TermsOfUseAcceptanceDate] <> OLD.[TermsOfUseAcceptanceDate]) 
					Or (NEW.[TermsOfUseAcceptanceDate] Is Null And OLD.[TermsOfUseAcceptanceDate] Is Not Null)
					Or (NEW.[TermsOfUseAcceptanceDate] Is Not Null And OLD.[TermsOfUseAcceptanceDate] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ProviderName]
	IF UPDATE([ProviderName]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[ProviderName]',
				CONVERT(nvarchar(4000), OLD.[ProviderName], 126),
				CONVERT(nvarchar(4000), NEW.[ProviderName], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[ProviderName] <> OLD.[ProviderName]) 
					Or (NEW.[ProviderName] Is Null And OLD.[ProviderName] Is Not Null)
					Or (NEW.[ProviderName] Is Not Null And OLD.[ProviderName] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ProviderSubjectId]
	IF UPDATE([ProviderSubjectId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[ProviderSubjectId]',
				CONVERT(nvarchar(4000), OLD.[ProviderSubjectId], 126),
				CONVERT(nvarchar(4000), NEW.[ProviderSubjectId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[ProviderSubjectId] <> OLD.[ProviderSubjectId]) 
					Or (NEW.[ProviderSubjectId] Is Null And OLD.[ProviderSubjectId] Is Not Null)
					Or (NEW.[ProviderSubjectId] Is Not Null And OLD.[ProviderSubjectId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [IsLocked]
	IF UPDATE([IsLocked]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[IsLocked]',
				CONVERT(nvarchar(4000), OLD.[IsLocked], 126),
				CONVERT(nvarchar(4000), NEW.[IsLocked], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[IsLocked] <> OLD.[IsLocked]) 
					Or (NEW.[IsLocked] Is Null And OLD.[IsLocked] Is Not Null)
					Or (NEW.[IsLocked] Is Not Null And OLD.[IsLocked] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [TwoFactor]
	IF UPDATE([TwoFactor]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[TwoFactor]',
				CONVERT(nvarchar(4000), OLD.[TwoFactor], 126),
				CONVERT(nvarchar(4000), NEW.[TwoFactor], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[TwoFactor] <> OLD.[TwoFactor]) 
					Or (NEW.[TwoFactor] Is Null And OLD.[TwoFactor] Is Not Null)
					Or (NEW.[TwoFactor] Is Not Null And OLD.[TwoFactor] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [TwoFactorVerified]
	IF UPDATE([TwoFactorVerified]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[UserId]]='+CONVERT(nvarchar(4000), IsNull(OLD.[UserId], NEW.[UserId]), 0), '[[UserId]] Is Null')),
				'[TwoFactorVerified]',
				CONVERT(nvarchar(4000), OLD.[TwoFactorVerified], 126),
				CONVERT(nvarchar(4000), NEW.[TwoFactorVerified], 126),
				convert(nvarchar(4000), COALESCE(OLD.[UserId], NEW.[UserId], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[UserId] = OLD.[UserId] or (NEW.[UserId] Is Null and OLD.[UserId] Is Null))
			WHERE ((NEW.[TwoFactorVerified] <> OLD.[TwoFactorVerified]) 
					Or (NEW.[TwoFactorVerified] Is Null And OLD.[TwoFactorVerified] Is Not Null)
					Or (NEW.[TwoFactorVerified] Is Not Null And OLD.[TwoFactorVerified] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
