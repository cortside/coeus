Declare @clientName varchar(200) ='Maintenance Web'
Declare @clientId varchar(200) ='maintenance-web'
Declare @localUrlV1 varchar(200)='http://localhost:4229'
Declare @localUrl varchar(200)='http://localhost:4200'
Declare @devUrl varchar(200)='https://maintenance.dev.cortside.com'
Declare @testUrl varchar(200)='https://maintenance.test.cortside.com'
Declare @stageUrl varchar(200)='https://maintenance.stage.cortside.com'
Declare @demoUrl varchar(200)='https://maintenance.demo.cortside.com'
Declare @integrationUrl varchar(200)='https://maintenance.integration.cortside.com'
Declare @uatUrl varchar(200)='https://maintenance.uat.cortside.com'
Declare @prodUrl varchar(200)='https://maintenance.cortside.com'
declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN

	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 1, 1, 0, 0, 1, 1, 1, 300, 1, NULL, N'client_', @clientId, @clientName, NULL, NULL, NULL, 0, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)
            
	set @id = SCOPE_IDENTITY()

	--INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, N'role', N'admin')
	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'implicit')

	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'maintenance.api')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'identity')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'groups')
  END

select @id = id from [AUTH].[Clients] where ClientId = @clientId

if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'loan-api'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'loan-api')
  END

 if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'user-api'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'user-api')
  END 

 if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'eboa.webapi'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'eboa.webapi')
  END 
  
 if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'calendar.api'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'calendar.api')
  END 

 if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'common.communications'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'common.communications')
  END 

-- setup prod environment
if not exists (select * from [AUTH].[ClientPostLogoutRedirectUris] where [PostLogoutRedirectUri] like '%'+@prodUrl+'%')
  BEGIN
	--prod environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @prodUrl + '/logout.html')
	INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @prodUrl + '/login-redirect.html')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, @prodUrl)
  END

-- setup local environment
if not exists (select * from [AUTH].[ClientPostLogoutRedirectUris] where [PostLogoutRedirectUri] like '%'+@localUrlV1+'%')
  BEGIN
	--local environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @localUrlV1 + '/logout.html')
	INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @localUrlV1 + '/login-redirect.html')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, @localUrlV1)
  END

-- setup dev environment
if not exists (select * from [AUTH].[ClientPostLogoutRedirectUris] where [PostLogoutRedirectUri] like '%'+@devUrl+'%')
  BEGIN
	--dev environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @devUrl + '/logout.html')
	INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @devUrl + '/login-redirect.html')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, @devUrl)
  END

-- setup test environment
if not exists (select * from [AUTH].[ClientPostLogoutRedirectUris] where [PostLogoutRedirectUri] like '%'+@testUrl+'%')
  BEGIN
	--test environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @testUrl + '/logout.html')
	INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @testUrl + '/login-redirect.html')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, @testUrl)
  END

-- setup stage environment
if not exists (select * from [AUTH].[ClientPostLogoutRedirectUris] where [PostLogoutRedirectUri] like '%'+@stageUrl+'%')
  BEGIN
	--stage environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @stageUrl + '/logout.html')
	INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @stageUrl + '/login-redirect.html')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, @stageUrl)
  END

-- setup demo environment
if not exists (select * from [AUTH].[ClientPostLogoutRedirectUris] where [PostLogoutRedirectUri] like '%'+@demoUrl+'%')
  BEGIN
	--demo environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @demoUrl + '/logout.html')
	INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @demoUrl + '/login-redirect.html')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, @demoUrl)
  END

-- setup integration environment
if not exists (select * from [AUTH].[ClientPostLogoutRedirectUris] where [PostLogoutRedirectUri] like '%'+@integrationUrl+'%')
  BEGIN
	--integration environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @integrationUrl + '/logout.html')
	INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @integrationUrl + '/login-redirect.html')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, @integrationUrl)
  END

-- setup uat environment
if not exists (select * from [AUTH].[ClientPostLogoutRedirectUris] where [PostLogoutRedirectUri] like '%'+@uatUrl+'%')
  BEGIN
	--uat environment
	INSERT [AUTH].[ClientPostLogoutRedirectUris] ([ClientId], [PostLogoutRedirectUri]) VALUES (@id, @uatUrl + '/logout.html')
	INSERT [AUTH].[ClientRedirectUris] ([ClientId], [RedirectUri]) VALUES (@id, @uatUrl + '/login-redirect.html')
	INSERT [AUTH].[ClientCorsOrigins] VALUES (@id, @uatUrl)
  END

-- setup allowed redirect uris
print N'setup allowed redirect uris'
declare @allowedRedirectUris table (
    Uri nvarchar(1000)
)
insert into @allowedRedirectUris values (@localUrl + '/silent-redirect')
insert into @allowedRedirectUris values (@localUrl + '/login-redirect')
insert into @allowedRedirectUris values (@devUrl + '/silent-redirect')
insert into @allowedRedirectUris values (@devUrl + '/login-redirect')
insert into @allowedRedirectUris values (@testUrl + '/silent-redirect')
insert into @allowedRedirectUris values (@testUrl + '/login-redirect')
insert into @allowedRedirectUris values (@stageUrl + '/silent-redirect')
insert into @allowedRedirectUris values (@stageUrl + '/login-redirect')
insert into @allowedRedirectUris values (@uatUrl + '/silent-redirect')
insert into @allowedRedirectUris values (@uatUrl + '/login-redirect')
insert into @allowedRedirectUris values (@demoUrl + '/silent-redirect')
insert into @allowedRedirectUris values (@demoUrl + '/login-redirect')
insert into @allowedRedirectUris values (@integrationUrl + '/silent-redirect')
insert into @allowedRedirectUris values (@integrationUrl + '/login-redirect')
insert into @allowedRedirectUris values (@prodUrl + '/silent-redirect')
insert into @allowedRedirectUris values (@prodUrl + '/login-redirect')
insert into [AUTH].[ClientRedirectUris] (ClientId, RedirectUri)
select @id, a.Uri
from @allowedRedirectUris a
where uri not in (select RedirectUri from auth.ClientRedirectUris where clientId=@id)



-- setup allowed logout redirect uris
print N'setup allowed logout redirect uris'
declare @allowedLogoutRedirectUris table (
    Uri nvarchar(1000)
)
insert into @allowedLogoutRedirectUris values (@localUrl + '/logout')
insert into @allowedLogoutRedirectUris values (@devUrl + '/logout')
insert into @allowedLogoutRedirectUris values (@testUrl + '/logout')
insert into @allowedLogoutRedirectUris values (@stageUrl + '/logout')
insert into @allowedLogoutRedirectUris values (@uatUrl + '/logout')
insert into @allowedLogoutRedirectUris values (@demoUrl + '/logout')
insert into @allowedLogoutRedirectUris values (@integrationUrl + '/logout')
insert into @allowedLogoutRedirectUris values (@prodUrl + '/logout')
insert into [AUTH].[ClientPostLogoutRedirectUris] (ClientId, PostLogoutRedirectUri)
select @id, a.Uri
from @allowedLogoutRedirectUris a
where uri not in (select PostLogoutRedirectUri from auth.ClientPostLogoutRedirectUris where clientId=@id)



-- setup allowed cors origins
print N'setup allowed cors origins'
declare @allowedCorsOrigins table (
    Uri nvarchar(1000)
)
insert into @allowedCorsOrigins values (@localUrl)
insert into @allowedCorsOrigins values (@devUrl)
insert into @allowedCorsOrigins values (@testUrl)
insert into @allowedCorsOrigins values (@stageUrl)
insert into @allowedCorsOrigins values (@uatUrl)
insert into @allowedCorsOrigins values (@demoUrl)
insert into @allowedCorsOrigins values (@integrationUrl)
insert into @allowedCorsOrigins values (@prodUrl)
insert into [AUTH].[ClientCorsOrigins] (ClientId, Origin)
select @id, a.Uri
from @allowedCorsOrigins a
where uri not in (select Origin from auth.ClientCorsOrigins where clientId=@id)
