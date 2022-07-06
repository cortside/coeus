declare @clientId nvarchar(400)
set @clientId='fms-web'
declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN
	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, N'client_', N'fms-web', N'Funding Management Web', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	--INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, N'role', N'admin')
	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'implicit')

	--Local environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'http://localhost:4200/logout')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'http://localhost:4200')

	--Develop environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://fms.dev.cortside.com/logout')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://fms.dev.cortside.com')

	--Test environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://fms.test.cortside.com/logout')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://fms.test.cortside.com')

	--Staging environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://fms.stage.cortside.com/logout')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://fms.stage.cortside.com')

	--Demo environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://fms.demo.cortside.com/logout')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://fms.demo.cortside.com')

	--Integration environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://fms.integration.cortside.com/logout')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://fms.integration.cortside.com')

	--UAT environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://fms.uat.cortside.com/logout')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://fms.uat.cortside.com')
	
	--Production environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://fms.cortside.com/logout')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://fms.cortside.com')

  END

if (exists(select * from auth.Clients where ClientId=@clientId))
  BEGIN

    set @id = (select Id from auth.Clients where ClientId=@clientId)

    if (exists(select * from [AUTH].[Clients] where Id = @id AND ClientId = @clientId AND AccessTokenType = 0))
      BEGIN
        UPDATE [AUTH].[Clients]
        SET AccessTokenType = 1
        WHERE Id = @id AND ClientId = @clientId AND AccessTokenType = 0
      END

    declare @scope nvarchar(200)
    set @scope =  N'user-api'
    if (not exists(select * from [AUTH].[ClientScopes] where ClientId = @id AND Scope = @scope))
        BEGIN
          INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, @scope)
        END
        
    --Test environment--

    declare @testPostLogoutRedirectUri nvarchar(200)
    set @testPostLogoutRedirectUri = 'https://fms.test.cortside.com/logout'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @testPostLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @testPostLogoutRedirectUri)
      END

    declare @origin nvarchar(200)
    set @origin = 'https://fms.test.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @origin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @origin)
      END

    --Staging environment--

    declare @stagingPostLogoutRedirectUri nvarchar(200)
    set @stagingPostLogoutRedirectUri = 'https://fms.stage.cortside.com/logout'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @stagingPostLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @stagingPostLogoutRedirectUri)
      END

    declare @stagingOrigin nvarchar(200)
    set @stagingOrigin = 'https://fms.stage.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @stagingOrigin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @stagingOrigin)
      END

    --Demo environment--

    declare @demoPostLogoutRedirectUri nvarchar(200)
    set @demoPostLogoutRedirectUri = 'https://fms.demo.cortside.com/logout'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @demoPostLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @demoPostLogoutRedirectUri)
      END

    declare @demoOrigin nvarchar(200)
    set @demoOrigin = 'https://fms.demo.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @demoOrigin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @demoOrigin)
      END

    --Integration environment--

    declare @integrationPostLogoutRedirectUri nvarchar(200)
    set @integrationPostLogoutRedirectUri = 'https://fms.integration.cortside.com/logout'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @integrationPostLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @integrationPostLogoutRedirectUri)
      END

    declare @integrationOrigin nvarchar(200)
    set @integrationOrigin = 'https://fms.integration.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @integrationOrigin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @integrationOrigin)
      END

    --UAT environment--

    declare @uatPostLogoutRedirectUri nvarchar(200)
    set @uatPostLogoutRedirectUri = 'https://fms.uat.cortside.com/logout'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @uatPostLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @uatPostLogoutRedirectUri)
      END

    declare @uatOrigin nvarchar(200)
    set @uatOrigin = 'https://fms.uat.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @uatOrigin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @uatOrigin)
      END

    --Production environment--

    declare @productionPostLogoutRedirectUri nvarchar(200)
    set @productionPostLogoutRedirectUri = 'https://fms.cortside.com/logout'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @productionPostLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @productionPostLogoutRedirectUri)
      END

    declare @productionOrigin nvarchar(200)
    set @productionOrigin = 'https://fms.cortside.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @productionOrigin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @productionOrigin)
      END  
  END

  --setup desired scopes
print N'setup desired scopes'
declare @grantedScopes table (
    Scope nvarchar(100)
)
insert into @grantedScopes values (N'openid')
insert into @grantedScopes values (N'profile')
insert into @grantedScopes values (N'role')
insert into @grantedScopes values (N'fundingmanagement.api')
insert into @grantedScopes values (N'multidisbursement.api')
insert into @grantedScopes values (N'multidisbursement.backendapp')
insert into [AUTH].[ClientScopes] (ClientId, Scope)
select @id, a.Scope
from @grantedScopes a
where Scope not in (select Scope from auth.ClientScopes where clientId=@id)

select @id=id from auth.clients where clientId=@clientId
-- disable the local login option
print N'disable the local login option'
update auth.Clients set EnableLocalLogin=0 where EnableLocalLogin=1 and Id = @id

-- setup allowed redirect uris
print N'setup allowed redirect uris'
declare @allowedRedirectUris table (
    Uri nvarchar(1000)
)
insert into @allowedRedirectUris values (N'http://localhost:4200/login-redirect')
insert into @allowedRedirectUris values (N'http://localhost:4200/silent-redirect')
insert into @allowedRedirectUris values (N'https://fms.dev.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://fms.dev.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://fms.test.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://fms.test.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://fms.stage.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://fms.stage.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://fms.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://fms.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://fms.uat.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://fms.uat.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://fms.demo.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://fms.demo.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://fms.integration.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://fms.integration.cortside.com/silent-redirect')
insert into [AUTH].[ClientRedirectUris] (ClientId, RedirectUri)
select @id, a.Uri
from @allowedRedirectUris a
where uri not in (select RedirectUri from auth.ClientRedirectUris where clientId=@id)