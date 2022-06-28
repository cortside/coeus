declare @clientId nvarchar(400)
set @clientId='fms-client-dummy'

declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN

	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', N'fms-client-dummy', N'FundingManagement Client Dummy User', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'fundingmanagement.api')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'eboa.webapi')
	INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'admin');

	-- This role is designated for FMS API as a value that is whitelisted in OA in order to access some endpoints in it
	INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'fms-api-oa-access');

	-- Role for accessing Loan Servicing/Loans API. Note the role is the same string value as the scope.
	INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'loan-api');

	-- This group is "OA Test Lenders", required for access to some other OA endpoints
	INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'groups', '11c24487-a582-4738-866d-f652a978a445'); 

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: fmspassdummy
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'invalid-hash=')

  END

declare @entryType nvarchar(50)
set @entryType='sub'

if (exists(SELECT * FROM [AUTH].[Clients] WHERE ClientId=@clientId))
	BEGIN
		set @id = (SELECT Id FROM [AUTH].[Clients] WHERE ClientId=@clientId)

		-- Switching to new subject ID
		UPDATE [AUTH].[ClientClaims]
		SET Value = 'c28804b6-250f-4d6a-ac6b-f3428f3a7c3a'
		WHERE ClientId = @id AND Type = @entryType AND Value = '36C90FC9-496C-40CD-87D9-23DC3F79AE2C'

		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type='sub'))
			BEGIN
  				INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'sub', 'c28804b6-250f-4d6a-ac6b-f3428f3a7c3a');
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'multidisbursement.api'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'multidisbursement.api')
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'common.communications'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'common.communications')
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'user-api'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'user-api')
			END

		----- Access to Loans/LoanServicing API -----
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'loan-api'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'loan-api')
			END

		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type='role' AND Value=N'loan-api'))
			BEGIN
                INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'loan-api');
			END

		-----

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'calendar.api'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'calendar.api')
			END
			
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'policyserver.runtime'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.runtime')
			END	

		UPDATE [AUTH].[Clients]
		SET AccessTokenLifetime = 259200 --Three days
		WHERE Id = @id AND ClientId = @clientId

		
		----- Access to OnlineApplication (OA)
		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'role' AND Value=N'fms-api-oa-access'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'fms-api-oa-access');
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'eboa.webapi'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'eboa.webapi')
			END

		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'groups' AND Value=N'11c24487-a582-4738-866d-f652a978a445'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'groups', '11c24487-a582-4738-866d-f652a978a445');
			END
	END

