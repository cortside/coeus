
declare @id int
declare @clientId nvarchar(400)
set @clientId='pp-web'

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN
	--Just set the default values. The Update script later will set the settings we want
    INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], 
    [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], 
    [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], 
    [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], 
    [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], 
    [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], 
    [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], 
    [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
      VALUES (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, N'', @clientId, N'', 
      NULL, NULL, NULL, 0, 0, 0, NULL, 0, 0, NULL, NULL, 0, NULL, 0, N'', 0, 0, 0, 0, 0, 0, 0)

    set @id = (select id from auth.clients where clientId=@clientId)

    INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'implicit')

    --Local environment
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'http://localhost:8088/partnerportal/Login.aspx')
    INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, N'http://localhost:8088/partnerportal/Login.aspx')
    INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'http://localhost:8088')

	--Dev Internal environment
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://portal-dev-admin.cortside.com/PartnerPortal/Login.aspx')
    INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, N'https://portal-dev-admin.cortside.com/PartnerPortal/Login.aspx')
    INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://portal-dev-admin.cortside.com')
	
	--Dev External environment
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://portal-dev.cortside.com/PartnerPortal/Login.aspx')
    INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, N'https://portal-dev.cortside.com/PartnerPortal/Login.aspx')
    INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://portal-dev.cortside.com')

    --Test Internal environment
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://portal-test-admin.cortside.com/PartnerPortal/Login.aspx')
    INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, N'https://portal-test-admin.cortside.com/PartnerPortal/Login.aspx')
    INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://portal-test-admin.cortside.com')
	
	--Test External environment
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://portal-test.cortside.com/PartnerPortal/Login.aspx')
    INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, N'https://portal-test.cortside.com/PartnerPortal/Login.aspx')
    INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://portal-test.cortside.com')

    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'partnerportal')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'user-api')
   
	 --Update the Client settings with the values we need
	 --Definitions of Client fields - http://docs.identityserver.io/en/latest/reference/client.html?highlight=AccessTokenType
	UPDATE [AUTH].[Clients]
	   SET  
		--*********************
		-- BASIC SETTINGS
		--*********************
		  -- Specifies whether this client is allowed to receive access tokens via the browser. 
		  -- This is useful to harden flows that allow multiple response types 
		  -- (e.g. by disallowing a hybrid flow client that is supposed to use code id_token to add the 
		  -- token response type and thus leaking the token to the browser.)
		  [AllowAccessTokensViaBrowser] = 1
		  -- Specifies whether this client can request refresh tokens (be requesting the offline_access scope)
		  ,[AllowOfflineAccess] = 0
		  -- Specifies whether clients using PKCE can use a plain text code challenge (not recommended - and default to false)
		  ,[AllowPlainTextPkce] = 0
		  -- Unique ID of the client
		  ,[ClientId] = @clientId
		  ,[Description] = NULL
		  -- Specifies if client is enabled. Defaults to true.
		  ,[Enabled] = 1
		  -- Specifies whether this client needs a secret to request tokens from the token endpoint (defaults to true)
		  ,[RequireClientSecret] = 1
		  -- Specifies whether clients using an authorization code based grant type must send a proof key
		  ,[RequirePkce] = 0 
		--*********************
		-- TOKEN SETTINGS
		--*********************
		  -- Specifies whether the access token is a reference token or a self contained JWT token (defaults to Jwt).
		  --	0:Reference Token
		  --	1:JWT Access Token
		  ,[AccessTokenType] = 1
		  -- Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)
		  ,[AccessTokenLifetime] = 3600
		  -- AlwaysIncludeUserClaimsInIdToken Settings
		  --	1:When requesting both an id token and access token, 
		  --	  the user claims will always be added to the id token instead of 
		  --	  requring the client to use the userinfo endpoint. 
		  --	0:Default is false.
		  ,[AlwaysIncludeUserClaimsInIdToken] = 1
		  -- AlwaysSendClientClaims Settings
		  --	1:The client claims will be sent for every flow.
		  --		Note: you can add more client claims by adding them to the UserClaims table
		  --	0:Only sent for client credentials flow (default is false)
		  ,[AlwaysSendClientClaims] = 1
		  -- Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)
		  ,[AuthorizationCodeLifetime] = 300
		  -- ClientClaimsPrefix Settings
		  --	1:The prefix client claim types will be prefixed with. Defaults to client_. 
		  --		Note: The intent is to make sure they don’t accidentally collide with user claims.
		  --	0:Don't include the prefix
		  ,[ClientClaimsPrefix] = N'client_'
		  -- Lifetime to identity token in seconds (defaults to 300 seconds / 5 minutes)
		  ,[IdentityTokenLifetime] = 300
		  -- Salt value used in pair-wise subjectId generation for users of this client.
		  ,[PairWiseSubjectSalt] = NULL
		--*********************
		-- AUTHENTICATION AND LOGOUT
		--*********************
		  -- Specifies if the user’s session id should be sent in the request to the BackChannelLogoutUri. Defaults to true.
		  ,[BackChannelLogoutSessionRequired] = 1
		  -- Specifies logout URI at client for HTTP based back-channel logout. See the OIDC Back-Channel spec for more details.
		  ,[BackChannelLogoutUri] = NULL
		--*********************
		-- CONSENT SCREEN
		--*********************
		  -- Specifies whether a consent screen is required. Defaults to true.
		  ,[RequireConsent] = 0
		  -- Specifies whether user can choose to store consent decisions. Defaults to true. 
		  ,[AllowRememberConsent] = 1
		  -- Lifetime of a user consent in seconds. Defaults to null (no expiration).
		  ,[ConsentLifetime] = NULL
		  -- Client display name (used for logging and consent screen)  
		  ,[ClientName] = N'PartnerPortal Web'
		  -- URI to further information about client (used on consent screen)
		  ,[ClientUri] = NULL
		  -- URI to client logo (used on consent screen)
		  ,[LogoUri] = NULL
		--*********************
		-- REFRESH TOKENS
		--*********************
		  -- Maximum lifetime of a refresh token in seconds. Defaults to 2592000 seconds / 30 days
		  ,[AbsoluteRefreshTokenLifetime] = 2592000
		  -- RefreshTokenExpiration Settings
		  -- 	0:Absolute - based on AbsoluteRefreshTokenLifetime
		  -- 	1:Sliding - based on SlidingRefreshTokenLifetime up to AbsoluteRefreshTokenLifetime
		  ,[RefreshTokenExpiration] = 1
		  -- RefreshTokenUsage Settings
		  --	0:ReUse - the refresh token handle will stay the same when refreshing tokens
		  --	1:OneTime - the refresh token handle will be updated when refreshing tokens
		  ,[RefreshTokenUsage] = 1
		  --Sliding lifetime of a refresh token in seconds. Defaults to 1296000 seconds / 15 days
		  ,[SlidingRefreshTokenLifetime] = 1296000
		  -- UpdateAccessTokenClaimsOnRefresh Settings
		  -- 	0:Don't Refresh Token
		  --	1:Refresh Token
		  ,[UpdateAccessTokenClaimsOnRefresh] = 1  
		--*********************
		-- LOGOUT
		--*********************
		  ,[LogoutSessionRequired] = 0
		  ,[LogoutUri] = NULL
		--*********************
		-- MISC
		--*********************
		  ,[EnableLocalLogin] = 1
		  ,[FrontChannelLogoutSessionRequired] = 1
		  ,[FrontChannelLogoutUri] = NULL	  
		  -- Specifies whether JWT access tokens should have an embedded unique ID (via the jti claim).
		  ,[IncludeJwtId] = 0
		  ,[NonEditable] = 0
		  ,[ProtocolType] = N'oidc'  
	 WHERE clientId=@clientId
  END

if (exists(select * from auth.Clients where ClientId=@clientId))
  BEGIN
    set @id = (select Id from auth.Clients where ClientId=@clientId)

    declare @scope nvarchar(200)
    set @scope =  N'partnerportal.web'

    if (not exists(select * from [AUTH].[ClientScopes] where ClientId = @id AND Scope = @scope))
      BEGIN
        INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'multidisbursement.backendapp')
      END

    --Local environment--
	declare @postLogoutRedirectUri nvarchar(200)
    set @postLogoutRedirectUri = 'http://localhost:8088/partnerportal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @postLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @postLogoutRedirectUri)
      END
	  
	declare @redirectUri nvarchar(200)
    set @redirectUri = 'http://localhost:8088/partnerportal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientRedirectUris] where ClientId = @id AND RedirectUri = @redirectUri))
      BEGIN
        INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @redirectUri)
      END
	  
	declare @origin nvarchar(200)
    set @origin = 'http://localhost:8088'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @origin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @origin)
      END

	--Another local environment
	set @postLogoutRedirectUri = 'http://127.0.0.1/partnerportal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @postLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @postLogoutRedirectUri)
      END
	  
	set @redirectUri = 'http://127.0.0.1/partnerportal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientRedirectUris] where ClientId = @id AND RedirectUri = @redirectUri))
      BEGIN
        INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @redirectUri)
      END
	  
	set @origin = 'http://127.0.0.1'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @origin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @origin)
      END

	--Internal Dev environment--
    set @postLogoutRedirectUri = 'https://portal-dev-admin.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @postLogoutRedirectUri))
      BEGIN
       INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @postLogoutRedirectUri)
      END

    set @redirectUri = 'https://portal-dev-admin.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientRedirectUris] where ClientId = @id AND RedirectUri = @redirectUri))
      BEGIN
        INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @redirectUri)
      END

    set @origin = 'https://portal-dev-admin.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @origin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @origin)
      END
	  
	--External Dev environment--
    set @postLogoutRedirectUri = 'https://portal-dev.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @postLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @postLogoutRedirectUri)
      END

    set @redirectUri = 'https://portal-dev.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientRedirectUris] where ClientId = @id AND RedirectUri = @redirectUri))
      BEGIN
        INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @redirectUri)
      END

    set @origin = 'https://portal-dev.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @origin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @origin)
      END

    --Internal Test environment--
    set @postLogoutRedirectUri = 'https://portal-test-admin.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @postLogoutRedirectUri))
      BEGIN
       INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @postLogoutRedirectUri)
      END

    set @redirectUri = 'https://portal-test-admin.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientRedirectUris] where ClientId = @id AND RedirectUri = @redirectUri))
      BEGIN
        INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @redirectUri)
      END

    set @origin = 'https://portal-test-admin.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @origin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @origin)
      END
	  
	--External Test environment--
    set @postLogoutRedirectUri = 'https://portal-test.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @postLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @postLogoutRedirectUri)
      END

    set @redirectUri = 'https://portal-test.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientRedirectUris] where ClientId = @id AND RedirectUri = @redirectUri))
      BEGIN
        INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @redirectUri)
      END

    set @origin = 'https://portal-test.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @origin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @origin)
      END

    --Internal Prod environment--
    set @postLogoutRedirectUri = 'https://portal-admin.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @postLogoutRedirectUri))
      BEGIN
       INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @postLogoutRedirectUri)
      END

    set @redirectUri = 'https://portal-admin.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientRedirectUris] where ClientId = @id AND RedirectUri = @redirectUri))
      BEGIN
        INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @redirectUri)
      END

    set @origin = 'https://portal-admin.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @origin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @origin)
      END
	  
    --External Prod environment--
    set @postLogoutRedirectUri = 'https://portal.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @postLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @postLogoutRedirectUri)
      END

    set @redirectUri = 'https://portal.cortside.com/PartnerPortal/Login.aspx'

    if (not exists(select * from [AUTH].[ClientRedirectUris] where ClientId = @id AND RedirectUri = @redirectUri))
      BEGIN
        INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @redirectUri)
      END

    set @origin = 'https://portal.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @origin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @origin)
      END
  END