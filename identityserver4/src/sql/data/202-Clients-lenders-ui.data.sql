-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://approsto.com/sha-generator/
declare @clientId nvarchar(400), @id int
set @clientId='lenders-ui'
if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN
    INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
        VALUES (2592000, 3600, 1, 1, 0, 0, 1, 1, 1, 300, 1, NULL, N'client_', @clientId, N'Lenders UI', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)
    set @id = SCOPE_IDENTITY()
    INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'implicit')
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'http://localhost:4200')
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://lenderapp.dev.cortside.com')
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://lenderapp.test.cortside.com')
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://lenderapp.stage.cortside.com')
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://lenderapp.cortside.com')
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://lenderapp.uat.cortside.com')
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://lenderapp.demo.cortside.com')
    INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, N'https://lenderapp.integration.cortside.com')
  END
select @id=id from auth.clients where clientId=@clientId
-- disable the local login option
print N'disable the local login option'
update auth.Clients set EnableLocalLogin=0 where EnableLocalLogin=1 and Id = @id
-- use reference token
print N'use reference token'
update auth.Clients set AccessTokenType = 1 where Id = @id
-- setup allowed scopes
print N'setup allowed scopes'
declare @allowedScopes table (
    Scope nvarchar(100)
)
insert into @allowedScopes values (N'openid')
insert into @allowedScopes values (N'profile')
insert into @allowedScopes values (N'role')
insert into @allowedScopes values (N'identity')
insert into @allowedScopes values (N'eboa.webapi')
insert into @allowedScopes values (N'groups')
insert into @allowedScopes values (N'loan-api')
insert into @allowedScopes values (N'document-api')
insert into @allowedScopes values (N'applicationreview-api')
insert into @allowedScopes values (N'prequalification-api')
insert into @allowedScopes values (N'decisionengine-api')
insert into @allowedScopes values (N'creditreports.api')
insert into @allowedScopes values (N'common.communications')
insert into [Auth].ClientScopes (ClientId, Scope)
select @id, a.Scope
from @allowedScopes a
where a.Scope not in (select Scope from [Auth].[ClientScopes] where ClientId = @id and Scope = a.Scope)

-- setup allowed redirect uris
print N'setup allowed redirect uris'
declare @allowedRedirectUris table (
    Uri nvarchar(1000)
)
insert into @allowedRedirectUris values (N'http://localhost:4200/auth-callback')
insert into @allowedRedirectUris values (N'http://localhost:4200/silent-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.dev.cortside.com/auth-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.dev.cortside.com/silent-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.test.cortside.com/auth-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.test.cortside.com/silent-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.stage.cortside.com/auth-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.stage.cortside.com/silent-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.cortside.com/auth-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.cortside.com/silent-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.uat.cortside.com/auth-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.uat.cortside.com/silent-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.demo.cortside.com/auth-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.demo.cortside.com/silent-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.integration.cortside.com/auth-callback')
insert into @allowedRedirectUris values (N'https://lenderapp.integration.cortside.com/silent-callback')
insert into [AUTH].[ClientRedirectUris] (ClientId, RedirectUri)
select @id, a.Uri
from @allowedRedirectUris a
where uri not in (select RedirectUri from auth.ClientRedirectUris where clientId=@id)
 
--setup allowed client cors
print N'setup allowed client cors'
declare @allowedCors table (
    Origin nvarchar(1000)
)
insert into @allowedCors values (N'http://localhost:4200')
insert into @allowedCors values (N'https://lenderapp.dev.cortside.com')
insert into @allowedCors values (N'https://lenderapp.test.cortside.com')
insert into @allowedCors values (N'https://lenderapp.stage.cortside.com')
insert into @allowedCors values (N'https://lenderapp.cortside.com')
insert into @allowedCors values (N'https://lenderapp.uat.cortside.com')
insert into @allowedCors values (N'https://lenderapp.demo.cortside.com')
insert into @allowedCors values (N'https://lenderapp.integration.cortside.com')
insert into [AUTH].[ClientCorsOrigins] (ClientId, Origin)
select @id, a.Origin
from @allowedCors a
where origin not in (select Origin from auth.ClientCorsOrigins where clientId=@id)

-- set the AccessTokenLifetime to 1 hour if it's not already
update auth.Clients set AccessTokenLifetime=3600 where Id=@id and AccessTokenLifetime!=3600
