DROP TRIGGER IF EXISTS auth.trClients
GO

---
-- Trigger for Clients that will handle logging of both update and delete
-- NOTE: inserted data is current value in row if not altered
---
CREATE TRIGGER trClients
	ON [auth].[Clients]
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
	values('Clients', 'auth', @action, CASE WHEN LEN(HOST_NAME()) < 1 THEN ' ' ELSE HOST_NAME() END,
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

	-- [AbsoluteRefreshTokenLifetime]
	IF UPDATE([AbsoluteRefreshTokenLifetime]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AbsoluteRefreshTokenLifetime]',
				CONVERT(nvarchar(4000), OLD.[AbsoluteRefreshTokenLifetime], 126),
				CONVERT(nvarchar(4000), NEW.[AbsoluteRefreshTokenLifetime], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AbsoluteRefreshTokenLifetime] <> OLD.[AbsoluteRefreshTokenLifetime]) 
					Or (NEW.[AbsoluteRefreshTokenLifetime] Is Null And OLD.[AbsoluteRefreshTokenLifetime] Is Not Null)
					Or (NEW.[AbsoluteRefreshTokenLifetime] Is Not Null And OLD.[AbsoluteRefreshTokenLifetime] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AccessTokenLifetime]
	IF UPDATE([AccessTokenLifetime]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AccessTokenLifetime]',
				CONVERT(nvarchar(4000), OLD.[AccessTokenLifetime], 126),
				CONVERT(nvarchar(4000), NEW.[AccessTokenLifetime], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AccessTokenLifetime] <> OLD.[AccessTokenLifetime]) 
					Or (NEW.[AccessTokenLifetime] Is Null And OLD.[AccessTokenLifetime] Is Not Null)
					Or (NEW.[AccessTokenLifetime] Is Not Null And OLD.[AccessTokenLifetime] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AccessTokenType]
	IF UPDATE([AccessTokenType]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AccessTokenType]',
				CONVERT(nvarchar(4000), OLD.[AccessTokenType], 126),
				CONVERT(nvarchar(4000), NEW.[AccessTokenType], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AccessTokenType] <> OLD.[AccessTokenType]) 
					Or (NEW.[AccessTokenType] Is Null And OLD.[AccessTokenType] Is Not Null)
					Or (NEW.[AccessTokenType] Is Not Null And OLD.[AccessTokenType] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AllowAccessTokensViaBrowser]
	IF UPDATE([AllowAccessTokensViaBrowser]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AllowAccessTokensViaBrowser]',
				CONVERT(nvarchar(4000), OLD.[AllowAccessTokensViaBrowser], 126),
				CONVERT(nvarchar(4000), NEW.[AllowAccessTokensViaBrowser], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AllowAccessTokensViaBrowser] <> OLD.[AllowAccessTokensViaBrowser]) 
					Or (NEW.[AllowAccessTokensViaBrowser] Is Null And OLD.[AllowAccessTokensViaBrowser] Is Not Null)
					Or (NEW.[AllowAccessTokensViaBrowser] Is Not Null And OLD.[AllowAccessTokensViaBrowser] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AllowOfflineAccess]
	IF UPDATE([AllowOfflineAccess]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AllowOfflineAccess]',
				CONVERT(nvarchar(4000), OLD.[AllowOfflineAccess], 126),
				CONVERT(nvarchar(4000), NEW.[AllowOfflineAccess], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AllowOfflineAccess] <> OLD.[AllowOfflineAccess]) 
					Or (NEW.[AllowOfflineAccess] Is Null And OLD.[AllowOfflineAccess] Is Not Null)
					Or (NEW.[AllowOfflineAccess] Is Not Null And OLD.[AllowOfflineAccess] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AllowPlainTextPkce]
	IF UPDATE([AllowPlainTextPkce]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AllowPlainTextPkce]',
				CONVERT(nvarchar(4000), OLD.[AllowPlainTextPkce], 126),
				CONVERT(nvarchar(4000), NEW.[AllowPlainTextPkce], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AllowPlainTextPkce] <> OLD.[AllowPlainTextPkce]) 
					Or (NEW.[AllowPlainTextPkce] Is Null And OLD.[AllowPlainTextPkce] Is Not Null)
					Or (NEW.[AllowPlainTextPkce] Is Not Null And OLD.[AllowPlainTextPkce] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AllowRememberConsent]
	IF UPDATE([AllowRememberConsent]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AllowRememberConsent]',
				CONVERT(nvarchar(4000), OLD.[AllowRememberConsent], 126),
				CONVERT(nvarchar(4000), NEW.[AllowRememberConsent], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AllowRememberConsent] <> OLD.[AllowRememberConsent]) 
					Or (NEW.[AllowRememberConsent] Is Null And OLD.[AllowRememberConsent] Is Not Null)
					Or (NEW.[AllowRememberConsent] Is Not Null And OLD.[AllowRememberConsent] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AlwaysIncludeUserClaimsInIdToken]
	IF UPDATE([AlwaysIncludeUserClaimsInIdToken]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AlwaysIncludeUserClaimsInIdToken]',
				CONVERT(nvarchar(4000), OLD.[AlwaysIncludeUserClaimsInIdToken], 126),
				CONVERT(nvarchar(4000), NEW.[AlwaysIncludeUserClaimsInIdToken], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AlwaysIncludeUserClaimsInIdToken] <> OLD.[AlwaysIncludeUserClaimsInIdToken]) 
					Or (NEW.[AlwaysIncludeUserClaimsInIdToken] Is Null And OLD.[AlwaysIncludeUserClaimsInIdToken] Is Not Null)
					Or (NEW.[AlwaysIncludeUserClaimsInIdToken] Is Not Null And OLD.[AlwaysIncludeUserClaimsInIdToken] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AllowedIdentityTokenSigningAlgorithms]
	IF UPDATE([AllowedIdentityTokenSigningAlgorithms]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AllowedIdentityTokenSigningAlgorithms]',
				CONVERT(nvarchar(4000), OLD.[AllowedIdentityTokenSigningAlgorithms], 126),
				CONVERT(nvarchar(4000), NEW.[AllowedIdentityTokenSigningAlgorithms], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AllowedIdentityTokenSigningAlgorithms] <> OLD.[AllowedIdentityTokenSigningAlgorithms]) 
					Or (NEW.[AllowedIdentityTokenSigningAlgorithms] Is Null And OLD.[AllowedIdentityTokenSigningAlgorithms] Is Not Null)
					Or (NEW.[AllowedIdentityTokenSigningAlgorithms] Is Not Null And OLD.[AllowedIdentityTokenSigningAlgorithms] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AlwaysSendClientClaims]
	IF UPDATE([AlwaysSendClientClaims]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AlwaysSendClientClaims]',
				CONVERT(nvarchar(4000), OLD.[AlwaysSendClientClaims], 126),
				CONVERT(nvarchar(4000), NEW.[AlwaysSendClientClaims], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AlwaysSendClientClaims] <> OLD.[AlwaysSendClientClaims]) 
					Or (NEW.[AlwaysSendClientClaims] Is Null And OLD.[AlwaysSendClientClaims] Is Not Null)
					Or (NEW.[AlwaysSendClientClaims] Is Not Null And OLD.[AlwaysSendClientClaims] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [AuthorizationCodeLifetime]
	IF UPDATE([AuthorizationCodeLifetime]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[AuthorizationCodeLifetime]',
				CONVERT(nvarchar(4000), OLD.[AuthorizationCodeLifetime], 126),
				CONVERT(nvarchar(4000), NEW.[AuthorizationCodeLifetime], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[AuthorizationCodeLifetime] <> OLD.[AuthorizationCodeLifetime]) 
					Or (NEW.[AuthorizationCodeLifetime] Is Null And OLD.[AuthorizationCodeLifetime] Is Not Null)
					Or (NEW.[AuthorizationCodeLifetime] Is Not Null And OLD.[AuthorizationCodeLifetime] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [BackChannelLogoutSessionRequired]
	IF UPDATE([BackChannelLogoutSessionRequired]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[BackChannelLogoutSessionRequired]',
				CONVERT(nvarchar(4000), OLD.[BackChannelLogoutSessionRequired], 126),
				CONVERT(nvarchar(4000), NEW.[BackChannelLogoutSessionRequired], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[BackChannelLogoutSessionRequired] <> OLD.[BackChannelLogoutSessionRequired]) 
					Or (NEW.[BackChannelLogoutSessionRequired] Is Null And OLD.[BackChannelLogoutSessionRequired] Is Not Null)
					Or (NEW.[BackChannelLogoutSessionRequired] Is Not Null And OLD.[BackChannelLogoutSessionRequired] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [BackChannelLogoutUri]
	IF UPDATE([BackChannelLogoutUri]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[BackChannelLogoutUri]',
				CONVERT(nvarchar(4000), OLD.[BackChannelLogoutUri], 126),
				CONVERT(nvarchar(4000), NEW.[BackChannelLogoutUri], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[BackChannelLogoutUri] <> OLD.[BackChannelLogoutUri]) 
					Or (NEW.[BackChannelLogoutUri] Is Null And OLD.[BackChannelLogoutUri] Is Not Null)
					Or (NEW.[BackChannelLogoutUri] Is Not Null And OLD.[BackChannelLogoutUri] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ClientClaimsPrefix]
	IF UPDATE([ClientClaimsPrefix]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[ClientClaimsPrefix]',
				CONVERT(nvarchar(4000), OLD.[ClientClaimsPrefix], 126),
				CONVERT(nvarchar(4000), NEW.[ClientClaimsPrefix], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[ClientClaimsPrefix] <> OLD.[ClientClaimsPrefix]) 
					Or (NEW.[ClientClaimsPrefix] Is Null And OLD.[ClientClaimsPrefix] Is Not Null)
					Or (NEW.[ClientClaimsPrefix] Is Not Null And OLD.[ClientClaimsPrefix] Is Null))
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

	-- [ClientName]
	IF UPDATE([ClientName]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[ClientName]',
				CONVERT(nvarchar(4000), OLD.[ClientName], 126),
				CONVERT(nvarchar(4000), NEW.[ClientName], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[ClientName] <> OLD.[ClientName]) 
					Or (NEW.[ClientName] Is Null And OLD.[ClientName] Is Not Null)
					Or (NEW.[ClientName] Is Not Null And OLD.[ClientName] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ClientUri]
	IF UPDATE([ClientUri]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[ClientUri]',
				CONVERT(nvarchar(4000), OLD.[ClientUri], 126),
				CONVERT(nvarchar(4000), NEW.[ClientUri], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[ClientUri] <> OLD.[ClientUri]) 
					Or (NEW.[ClientUri] Is Null And OLD.[ClientUri] Is Not Null)
					Or (NEW.[ClientUri] Is Not Null And OLD.[ClientUri] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ConsentLifetime]
	IF UPDATE([ConsentLifetime]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[ConsentLifetime]',
				CONVERT(nvarchar(4000), OLD.[ConsentLifetime], 126),
				CONVERT(nvarchar(4000), NEW.[ConsentLifetime], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[ConsentLifetime] <> OLD.[ConsentLifetime]) 
					Or (NEW.[ConsentLifetime] Is Null And OLD.[ConsentLifetime] Is Not Null)
					Or (NEW.[ConsentLifetime] Is Not Null And OLD.[ConsentLifetime] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Description]
	IF UPDATE([Description]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[Description]',
				CONVERT(nvarchar(4000), OLD.[Description], 126),
				CONVERT(nvarchar(4000), NEW.[Description], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[Description] <> OLD.[Description]) 
					Or (NEW.[Description] Is Null And OLD.[Description] Is Not Null)
					Or (NEW.[Description] Is Not Null And OLD.[Description] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [EnableLocalLogin]
	IF UPDATE([EnableLocalLogin]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[EnableLocalLogin]',
				CONVERT(nvarchar(4000), OLD.[EnableLocalLogin], 126),
				CONVERT(nvarchar(4000), NEW.[EnableLocalLogin], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[EnableLocalLogin] <> OLD.[EnableLocalLogin]) 
					Or (NEW.[EnableLocalLogin] Is Null And OLD.[EnableLocalLogin] Is Not Null)
					Or (NEW.[EnableLocalLogin] Is Not Null And OLD.[EnableLocalLogin] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [Enabled]
	IF UPDATE([Enabled]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[Enabled]',
				CONVERT(nvarchar(4000), OLD.[Enabled], 126),
				CONVERT(nvarchar(4000), NEW.[Enabled], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[Enabled] <> OLD.[Enabled]) 
					Or (NEW.[Enabled] Is Null And OLD.[Enabled] Is Not Null)
					Or (NEW.[Enabled] Is Not Null And OLD.[Enabled] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [FrontChannelLogoutSessionRequired]
	IF UPDATE([FrontChannelLogoutSessionRequired]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[FrontChannelLogoutSessionRequired]',
				CONVERT(nvarchar(4000), OLD.[FrontChannelLogoutSessionRequired], 126),
				CONVERT(nvarchar(4000), NEW.[FrontChannelLogoutSessionRequired], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[FrontChannelLogoutSessionRequired] <> OLD.[FrontChannelLogoutSessionRequired]) 
					Or (NEW.[FrontChannelLogoutSessionRequired] Is Null And OLD.[FrontChannelLogoutSessionRequired] Is Not Null)
					Or (NEW.[FrontChannelLogoutSessionRequired] Is Not Null And OLD.[FrontChannelLogoutSessionRequired] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [FrontChannelLogoutUri]
	IF UPDATE([FrontChannelLogoutUri]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[FrontChannelLogoutUri]',
				CONVERT(nvarchar(4000), OLD.[FrontChannelLogoutUri], 126),
				CONVERT(nvarchar(4000), NEW.[FrontChannelLogoutUri], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[FrontChannelLogoutUri] <> OLD.[FrontChannelLogoutUri]) 
					Or (NEW.[FrontChannelLogoutUri] Is Null And OLD.[FrontChannelLogoutUri] Is Not Null)
					Or (NEW.[FrontChannelLogoutUri] Is Not Null And OLD.[FrontChannelLogoutUri] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [IdentityTokenLifetime]
	IF UPDATE([IdentityTokenLifetime]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[IdentityTokenLifetime]',
				CONVERT(nvarchar(4000), OLD.[IdentityTokenLifetime], 126),
				CONVERT(nvarchar(4000), NEW.[IdentityTokenLifetime], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[IdentityTokenLifetime] <> OLD.[IdentityTokenLifetime]) 
					Or (NEW.[IdentityTokenLifetime] Is Null And OLD.[IdentityTokenLifetime] Is Not Null)
					Or (NEW.[IdentityTokenLifetime] Is Not Null And OLD.[IdentityTokenLifetime] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [IncludeJwtId]
	IF UPDATE([IncludeJwtId]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[IncludeJwtId]',
				CONVERT(nvarchar(4000), OLD.[IncludeJwtId], 126),
				CONVERT(nvarchar(4000), NEW.[IncludeJwtId], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[IncludeJwtId] <> OLD.[IncludeJwtId]) 
					Or (NEW.[IncludeJwtId] Is Null And OLD.[IncludeJwtId] Is Not Null)
					Or (NEW.[IncludeJwtId] Is Not Null And OLD.[IncludeJwtId] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LogoUri]
	IF UPDATE([LogoUri]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[LogoUri]',
				CONVERT(nvarchar(4000), OLD.[LogoUri], 126),
				CONVERT(nvarchar(4000), NEW.[LogoUri], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[LogoUri] <> OLD.[LogoUri]) 
					Or (NEW.[LogoUri] Is Null And OLD.[LogoUri] Is Not Null)
					Or (NEW.[LogoUri] Is Not Null And OLD.[LogoUri] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [PairWiseSubjectSalt]
	IF UPDATE([PairWiseSubjectSalt]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[PairWiseSubjectSalt]',
				CONVERT(nvarchar(4000), OLD.[PairWiseSubjectSalt], 126),
				CONVERT(nvarchar(4000), NEW.[PairWiseSubjectSalt], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[PairWiseSubjectSalt] <> OLD.[PairWiseSubjectSalt]) 
					Or (NEW.[PairWiseSubjectSalt] Is Null And OLD.[PairWiseSubjectSalt] Is Not Null)
					Or (NEW.[PairWiseSubjectSalt] Is Not Null And OLD.[PairWiseSubjectSalt] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LogoutSessionRequired]
	IF UPDATE([LogoutSessionRequired]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[LogoutSessionRequired]',
				CONVERT(nvarchar(4000), OLD.[LogoutSessionRequired], 126),
				CONVERT(nvarchar(4000), NEW.[LogoutSessionRequired], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[LogoutSessionRequired] <> OLD.[LogoutSessionRequired]) 
					Or (NEW.[LogoutSessionRequired] Is Null And OLD.[LogoutSessionRequired] Is Not Null)
					Or (NEW.[LogoutSessionRequired] Is Not Null And OLD.[LogoutSessionRequired] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [LogoutUri]
	IF UPDATE([LogoutUri]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[LogoutUri]',
				CONVERT(nvarchar(4000), OLD.[LogoutUri], 126),
				CONVERT(nvarchar(4000), NEW.[LogoutUri], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[LogoutUri] <> OLD.[LogoutUri]) 
					Or (NEW.[LogoutUri] Is Null And OLD.[LogoutUri] Is Not Null)
					Or (NEW.[LogoutUri] Is Not Null And OLD.[LogoutUri] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [NonEditable]
	IF UPDATE([NonEditable]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[NonEditable]',
				CONVERT(nvarchar(4000), OLD.[NonEditable], 126),
				CONVERT(nvarchar(4000), NEW.[NonEditable], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[NonEditable] <> OLD.[NonEditable]) 
					Or (NEW.[NonEditable] Is Null And OLD.[NonEditable] Is Not Null)
					Or (NEW.[NonEditable] Is Not Null And OLD.[NonEditable] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [ProtocolType]
	IF UPDATE([ProtocolType]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[ProtocolType]',
				CONVERT(nvarchar(4000), OLD.[ProtocolType], 126),
				CONVERT(nvarchar(4000), NEW.[ProtocolType], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[ProtocolType] <> OLD.[ProtocolType]) 
					Or (NEW.[ProtocolType] Is Null And OLD.[ProtocolType] Is Not Null)
					Or (NEW.[ProtocolType] Is Not Null And OLD.[ProtocolType] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [RefreshTokenExpiration]
	IF UPDATE([RefreshTokenExpiration]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[RefreshTokenExpiration]',
				CONVERT(nvarchar(4000), OLD.[RefreshTokenExpiration], 126),
				CONVERT(nvarchar(4000), NEW.[RefreshTokenExpiration], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[RefreshTokenExpiration] <> OLD.[RefreshTokenExpiration]) 
					Or (NEW.[RefreshTokenExpiration] Is Null And OLD.[RefreshTokenExpiration] Is Not Null)
					Or (NEW.[RefreshTokenExpiration] Is Not Null And OLD.[RefreshTokenExpiration] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [RefreshTokenUsage]
	IF UPDATE([RefreshTokenUsage]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[RefreshTokenUsage]',
				CONVERT(nvarchar(4000), OLD.[RefreshTokenUsage], 126),
				CONVERT(nvarchar(4000), NEW.[RefreshTokenUsage], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[RefreshTokenUsage] <> OLD.[RefreshTokenUsage]) 
					Or (NEW.[RefreshTokenUsage] Is Null And OLD.[RefreshTokenUsage] Is Not Null)
					Or (NEW.[RefreshTokenUsage] Is Not Null And OLD.[RefreshTokenUsage] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [RequireClientSecret]
	IF UPDATE([RequireClientSecret]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[RequireClientSecret]',
				CONVERT(nvarchar(4000), OLD.[RequireClientSecret], 126),
				CONVERT(nvarchar(4000), NEW.[RequireClientSecret], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[RequireClientSecret] <> OLD.[RequireClientSecret]) 
					Or (NEW.[RequireClientSecret] Is Null And OLD.[RequireClientSecret] Is Not Null)
					Or (NEW.[RequireClientSecret] Is Not Null And OLD.[RequireClientSecret] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [RequireConsent]
	IF UPDATE([RequireConsent]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[RequireConsent]',
				CONVERT(nvarchar(4000), OLD.[RequireConsent], 126),
				CONVERT(nvarchar(4000), NEW.[RequireConsent], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[RequireConsent] <> OLD.[RequireConsent]) 
					Or (NEW.[RequireConsent] Is Null And OLD.[RequireConsent] Is Not Null)
					Or (NEW.[RequireConsent] Is Not Null And OLD.[RequireConsent] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [RequirePkce]
	IF UPDATE([RequirePkce]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[RequirePkce]',
				CONVERT(nvarchar(4000), OLD.[RequirePkce], 126),
				CONVERT(nvarchar(4000), NEW.[RequirePkce], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[RequirePkce] <> OLD.[RequirePkce]) 
					Or (NEW.[RequirePkce] Is Null And OLD.[RequirePkce] Is Not Null)
					Or (NEW.[RequirePkce] Is Not Null And OLD.[RequirePkce] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [RequireRequestObject]
	IF UPDATE([RequireRequestObject]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[RequireRequestObject]',
				CONVERT(nvarchar(4000), OLD.[RequireRequestObject], 126),
				CONVERT(nvarchar(4000), NEW.[RequireRequestObject], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[RequireRequestObject] <> OLD.[RequireRequestObject]) 
					Or (NEW.[RequireRequestObject] Is Null And OLD.[RequireRequestObject] Is Not Null)
					Or (NEW.[RequireRequestObject] Is Not Null And OLD.[RequireRequestObject] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [SlidingRefreshTokenLifetime]
	IF UPDATE([SlidingRefreshTokenLifetime]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[SlidingRefreshTokenLifetime]',
				CONVERT(nvarchar(4000), OLD.[SlidingRefreshTokenLifetime], 126),
				CONVERT(nvarchar(4000), NEW.[SlidingRefreshTokenLifetime], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[SlidingRefreshTokenLifetime] <> OLD.[SlidingRefreshTokenLifetime]) 
					Or (NEW.[SlidingRefreshTokenLifetime] Is Null And OLD.[SlidingRefreshTokenLifetime] Is Not Null)
					Or (NEW.[SlidingRefreshTokenLifetime] Is Not Null And OLD.[SlidingRefreshTokenLifetime] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

	-- [UpdateAccessTokenClaimsOnRefresh]
	IF UPDATE([UpdateAccessTokenClaimsOnRefresh]) OR @action in ('INSERT', 'DELETE')      
		BEGIN       
			INSERT INTO audit.AuditLog (AuditLogTransactionId, PrimaryKey, ColumnName, OldValue, NewValue, Key1)
			SELECT
				@AuditLogTransactionId,
				convert(nvarchar(1500), IsNull('[[Id]]='+CONVERT(nvarchar(4000), IsNull(OLD.[Id], NEW.[Id]), 0), '[[Id]] Is Null')),
				'[UpdateAccessTokenClaimsOnRefresh]',
				CONVERT(nvarchar(4000), OLD.[UpdateAccessTokenClaimsOnRefresh], 126),
				CONVERT(nvarchar(4000), NEW.[UpdateAccessTokenClaimsOnRefresh], 126),
				convert(nvarchar(4000), COALESCE(OLD.[Id], NEW.[Id], null))
			FROM deleted OLD 
			LEFT JOIN inserted NEW On (NEW.[Id] = OLD.[Id] or (NEW.[Id] Is Null and OLD.[Id] Is Null))
			WHERE ((NEW.[UpdateAccessTokenClaimsOnRefresh] <> OLD.[UpdateAccessTokenClaimsOnRefresh]) 
					Or (NEW.[UpdateAccessTokenClaimsOnRefresh] Is Null And OLD.[UpdateAccessTokenClaimsOnRefresh] Is Not Null)
					Or (NEW.[UpdateAccessTokenClaimsOnRefresh] Is Not Null And OLD.[UpdateAccessTokenClaimsOnRefresh] Is Null))
			set @inserted = @inserted + @@ROWCOUNT
		END

END
GO
