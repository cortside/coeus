

declare @clientId nvarchar(400)
declare @id int
set @clientId='sqlreport.api'

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN
	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', @clientId, @clientId, NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: 7BrZMQxDUiNXS81G
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'hBp7o136SvAUJcjeqrHI4mQB5VF+sJu3yAFvy/pC0qE=')
  END

select @id=id from auth.clients where clientId=@clientId

if not exists(select * from auth.clientscopes where clientId=@id)
  begin
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.runtime')
  end

if not exists(select * from auth.ClientClaims where clientId=@id)
  begin
	-- MUST be unique across all clients and users (subjects)
	INSERT INTO auth.ClientClaims VALUES (@id, 'role', @clientId)
	INSERT INTO auth.ClientClaims VALUES (@id, 'sub', 'adc5ae20-8bd2-4966-b02a-6de54e61dcc2')
  end

  if (exists(select * from [AUTH].[Clients] where ClientId=@clientId))
	begin
		if (not exists(select * from [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'openid'))
			begin
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
			end
			
		if (not exists(select * from [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'profile'))
			begin
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
			end

		if (not exists(select * from [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'role'))
			begin
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
			end			
			
		if (not exists(select * from [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'policyserver.runtime'))
			begin
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.runtime')
			end

		if (not exists(select * from [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'sqlreport.api'))
			begin
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'sqlreport.api')
			end
        end
