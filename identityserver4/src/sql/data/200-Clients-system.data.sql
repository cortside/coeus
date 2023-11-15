Declare @clientName varchar(200) ='system'
Declare @clientId varchar(200) ='system'
Declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN

	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, N'', @clientId, @clientName, NULL, NULL, NULL, 0, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)
  END

SELECT @id = id from [AUTH].[Clients] where ClientId = @clientId

if (not exists(select * from auth.ClientSecrets where clientid = @id))
  BEGIN  
  -- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- Generate the [Value] using ConverTo-IdentityServerHashBase64.ps1
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'invalid-hash=')
  END

if (not exists(select * from auth.ClientGrantTypes where clientid = @id and GrantType = 'client_credentials'))
  BEGIN
	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')
  END
  
if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'openid'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
  END
  
if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'profile'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
  END
  
if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'policyserver.runtime'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.runtime')
  END

if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'catalog-api'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'catalog-api')
  END

if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'shoppingcart-api'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'shoppingcart-api')
  END

if (not exists(select * from auth.ClientClaims where clientid = @id and Type='sub' and Value = '15dc38a9-e9a0-4d44-8244-c7e28b20c558'))
BEGIN
  insert into auth.ClientClaims values(@id, 'sub', '15dc38a9-e9a0-4d44-8244-c7e28b20c558')
END

