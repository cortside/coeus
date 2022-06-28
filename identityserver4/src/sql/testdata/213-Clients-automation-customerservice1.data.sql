-- "admin" client with access to all scopes

declare @clientId nvarchar(400)
declare @id int
set @clientId='automation.customerservice1'

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN
	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', @clientId, @clientId, NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'eboa.webapi')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'document-api')


	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: secret
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=')
  END

-- make sure client has access to all api scopes
select @id=id from auth.clients where clientId=@clientId
insert into auth.clientscopes(clientId, scope) select @id, name from auth.ApiResources where name not in (select scope from auth.ClientScopes where clientId=@id)

if not exists (select * from auth.ClientClaims where clientId=@id)
  BEGIN
	insert into auth.ClientClaims values(@id, 'groups', '639efbc8-f03b-4a02-be93-2d18140db730')
	insert into auth.ClientClaims values(@id, 'sub', '128547a1-4a94-0d56-0000-000000000000')
  END
