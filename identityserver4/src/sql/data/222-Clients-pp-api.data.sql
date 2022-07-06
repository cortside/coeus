declare @clientId nvarchar(400)
declare @id int
set @clientId='pp-api'

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN
	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', @clientId, @clientId, NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'invalid-hash=')

  END

select @id=id from auth.clients where clientId=@clientId

if not exists(select * from auth.clientscopes where clientId=@id)
  begin
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'common.communications')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'partnerportal')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'user-api')
  end

if not exists(select * from auth.ClientClaims where clientId=@id)
  begin
	-- MUST be unique across all clients and users (subjects)
	insert into auth.ClientClaims values(@id, 'role', 'pp-api')
	insert into auth.ClientClaims values(@id, 'sub', 'b40dc64b-a9fe-4112-b750-bb6872f450ed')
  end

if not exists(select * from auth.clientscopes where clientId=@id and [Scope]=N'eboa.webapi')
  begin
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'eboa.webapi')
  end

if not exists(select * from auth.clientscopes where clientId=@id AND [Scope]=N'multidisbursement.api')
    BEGIN
        INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'multidisbursement.api')
    END

if not exists(select * from auth.clientscopes where clientId=@id AND [Scope]=N'adminservicelegacy-api')
    BEGIN
        INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'adminservicelegacy-api')
    END

if not exists(select * from auth.clientscopes where clientId=@id AND [Scope]=N'reportservicelegacy-api')
    BEGIN
        INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'reportservicelegacy-api')
    END
	