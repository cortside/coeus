#How to add a new client.

add a new sql script to: src/sql/data/ using this template:
for to use this template you'll need a unique client ID,
a password, a secret (generated via the linked tool)
and a new GUID unquie to the new client.


	declare @clientId nvarchar(400)
	declare @id int
	set @clientId={Client Identity}

	if (not exists(select * from auth.Clients where clientId=@clientId))
  	BEGIN
	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', @clientId, @clientId, NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate. (Consider using a random password generator).
	-- password: {Client Password}
	-- Generate the [Value] using the password in https://dotnetfiddle.net/h3aeqd (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'{Generated Secret}')
	END

	select @id=id from auth.clients where clientId=@clientId

	if not exists(select * from auth.clientscopes where clientId=@id)
	begin
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'eboa.webapi')
	end

	if not exists(select * from auth.ClientClaims where clientId=@id)
	begin
	-- MUST be unique across all clients and users (subjects)
	-- If you are adding a new clinet you must generate a new GUID.
	insert into auth.ClientClaims values(@id, 'sub', '{new client GUID}')
	end
