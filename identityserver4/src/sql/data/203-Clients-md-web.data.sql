declare @id int
declare @clientId nvarchar(400)
set @clientId='md-web'

IF (NOT EXISTS(SELECT * FROM auth.Clients WHERE clientId=@clientId))
  BEGIN
    INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
      VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, N'client_', N'md-web', N'Multidisbursement Web', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)
    set @id = SCOPE_IDENTITY()
  END

IF (EXISTS(SELECT * FROM auth.Clients WHERE ClientId=@clientId))
  BEGIN
    SET @id = (SELECT Id FROM auth.Clients WHERE ClientId=@clientId)
    -- update token type
    IF (EXISTS(SELECT * FROM [AUTH].[Clients] WHERE Id = @id AND ClientId = @clientId AND AccessTokenType = 0))
      BEGIN
        UPDATE [AUTH].[Clients]
        SET AccessTokenType = 1
        WHERE Id = @id AND ClientId = @clientId AND AccessTokenType = 0
      END

    --Dev environment revision--
       -- Remove undesired CORS
    DECLARE @oldOrigins TABLE( Origin NVarchar(1000) )
    INSERT INTO @oldOrigins VALUES (N'https://md.dev.cortside.com')
    INSERT INTO @oldOrigins VALUES (N'https://md.test.cortside.com')
    INSERT INTO @oldOrigins VALUES (N'https://md.demo.cortside.com')
    INSERT INTO @oldOrigins VALUES (N'https://md.stage.cortside.com')
    INSERT INTO @oldOrigins VALUES (N'https://md.integration.cortside.com')
    INSERT INTO @oldOrigins VALUES (N'https://md.uat.cortside.com')
    INSERT INTO @oldOrigins VALUES (N'https://md.cortside.com')
    IF (EXISTS(SELECT * FROM [AUTH].[ClientCorsOrigins] WHERE ClientId = @id AND Origin IN (Select Origin from @oldOrigins)))
      BEGIN
        DELETE [AUTH].[ClientCorsOrigins]
        WHERE ClientId = @id AND Origin IN (Select Origin from @oldOrigins)
      END

    -- Remove redirect uris
    DECLARE @oldRedirectUris TABLE ( Uri NVARCHAR(1000) )
    INSERT INTO @oldRedirectUris VALUES (N'https://md.dev.cortside.com/login-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.dev.cortside.com/silent-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.test.cortside.com/login-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.test.cortside.com/silent-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.demo.cortside.com/login-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.demo.cortside.com/silent-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.stage.cortside.com/login-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.stage.cortside.com/silent-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.integration.cortside.com/login-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.integration.cortside.com/silent-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.uat.cortside.com/login-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.uat.cortside.com/silent-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.cortside.com/login-redirect')
    INSERT INTO @oldRedirectUris VALUES (N'https://md.cortside.com/silent-redirect')
    IF (EXISTS(SELECT * FROM [AUTH].[ClientRedirectUris] WHERE ClientId = @id AND RedirectUri IN (SELECT Uri FROM @oldRedirectUris) ))
      BEGIN
        DELETE [AUTH].[ClientRedirectUris]
        WHERE ClientId = @id AND RedirectUri IN (SELECT Uri FROM @oldRedirectUris)
      END

  --  Remove post logout redirect uris
  DECLARE @oldPostLogoutRedirectUris TABLE(Uri NVARCHAR(1000)) 
    INSERT INTO @oldPostLogoutRedirectUris VALUES (N'https://md.dev.cortside.com/logout')
    INSERT INTO @oldPostLogoutRedirectUris VALUES (N'https://md.test.cortside.com/logout')
    INSERT INTO @oldPostLogoutRedirectUris VALUES (N'https://md.demo.cortside.com/logout')
    INSERT INTO @oldPostLogoutRedirectUris VALUES (N'https://md.stage.cortside.com/logout')
    INSERT INTO @oldPostLogoutRedirectUris VALUES (N'https://md.integration.cortside.com/logout')
    INSERT INTO @oldPostLogoutRedirectUris VALUES (N'https://md.uat.cortside.com/logout')
    INSERT INTO @oldPostLogoutRedirectUris VALUES (N'https://md.cortside.com/logout')
    IF (EXISTS(SELECT * FROM [AUTH].[ClientPostLogoutRedirectUris] WHERE ClientId = @id AND PostLogoutRedirectUri IN (SELECT Uri FROM @oldPostLogoutRedirectUris) ))
      BEGIN
        DELETE [AUTH].[ClientPostLogoutRedirectUris]
        WHERE ClientId = @id AND PostLogoutRedirectUri IN (SELECT Uri FROM @oldPostLogoutRedirectUris)
      END
  END



-- setup allowed scopes
declare @allowedScopes table (
    Scope nvarchar(1000)
)
insert into @allowedScopes values (N'openid')
insert into @allowedScopes values (N'profile')
insert into @allowedScopes values (N'role')
insert into @allowedScopes values (N'multidisbursement.api')
insert into @allowedScopes values (N'multidisbursement.backendapp')
insert into @allowedScopes values (N'datawarehouse-api')
insert into @allowedScopes values (N'user-api')
insert into @allowedScopes values (N'rebate.api')
insert into @allowedScopes values (N'prequalification-api')
insert into [Auth].ClientScopes (ClientId, Scope)
select @id, a.Scope
from @allowedScopes a
where a.Scope not in (select Scope from [Auth].[ClientScopes] where ClientId = @id and Scope = a.Scope)

-- setup allowed redirect uris
print N'setup allowed redirect uris'
declare @allowedRedirectUris table (
    Uri nvarchar(1000)
)
insert into @allowedRedirectUris values (N'http://localhost:4200/login-redirect')
insert into @allowedRedirectUris values (N'http://localhost:4200/silent-redirect')
insert into @allowedRedirectUris values (N'http://localhost:4199/login-redirect')
insert into @allowedRedirectUris values (N'http://localhost:4199/silent-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.dev.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.dev.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.test.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.test.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.demo.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.demo.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.stage.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.stage.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.integration.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.integration.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.uat.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.uat.cortside.com/silent-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.cortside.com/login-redirect')
insert into @allowedRedirectUris values (N'https://partnerportal.cortside.com/silent-redirect')
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
insert into @allowedCors values (N'http://localhost:4199')
insert into @allowedCors values (N'https://partnerportal.dev.cortside.com')
insert into @allowedCors values (N'https://partnerportal.test.cortside.com')
insert into @allowedCors values (N'https://partnerportal.demo.cortside.com')
insert into @allowedCors values (N'https://partnerportal.stage.cortside.com')
insert into @allowedCors values (N'https://partnerportal.integration.cortside.com')
insert into @allowedCors values (N'https://partnerportal.uat.cortside.com')
insert into @allowedCors values (N'https://partnerportal.cortside.com')

insert into [AUTH].[ClientCorsOrigins] (ClientId, Origin)
select @id, a.Origin
from @allowedCors a
where origin not in (select Origin from auth.ClientCorsOrigins where clientId=@id)

--setup post logout redirect uris
print N'setup post logout redirect uris'
declare @allowedPostLogoutRedirectUris table (
    RedirectLogout nvarchar(1000)
)
insert into @allowedPostLogoutRedirectUris values (N'http://localhost:4200/logout')
insert into @allowedPostLogoutRedirectUris values (N'https://partnerportal.dev.cortside.com/logout')
insert into @allowedPostLogoutRedirectUris values (N'https://partnerportal.test.cortside.com/logout')
insert into @allowedPostLogoutRedirectUris values (N'https://partnerportal.demo.cortside.com/logout')
insert into @allowedPostLogoutRedirectUris values (N'https://partnerportal.stage.cortside.com/logout')
insert into @allowedPostLogoutRedirectUris values (N'https://partnerportal.integration.cortside.com/logout')
insert into @allowedPostLogoutRedirectUris values (N'https://partnerportal.uat.cortside.com/logout')
insert into @allowedPostLogoutRedirectUris values (N'https://partnerportal.cortside.com/logout')
insert into [AUTH].[ClientPostLogoutRedirectUris] (ClientId, PostLogoutRedirectUri)
select @id, a.RedirectLogout
from @allowedPostLogoutRedirectUris a
where RedirectLogout not in (select PostLogoutRedirectUri from auth.[ClientPostLogoutRedirectUris] where clientId=@id)
