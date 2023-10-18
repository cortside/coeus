declare @clientId nvarchar(400)
set @clientId='shoppingcart-web'
declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN
	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, N'client_', @clientId, N'ShoppingCart Web', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	--INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, N'role', N'admin')
	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'implicit')

	--Local environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'http://localhost:4200/logout')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'http://localhost:4200')

	--Develop environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://shoppingcart.dev.acme.com/logout')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://shoppingcart.dev.acme.com')

	--Production environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://shoppingcart.acme.com/logout')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, 'https://shoppingcart.acme.com')
  END

-- ClientCorsOrigins


if (exists(select * from auth.Clients where ClientId=@clientId))
  BEGIN
    set @id = (select Id from auth.Clients where ClientId=@clientId)

    if (exists(select * from [AUTH].[Clients] where Id = @id AND ClientId = @clientId AND AccessTokenType = 0))
      BEGIN
        UPDATE [AUTH].[Clients]
        SET AccessTokenType = 1
        WHERE Id = @id AND ClientId = @clientId AND AccessTokenType = 0
      END

    --Test environment--

    declare @testPostLogoutRedirectUri nvarchar(200)
    set @testPostLogoutRedirectUri = 'https://shoppingcart.test.acme.com/logout'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @testPostLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @testPostLogoutRedirectUri)
      END

    declare @origin nvarchar(200)
    set @origin = 'https://shoppingcart.test.acme.com'

    if (not exists(select * from [AUTH].[ClientCorsOrigins] where ClientId = @id AND Origin = @origin))
      BEGIN
        INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin]) VALUES (@id, @origin)
      END

    --Production environment--

    declare @productionPostLogoutRedirectUri nvarchar(200)
    set @productionPostLogoutRedirectUri = 'https://shoppingcart.acme.com/logout'

    if (not exists(select * from [AUTH].[ClientPostLogoutRedirectUris] where ClientId = @id AND PostLogoutRedirectUri = @productionPostLogoutRedirectUri))
      BEGIN
        INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @productionPostLogoutRedirectUri)
      END

    declare @productionOrigin nvarchar(200)
    set @productionOrigin = 'https://shoppingcart.acme.com'

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
insert into @grantedScopes values (N'shoppingcart-api')
insert into @grantedScopes values (N'catalog-api')

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
insert into @allowedRedirectUris values (N'https://shoppingcart.dev.acme.com/login-redirect')
insert into @allowedRedirectUris values (N'https://shoppingcart.dev.acme.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://shoppingcart.acme.com/login-redirect')
insert into @allowedRedirectUris values (N'https://shoppingcart.acme.com/silent-redirect')
insert into [AUTH].[ClientRedirectUris] (ClientId, RedirectUri)
select @id, a.Uri
from @allowedRedirectUris a
where uri not in (select RedirectUri from auth.ClientRedirectUris where clientId=@id)


update auth.clients set EnableLocalLogin=1 where id=@id