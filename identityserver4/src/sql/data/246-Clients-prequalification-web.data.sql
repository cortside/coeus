Declare @clientName varchar(200) ='Prequalification Web Anonymous User'
Declare @clientId varchar(200) ='prequalification-web'
Declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN

	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, N'', @clientId, @clientName, NULL, NULL, NULL, 0, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

  END

SELECT @id = id from [AUTH].[Clients] where ClientId = @clientId

if (not exists(select * from auth.ClientSecrets where clientid = @id))
  BEGIN  
  -- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- Generate the [Value] using the password in https://dotnetfiddle.net/h3aeqd
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'invalid-hash=')
  END

if (not exists(select * from auth.ClientGrantTypes where clientid = @id and GrantType = 'recaptcha'))
  BEGIN
	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'recaptcha')
  END
  
if (not exists(select * from auth.ClientScopes where clientid = @id and scope = 'prequalification-api'))
  BEGIN
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'prequalification-api')
  END
  
if (not exists(select * from auth.ClientClaims where clientid = @id and Type='sub' and Value = '51baf152-bf40-4da4-ae25-8314282c7d1a'))
  BEGIN
  	insert into auth.ClientClaims values(@id, 'sub', '51baf152-bf40-4da4-ae25-8314282c7d1a')
  END

if (not exists(select * from auth.Clients where Id = @id and AccessTokenType = 1))
  BEGIN
    UPDATE [AUTH].[Clients] SET AccessTokenType = 1 where Id = @id;
  END
 
if (not exists(select * from auth.Clients where Id = @id and ClientClaimsPrefix = ''))
  BEGIN
    UPDATE [AUTH].[Clients] SET ClientClaimsPrefix = '' where Id = @id;
  END



-- clean up any duplicate grants this script was previously creating
declare @recaptchaGrantCount int = (select count(1) from auth.ClientGrantTypes cc
join auth.clients c on c.id = cc.ClientId
where cc.GrantType = 'recaptcha'
and c.ClientId = @clientId) 

if (@recaptchaGrantCount > 1)
begin
 delete top (@recaptchaGrantCount - 1) cc
	from auth.ClientGrantTypes cc
	join auth.clients c on c.id = cc.ClientId
	where cc.GrantType = 'recaptcha'
	and c.ClientId = @clientId
end