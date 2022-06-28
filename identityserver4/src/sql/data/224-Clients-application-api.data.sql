

declare @clientId nvarchar(400)
declare @id int
set @clientId='application-api'

Update AUTH.ClientClaims 
Set Value = 'd2e9c5de-f619-4a64-b657-32f8abc5ebb6'
Where [Type] = 'sub' and [Value] = 'D2E9C5DE-F619-4A64-B657-32f8ABC5EBB6' COLLATE Latin1_General_CS_AS

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN
	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', @clientId, @clientId, NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: 4yG6SmfSmSx8UZND
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'invalid-hash=')
  END

select @id=id from auth.clients where clientId=@clientId

if not exists(select * from auth.clientscopes where clientId=@id)
  begin
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'document-api')
  end

 
if not exists(select * from auth.ClientClaims where clientId=@id)
  begin
	-- MUST be unique across all clients and users (subjects)
	insert into auth.ClientClaims values(@id, 'role', 'application-api')
	insert into auth.ClientClaims values(@id, 'sub', 'd2e9c5de-f619-4a64-b657-32f8abc5ebb6')
  end
  
if (exists(select * from [AUTH].[Clients] where ClientId=@clientId))
	begin
		set @id = (select Id from [AUTH].[Clients] where ClientId=@clientId)

		if (not exists(select * from [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'common.communications'))
			begin
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'common.communications')
			end
	end

if not exists(select * from [AUTH].[ClientScopes] where [ClientId]=@id and [Scope]=N'termscalculator-api')
  begin
    insert [AUTH].[ClientScopes] ([ClientId], [Scope]) values (@id, N'termscalculator-api')
  end

if not exists(select * from [AUTH].[ClientScopes] where [ClientId]=@id and [Scope]=N'policyserver.runtime')
  begin
    insert [AUTH].[ClientScopes] ([ClientId], [Scope]) values (@id, N'policyserver.runtime')
  end

 if not exists(select * from auth.clientscopes where clientId=@id and scope=N'user-api')
  begin
  	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'user-api')
  end

 if not exists(select * from auth.clientscopes where clientId=@id and scope=N'decisionengine-api')
  begin
  	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'decisionengine-api')
  end

 if not exists(select * from auth.clientscopes where clientId=@id and scope=N'creditreports.api')
  begin
  	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'creditreports.api')
  end

   if not exists(select * from auth.clientscopes where clientId=@id and scope=N'loan-api')
  begin
  	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'loan-api')
  end
  
 if not exists(select * from auth.clientscopes where clientId=@id and scope=N'maintenance.api')
  begin
  	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'maintenance.api')
  end
