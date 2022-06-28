declare @clientId nvarchar(400)
set @clientId='md-backendapp-client'

declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN

	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', @clientId, N'Multidisbursement Backend App Client Dummy User', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'multidisbursement.api')
	INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'admin');

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: mdbackendapppass
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'NQKc9PltG9iYQMgdudW4MZurd5CDAaHRhD3oB370OXQ=')

  END

if (exists(SELECT * FROM [AUTH].[Clients] WHERE ClientId=@clientId))
	BEGIN
		declare @entryType nvarchar(50)
		set @entryType='sub'

		set @id = (SELECT Id FROM [AUTH].[Clients] WHERE ClientId=@clientId)

		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=@entryType))
			BEGIN
  				INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, @entryType, '824989db-a618-4e13-8b1d-de0306b7aed5');
			END

		declare @scope nvarchar(200)
		set @scope =  N'multidisbursement.backendapp'

		if (not exists(select * from [AUTH].[ClientScopes] where ClientId = @id AND Scope = @scope))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, @scope)
			END

		----- For accessing LoanServicing/Loans API
		if (not exists(select * from [AUTH].[ClientScopes] where ClientId = @id AND Scope = 'loan-api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, 'loan-api')
			END
			
		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type='role' AND Value=N'loan-api'))
			BEGIN
                INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'loan-api');
			END

		----- User API Access
		if (not exists(select * from [AUTH].[ClientScopes] where ClientId = @id AND Scope = 'user-api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, 'user-api')
			END	

		----- PolicyServer Access -----
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'policyserver.runtime'))
            BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.runtime')
            END
		
		----- Document Service Access (need the correct scope and the document-api role, as that's how PolicyServer is setup for that service) -----
		if (not exists(select * from [AUTH].[ClientScopes] where ClientId = @id AND Scope = 'document-api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, 'document-api')
			END
	
		if (not exists(select * from [Auth].[ClientClaims] WHERE [ClientId] = @id AND [Type] = 'role' AND [Value] = 'document-api'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'document-api');
			END
	END