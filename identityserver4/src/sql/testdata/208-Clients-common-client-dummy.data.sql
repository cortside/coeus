declare @clientId nvarchar(400)
set @clientId='communications-client-dummy'

declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN

	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', N'communications-client-dummy', N'Communications Client Dummy User', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'common.communications')

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: "97b6eab7ad9a220d902aeaf57bc161bce0078408"
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'4kB1hrEzE/Y8YQLimmjXbDR1odgv5uImAKVHxD+6zw8=')

  END

if (exists(SELECT * FROM [AUTH].[Clients] WHERE ClientId=@clientId))
	BEGIN
		set @id = (SELECT Id FROM [AUTH].[Clients] WHERE ClientId=@clientId)

  		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'multidisbursement.api'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'multidisbursement.api')
			END
			
		if (not exists(SELECT * FROM [AUTH].[ClientClaims] WHERE ClientId=@id And Type=N'sub'))
			BEGIN
				INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'sub', '18828577-1f02-4c9e-8b32-015b502026b6');
			END
		
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'policyserver.runtime'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.runtime')
			END

		UPDATE [AUTH].[Clients]
		SET AccessTokenLifetime = 259200 --Three days
		WHERE Id = @id AND ClientId = @clientId
    END
