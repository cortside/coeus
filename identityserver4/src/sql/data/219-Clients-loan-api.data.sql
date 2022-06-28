

declare @clientId nvarchar(400)
declare @id int
set @clientId='loan-api'

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN
	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', @clientId, @clientId, NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: 327903c72C2c4CBaB248
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'invalid-hash')
  END

select @id=id from auth.clients where clientId=@clientId

if not exists(select * from auth.clientscopes where clientId=@id)
  begin
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'eboa.webapi')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'document-api')
  end

if not exists(select * from auth.ClientClaims where clientId=@id)
  begin
	-- MUST be unique across all clients and users (subjects)
	insert into auth.ClientClaims values(@id, 'role', 'loan-api')
	insert into auth.ClientClaims values(@id, 'sub', '8280fb16-077b-493a-bdaf-50f62e6d6ae9')
  end
  
if (exists(select * from [AUTH].[Clients] where ClientId=@clientId))
	begin
		set @id = (select Id from [AUTH].[Clients] where ClientId=@clientId)

		if (not exists(select * from [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'common.communications'))
			begin
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'common.communications')
			end
			
		if (not exists(select * from [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'policyserver.runtime'))
			begin
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.runtime')
			end
		if (not exists(select * from [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'fundingmanagement.api'))
			begin
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'fundingmanagement.api')
			end			
			
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'user-api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'user-api')
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'termscalculator-api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'termscalculator-api')
			END    
		if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'calendar.api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'calendar.api')
			END
		if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'maintenance.api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'maintenance.api')
			END 
	end


