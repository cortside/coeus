declare @clientId nvarchar(400)
set @clientId='dw-api-client'

declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN

	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (259200, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', @clientId, N'Datawarehouse API Client', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'datawarehouse-api')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'multidisbursement.api')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'common.communications')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'loan-api')

	INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'admin');
	INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'sub', 'e1da184f-a346-41f4-b98b-c94572b25973');

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: dwapipass
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'invalid-hash=')

  END

  if (exists(SELECT * FROM [AUTH].[Clients] WHERE ClientId=@clientId))
	BEGIN
		set @id = (SELECT Id FROM [AUTH].[Clients] WHERE ClientId=@clientId)

		-- Switching to new subject ID
		UPDATE [AUTH].[ClientClaims]
		SET Value = 'e1da184f-a346-41f4-b98b-c94572b25973'
		WHERE ClientId = @id AND Type = 'sub' AND Value = '36C90FC9-496C-40CD-87D9-23DC3F79AE2C'
		
		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type='sub'))
			BEGIN
  				INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'sub', 'e1da184f-a346-41f4-b98b-c94572b25973');
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'datawarehouse-api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'datawarehouse-api')
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'multidisbursement.api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'multidisbursement.api')
			END
			
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'common.communications'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'common.communications')
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'loan-api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'loan-api')
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'policyserver.runtime'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.runtime')
			END
	END
