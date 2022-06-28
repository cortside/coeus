declare @clientId nvarchar(400)
set @clientId='calendar-api-client'

declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN

	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', N'calendar-api-client', N'Calendar Api Client User', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'calendar.api')
	INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'admin');

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: calendarpass
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'invalid-hash=')

  END

declare @entryType nvarchar(50)
set @entryType='sub'

if (exists(SELECT * FROM [AUTH].[Clients] WHERE ClientId=@clientId))
	BEGIN
		set @id = (SELECT Id FROM [AUTH].[Clients] WHERE ClientId=@clientId)
		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=@entryType))
			BEGIN
  				INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, @entryType, '9d24ef42-48da-452d-ba5f-d05e5a360be7');
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'policyserver.runtime'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.runtime')
			END	

		UPDATE [AUTH].[Clients]
		SET AccessTokenLifetime = 259200 --Three days
		WHERE Id = @id AND ClientId = @clientId

	END

