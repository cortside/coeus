Declare @clientName varchar(200) ='DecisionEngine Api'
Declare @clientId varchar(200) ='decisionengine.api'
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
	-- Generate the [Value] using the password in https://dotnetfiddle.net/h3aeqd
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
  
if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'maintenance.api'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'maintenance.api')
  END
  
if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'role'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
  END
  
if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'groups'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'groups')
  END

if (not exists(select * from auth.ClientClaims where clientid = @id and Type='role' and Value = 'maintenance.api'))
  BEGIN
	-- MUST be unique across all clients and users (subjects)
	insert into auth.ClientClaims values(@id, 'role', 'maintenance.api')
  END
  
if (not exists(select * from auth.ClientClaims where clientid = @id and Type='sub' and Value = 'f7c73a09-1524-4be4-b754-84366df489d7'))
  BEGIN
  	insert into auth.ClientClaims values(@id, 'sub', 'f7c73a09-1524-4be4-b754-84366df489d7')
  END

if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'policyserver.runtime'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.runtime')
  END
  
  if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'creditreports.api'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'creditreports.api')
  END
  
if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'termscalculator-api'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'termscalculator-api')
  END
   
if (not exists(select * from auth.Clients where Id = @id and AccessTokenType = 0))
  BEGIN
    UPDATE [AUTH].[Clients] SET AccessTokenType = 0 where Id = @id;
  END
 
if (not exists(select * from auth.Clients where Id = @id and ClientClaimsPrefix = ''))
  BEGIN
    UPDATE [AUTH].[Clients] SET ClientClaimsPrefix = '' where Id = @id;
  END
