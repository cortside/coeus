-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://quickhash.com/
declare @clientId nvarchar(400)
set @clientId='pp-mobile'
if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN
		declare @id int
		INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
			VALUES (2592000, 3600, 1, 1, 0, 0, 1, 1, 1, 300, 1, NULL, N'client_', N'pp-mobile', N'PartnerPortal Mobile', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 1, 1296000, 0)
		set @id = SCOPE_IDENTITY()
		INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'authorization_code')
		INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
		INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
		INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
		INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'multidisbursement.api')
  END
if (exists(select * from auth.Clients where ClientId=@clientId))
  BEGIN
    set @id = (select Id from auth.Clients where ClientId=@clientId)
    declare @scope nvarchar(200)
    set @scope =  N'multidisbursement.backendapp'
    if (not exists(select * from [AUTH].[ClientScopes] where ClientId = @id AND Scope = @scope))
      BEGIN
        INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, @scope)
      END
    set @scope =  N'user-api'
    if (not exists(select * from [AUTH].[ClientScopes] where ClientId = @id AND Scope = @scope))
      BEGIN
        INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, @scope)
      END
    UPDATE [AUTH].[Clients] SET RequireClientSecret = 0 WHERE id = @id
    -- setup allowed redirect uris
    print N'setup allowed redirect uris'
    declare @allowedRedirectUris table (
        Uri nvarchar(1000)
    )
    insert into @allowedRedirectUris values (N'partnerportal://callback')
    insert into [AUTH].[ClientRedirectUris] (ClientId, RedirectUri)
    select @id, a.Uri
    from @allowedRedirectUris a
    where uri not in (select RedirectUri from auth.ClientRedirectUris where clientId=@id)
	-- setup allowed post redirect uris
    print N'setup allowed redirect uris'
    declare @postRedirectUris table (
        Uri nvarchar(1000)
    )
    insert into @postRedirectUris values (N'partnerportal://callback')
    insert into [AUTH].[ClientPostLogoutRedirectUris] (ClientId, PostLogoutRedirectUri)
    select @id, a.Uri
    from @postRedirectUris a
    where uri not in (select PostLogoutRedirectUri from auth.[ClientPostLogoutRedirectUris] where clientId=@id)
  END
