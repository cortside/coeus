-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://approsto.com/sha-generator/

if not exists (select * from auth.clients)
  BEGIN
	SET IDENTITY_INSERT [AUTH].[Clients] ON 
	INSERT [AUTH].[Clients] ([Id], [AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) VALUES (5, 2592000, 3600, 0, 0, 0, 0, 1, 1, 1, 300, 1, NULL, N'', N'acme', NULL, NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)
	SET IDENTITY_INSERT [AUTH].[Clients] OFF
	SET IDENTITY_INSERT [AUTH].[ClientClaims] ON 
	INSERT [AUTH].[ClientClaims] ([Id], [ClientId], [Type], [Value]) VALUES (3, 5, N'sub', N'36C90FC9-496C-40CD-87D9-23DC3F79AE2C')
	INSERT [AUTH].[ClientClaims] ([Id], [ClientId], [Type], [Value]) VALUES (5, 5, 'role', 'admin');
	SET IDENTITY_INSERT [AUTH].[ClientClaims] OFF
	SET IDENTITY_INSERT [AUTH].[ClientGrantTypes] ON 
	INSERT [AUTH].[ClientGrantTypes] ([Id], [ClientId], [GrantType]) VALUES (5, 5, N'client_credentials')
	SET IDENTITY_INSERT [AUTH].[ClientGrantTypes] OFF
	SET IDENTITY_INSERT [AUTH].[ClientScopes] ON 
	INSERT [AUTH].[ClientScopes] ([Id], [ClientId], [Scope]) VALUES (16, 5, N'openid')
	INSERT [AUTH].[ClientScopes] ([Id], [ClientId], [Scope]) VALUES (17, 5, N'profile')
	INSERT [AUTH].[ClientScopes] ([Id], [ClientId], [Scope]) VALUES (15, 5, N'role')
	INSERT [AUTH].[ClientScopes] ([Id], [ClientId], [Scope]) VALUES (18, 5, N'eboa.webapi')
	SET IDENTITY_INSERT [AUTH].[ClientScopes] OFF
	SET IDENTITY_INSERT [AUTH].[ClientSecrets] ON 
	-- secret
	INSERT [AUTH].[ClientSecrets] ([Id], [ClientId], [Description], [Expiration], [Type], [Value]) VALUES (1, 5, NULL, NULL, N'SharedSecret', N'invalid-hash=')
	SET IDENTITY_INSERT [AUTH].[ClientSecrets] OFF
  END

  -- remove lenders group from acme user OD-9002
  if exists (select * from auth.ClientClaims where clientId=5 and Type='groups') 
  begin
    delete from auth.ClientClaims where clientId=5 and Type='groups'
  end
