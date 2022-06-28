declare @clientId nvarchar(400)
set @clientId='sql-reporting-client'

declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN

	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (259200, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', @clientId, N'SQL Reporting Client', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()
	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')
	INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'ecbda3f7-d628-4e5a-879c-cf6caad23ba1');
	INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'sub', '697ca1fe-467f-476f-b89f-f527cfe15b0f');
	INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES(@id, 'role', 'loan-api')
	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: iUu9gfhLrOhJueVj9UyM
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'TsRszbtQ333Q53EBUEjvaFmgTIRfyzWatic12aMFvQI=')

  END

select @id=id from auth.clients where clientId=@clientId
		
if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'eboa.webapi'))
	BEGIN
		INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'eboa.webapi')
	END

if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'openid'))
	BEGIN	
		INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	END
	
if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'profile'))
	BEGIN		
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	END
	
if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'role'))
	BEGIN	
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')	
	END
