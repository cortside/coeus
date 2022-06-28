declare @clientId nvarchar(400)
set @clientId='user-api-client'

declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN

	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', @clientId, N'User API Client', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')

	INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'user-api');
	INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'sub', '6bc9b7da-e1f7-4bb6-be26-1f256240546b');

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: userapipass
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'invalid-hash=')

  END

if (exists(SELECT * FROM [AUTH].[Clients] WHERE ClientId=@clientId))
	BEGIN
		set @id = (SELECT Id FROM [AUTH].[Clients] WHERE ClientId=@clientId)

		UPDATE [AUTH].[Clients]
		SET AccessTokenLifetime = 259200 --Three days
		WHERE Id = @id AND ClientId = @clientId


		declare @userApiRole nvarchar(50)
    	set @userApiRole = 'user-api'

		if (not exists(select * from [AUTH].[ClientClaims] where ClientId=@id AND Type='role' AND Value=@userApiRole))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', @userApiRole)
			END
			
		declare @identityScope nvarchar(50)
		set @identityScope = 'identity'
			
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=@identityScope))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, @identityScope)
			END	
            
        declare @policyScope nvarchar(50)
		set @policyScope = 'policyserver.management'
			
        if (not exists(select * from auth.clientscopes where clientId=@id AND [Scope]=@policyScope))
            BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, @policyScope)
            END

		declare @policyScopeRuntime nvarchar(50)
		set @policyScopeRuntime = 'policyserver.runtime'
			
        if (not exists(select * from auth.clientscopes where clientId=@id AND [Scope]=@policyScopeRuntime))
            BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, @policyScopeRuntime)
            END
	
    	declare @commsScope nvarchar(50)
		set @commsScope = 'common.communications'
			
        if (not exists(select * from auth.clientscopes where clientId=@id AND [Scope]=@commsScope))
            BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, @commsScope)
            END

		declare @productScope nvarchar(50)
		set @productScope = 'maintenance.api'
			
        if (not exists(select * from auth.clientscopes where clientId=@id AND [Scope]=@productScope))
            BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, @productScope)
            END
    END

