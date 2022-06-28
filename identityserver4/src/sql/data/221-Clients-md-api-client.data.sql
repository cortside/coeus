declare @clientId nvarchar(400)
set @clientId='md-api-client'

declare @id int

if (not exists(select * from auth.Clients where clientId=@clientId))
  BEGIN

	INSERT [AUTH].[Clients] ([AbsoluteRefreshTokenLifetime], [AccessTokenLifetime], [AccessTokenType], [AllowAccessTokensViaBrowser], [AllowOfflineAccess], [AllowPlainTextPkce], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [AlwaysSendClientClaims], [AuthorizationCodeLifetime], [BackChannelLogoutSessionRequired], [BackChannelLogoutUri], [ClientClaimsPrefix], [ClientId], [ClientName], [ClientUri], [ConsentLifetime], [Description], [EnableLocalLogin], [Enabled], [FrontChannelLogoutSessionRequired], [FrontChannelLogoutUri], [IdentityTokenLifetime], [IncludeJwtId], [LogoUri], [PairWiseSubjectSalt], [LogoutSessionRequired], [LogoutUri], [NonEditable], [ProtocolType], [RefreshTokenExpiration], [RefreshTokenUsage], [RequireClientSecret], [RequireConsent], [RequirePkce], [SlidingRefreshTokenLifetime], [UpdateAccessTokenClaimsOnRefresh]) 
		VALUES (2592000, 3600, 0, 1, 0, 0, 1, 1, 1, 300, 1, NULL, '', @clientId, N'Multidisbursement API Client', NULL, NULL, NULL, 1, 1, 1, NULL, 300, 0, NULL, NULL, 0, NULL, 0, N'oidc', 1, 1, 1, 0, 0, 1296000, 0)

	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'client_credentials')

	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'openid')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'profile')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'role')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'multidisbursement.api')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'common.communications')
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'loan-api')
    INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'eboa.webapi')

	INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'admin');
	INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'sub', '08103249-bbeb-4e47-bedd-e3a7e7b624b1');

	-- This role is designated for MD API as a value that is whitelisted in OA in order to access some endpoints in it
	INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'md-api-oa-access');

	-- Role for accessing Loan Servicing/Loans API. Note the role is the same string value as the scope.
	INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'loan-api');

	-- This group is "OA Test Lenders", required for access to some other OA endpoints
	INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'groups', '11c24487-a582-4738-866d-f652a978a445');

	-- assigns a password with which the client (using client credentials grant, hence requiring a password) can use to authenticate
	-- password: mdapipass
	-- Generate the [Value] using the password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ClientSecrets] ([ClientId], [Description], [Expiration], [Type], [Value]) VALUES (@id, NULL, NULL, N'SharedSecret', N'invalid-hash=')

  END

if (exists(SELECT * FROM [AUTH].[Clients] WHERE ClientId=@clientId))
	BEGIN
		set @id = (SELECT Id FROM [AUTH].[Clients] WHERE ClientId=@clientId)

		UPDATE [AUTH].[Clients]
		SET AccessTokenLifetime = 259200 --Three days
		WHERE Id = @id AND ClientId = @clientId
		
		-- Switching to new subject ID
		UPDATE [AUTH].[ClientClaims]
		SET Value = '08103249-bbeb-4e47-bedd-e3a7e7b624b1'
		WHERE ClientId = @id AND Type = 'sub' AND Value = '36C90FC9-496C-40CD-87D9-23DC3F79AE2C'

		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type='sub'))
			BEGIN
  				INSERT [Auth].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'sub', '08103249-bbeb-4e47-bedd-e3a7e7b624b1');
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'user-api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'user-api')
			END
		
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'datawarehouse-api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'datawarehouse-api')
			END
			
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'policyserver.runtime'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.runtime')
			END
		
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'policyserver.management'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'policyserver.management')
			END	

        if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'maintenance.api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'maintenance.api')
			END		

		----- simulate internal EB admin user by setting Identity Provider as Azure Active Directory -----

		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'idp' AND Value=N'AAD'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'idp', 'AAD');
			END

		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'family_name' AND Value=N'MdApiClient'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'family_name', 'MdApiClient');
			END

		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'given_name' AND Value=N'Mac'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'given_name', 'Mac');
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'calendar.api'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'calendar.api')
			END

		----- Access to Loan Servicing/Loans API
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'loan-api'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'loan-api')
			END

		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type='role' AND Value=N'loan-api'))
			BEGIN
                INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'loan-api');
			END

        ----- Access to OnlineApplication (OA)
		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'role' AND Value=N'md-api-oa-access'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'md-api-oa-access');
			END

		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'eboa.webapi'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'eboa.webapi')
			END

		----- Access to Documents API -----
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'document-api'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'document-api')
			END
		
		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'role' AND Value=N'document-api'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'document-api');
			END

		----- Also needed for access to OnlineApplication and Documents API -----
		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'groups' AND Value=N'11c24487-a582-4738-866d-f652a978a445'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'groups', '11c24487-a582-4738-866d-f652a978a445');
			END

		----- Access to Funding Management API -----
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'fundingmanagement.api'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'fundingmanagement.api')
			END
		
		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'role' AND Value=N'fundingmanagement.api'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'fundingmanagement.api');
			END

		----- Access to Product API -----
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'maintenance.api'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'maintenance.api')
			END
		
		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'role' AND Value=N'maintenance.api'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'maintenance.api');
			END

		----- Access to Rebate API -----
		if (not exists(SELECT * FROM [AUTH].[ClientScopes] where ClientId=@id AND Scope=N'rebate.api'))
			BEGIN
                INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'rebate.api')
			END
		
		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'role' AND Value=N'rebate.api'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', 'rebate.api');
			END

		----- Access to PreQualification API -----
		if (not exists(select * from auth.ClientScopes where clientid = @id and scope = N'prequalification-api'))
			BEGIN
				INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'prequalification-api')
			END

		if (not exists(SELECT * FROM [AUTH].[ClientClaims] where ClientId=@id AND Type=N'role' AND Value=N'prequalification-api'))
			BEGIN
				INSERT [AUTH].[ClientClaims] ([ClientId], [Type], [Value]) VALUES (@id, 'role', N'prequalification-api');
			END
			
		if (not exists(SELECT * FROM [AUTH].[ClientGrantTypes] where ClientId=@id AND GrantType=N'delegation'))
			BEGIN
                INSERT [AUTH].[ClientGrantTypes] ([ClientId], [GrantType]) VALUES (@id, N'delegation')
			END		
	END

select @id=id from auth.clients where clientId=@clientId

-- remove name claim, both unneeded and duplicated -- causing issue with policyserver
delete from auth.ClientClaims where clientId=@id and type='name'
