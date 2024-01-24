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
  END

select @id=id from auth.clients where clientId=@clientId

UPDATE [AUTH].[Clients] SET AccessTokenType = 1 WHERE Id = @id
update auth.clients set EnableLocalLogin=1 where id=@id


  --setup desired scopes
print N'setup desired scopes'
declare @grantedScopes table (
    Scope nvarchar(200)
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


declare @uris table (
    Uri nvarchar(200)
)
insert into @uris values (N'http://localhost:4200')
insert into @uris values (N'http://kehlstein:5003')
insert into @uris values (N'http://kehlstein:5004')
insert into @uris values (N'https://shoppingcart.cortside.net')
insert into @uris values (N'https://status.dev.acme.com')
insert into @uris values (N'https://status.acme.com')

INSERT [AUTH].[ClientCorsOrigins] ([ClientId], [Origin])
select @id, a.Uri
from @uris a
where uri not in (select origin from auth.ClientCorsOrigins where clientId=@id)


DELETE FROM @uris
insert into @uris values (N'http://localhost:4200/logout')
insert into @uris values (N'http://kehlstein:5003/logout')
insert into @uris values (N'http://kehlstein:5004/logout')
insert into @uris values (N'https://shoppingcart.cortside.net/logout')
insert into @uris values (N'https://status.dev.acme.com/logout')
insert into @uris values (N'https://status.acme.com/logout')

INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri])
select @id, a.Uri
from @uris a
where uri not in (select PostLogoutRedirectUri from auth.ClientPostLogoutRedirectUris where clientId=@id)

-- setup allowed redirect uris
print N'setup allowed redirect uris'
declare @allowedRedirectUris table (
    Uri nvarchar(1000)
)
insert into @allowedRedirectUris values (N'http://localhost:4200/login-redirect')
insert into @allowedRedirectUris values (N'http://localhost:4200/silent-redirect')
insert into @allowedRedirectUris values (N'http://kehlstein:5004/login-redirect')
insert into @allowedRedirectUris values (N'http://kehlstein:5004/silent-redirect')
insert into @allowedRedirectUris values (N'https://shoppingcart.cortside.net/login-redirect')
insert into @allowedRedirectUris values (N'https://shoppingcart.cortside.net/silent-redirect')
insert into @allowedRedirectUris values (N'https://shoppingcart.dev.acme.com/login-redirect')
insert into @allowedRedirectUris values (N'https://shoppingcart.dev.acme.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://shoppingcart.acme.com/login-redirect')
insert into @allowedRedirectUris values (N'https://shoppingcart.acme.com/silent-redirect')
insert into [AUTH].[ClientRedirectUris] (ClientId, RedirectUri)
select @id, a.Uri
from @allowedRedirectUris a
where uri not in (select RedirectUri from auth.ClientRedirectUris where clientId=@id)
