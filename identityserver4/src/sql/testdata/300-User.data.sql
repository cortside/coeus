-- passwords are SHA512 hash base64 endcoded
-- online hash generator
-- https://dotnetfiddle.net/h3aeqd
declare @adminSubjectId nvarchar(100)
set @adminSubjectId = N'46f6eecf-e483-4428-a2a9-2bc5f6ce62db'

if (not exists(select * from auth.[user]))
  BEGIN
	-- test
	INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) VALUES (N'1770d5f4-319d-41a2-8673-8a48d8b9b300', N'Active', N'system', N'secret==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'1770d5f4-319d-41a2-8673-8a48d8b9b300', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'1770d5f4-319d-41a2-8673-8a48d8b9b300', NULL, NULL)
	INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) VALUES (@adminSubjectId, N'Active', N'admin', N'secret==', N'', 0, NULL, NULL, CAST(N'2017-08-04T22:08:41.080' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T22:08:39.957' AS DateTime), CAST(N'2017-08-04T22:08:41.080' AS DateTime), N'1770d5f4-319d-41a2-8673-8a48d8b9b300', CAST(N'2017-08-04T22:08:41.080' AS DateTime), N'1770d5f4-319d-41a2-8673-8a48d8b9b300', NULL, NULL)
	INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) VALUES (N'26cd64b9-e1d1-402d-b1c8-219aec0472d6', N'Active', N'test', N'secret==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'1770d5f4-319d-41a2-8673-8a48d8b9b300', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'1770d5f4-319d-41a2-8673-8a48d8b9b300', NULL, NULL)

	SET IDENTITY_INSERT [AUTH].[Role] ON 
	INSERT [AUTH].[Role] ([RoleId], [Name], [Description], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId]) VALUES (1, N'test', N'test', CAST(N'2017-08-17T18:21:12.353' AS DateTime), N'1770d5f4-319d-41a2-8673-8a48d8b9b300', CAST(N'2017-08-17T18:21:12.353' AS DateTime), N'1770d5f4-319d-41a2-8673-8a48d8b9b300')
	INSERT [AUTH].[Role] ([RoleId], [Name], [Description], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId]) VALUES (3, N'admin', N'admin', CAST(N'2017-08-17T18:21:12.353' AS DateTime), N'1770d5f4-319d-41a2-8673-8a48d8b9b300', CAST(N'2017-08-17T18:21:12.353' AS DateTime), N'1770d5f4-319d-41a2-8673-8a48d8b9b300')
	SET IDENTITY_INSERT [AUTH].[Role] OFF

	INSERT [AUTH].[UserRole] ([RoleId], [UserId]) VALUES (1, N'26cd64b9-e1d1-402d-b1c8-219aec0472d6')
	INSERT [AUTH].[UserRole] ([RoleId], [UserId]) VALUES (3, @adminSubjectId)

	SET IDENTITY_INSERT [AUTH].[UserClaim] ON 
	-- example insert
	--INSERT [AUTH].[UserClaim] ([UserClaimId], [UserId], [ProviderName], [Type], [Value]) VALUES (29, N'26cd64b9-e1d1-402d-b1c8-219aec0472d6', N'AAD', N'name', N'Elmer Fudd')
	SET IDENTITY_INSERT [AUTH].[UserClaim] OFF
  END
  
--Kilr0y$  
update auth.[user] set password='3qjVxEtx1opWjFLTjW/N1Ai549ykpW86SObmI2MNv8P84nisFcoMS6wpnh5wS9I5TJ4B5jzmQvLhpQ8Lx6aANQ==' where username in ('system', 'admin', 'test') and password='secret=='

if (exists(SELECT * FROM [AUTH].[User] WHERE UserId=@adminSubjectId))
	BEGIN

		if (not exists(select * from [AUTH].[UserClaim] where User=@adminSubjectId AND Type='firstname' AND Value='Admin'))
			BEGIN
				INSERT [AUTH].[UserClaim] ([UserId], [Type], [Value]) VALUES (@adminSubjectId, 'firstname', 'Admin')
			END
		if (not exists(select * from [AUTH].[UserClaim] where User=@adminSubjectId AND Type='lastname' AND Value='Adminum'))
			BEGIN
				INSERT [AUTH].[UserClaim] ([UserId], [Type], [Value]) VALUES (@adminSubjectId, 'lastname', 'Adminum')
			END
		if (not exists(select * from [AUTH].[UserClaim] where User=@adminSubjectId AND Type='name' AND Value='The Administrator'))
			BEGIN
				INSERT [AUTH].[UserClaim] ([UserId], [Type], [Value]) VALUES (@adminSubjectId, 'name', 'The Administrator')
			END
		if (not exists(select * from [AUTH].[UserClaim] where User=@adminSubjectId AND Type='email' AND Value='admin@acme.com'))
			BEGIN
				INSERT [AUTH].[UserClaim] ([UserId], [Type], [Value]) VALUES (@adminSubjectId, 'email', 'admin@acme.com')
			END
	END
