-- passwords are SHA512 hash base64 endcoded
-- online hash generator
-- https://approsto.com/sha-generator/
declare @adminSubjectId nvarchar(100)
set @adminSubjectId = N'278e1830-93aa-41e8-95a4-1de4a470a200'

if (not exists(select * from auth.[user]))
  BEGIN
	-- test
	INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) VALUES (N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', N'Active', N'system', N'7iaw3Ur350mqGo7jwQrpkj9hiYB3Lkc/iBml1JQODbJ6wYX4oOHV+E+IvIh/1nsUNzLDBMxfqa2Ob1f1ACio/w==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)
	INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) VALUES (@adminSubjectId, N'Active', N'admin', N'7iaw3Ur350mqGo7jwQrpkj9hiYB3Lkc/iBml1JQODbJ6wYX4oOHV+E+IvIh/1nsUNzLDBMxfqa2Ob1f1ACio/w==', N'', 0, NULL, NULL, CAST(N'2017-08-04T22:08:41.080' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T22:08:39.957' AS DateTime), CAST(N'2017-08-04T22:08:41.080' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-08-04T22:08:41.080' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)
	INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) VALUES (N'10a9e432-f75c-423e-b5e8-df4dcfcda19b', N'Active', N'test', N'7iaw3Ur350mqGo7jwQrpkj9hiYB3Lkc/iBml1JQODbJ6wYX4oOHV+E+IvIh/1nsUNzLDBMxfqa2Ob1f1ACio/w==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)

	SET IDENTITY_INSERT [AUTH].[Role] ON 
	INSERT [AUTH].[Role] ([RoleId], [Name], [Description], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId]) VALUES (1, N'test', N'test', CAST(N'2017-08-17T18:21:12.353' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-08-17T18:21:12.353' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186')
	INSERT [AUTH].[Role] ([RoleId], [Name], [Description], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId]) VALUES (3, N'admin', N'admin', CAST(N'2017-08-17T18:21:12.353' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-08-17T18:21:12.353' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186')
	SET IDENTITY_INSERT [AUTH].[Role] OFF

	INSERT [AUTH].[UserRole] ([RoleId], [UserId]) VALUES (1, N'10a9e432-f75c-423e-b5e8-df4dcfcda19b')
	INSERT [AUTH].[UserRole] ([RoleId], [UserId]) VALUES (3, @adminSubjectId)

	SET IDENTITY_INSERT [AUTH].[UserClaim] ON 
	-- example insert
	--INSERT [AUTH].[UserClaim] ([UserClaimId], [UserId], [ProviderName], [Type], [Value]) VALUES (29, N'611c0fcb-4cf4-45a0-b9ea-5240f53d8726', N'AAD', N'name', N'Elmer Fudd')
	SET IDENTITY_INSERT [AUTH].[UserClaim] OFF
  END
  
--Kilr0y$  
update auth.[user] set password='3qjVxEtx1opWjFLTjW/N1Ai549ykpW86SObmI2MNv8P84nisFcoMS6wpnh5wS9I5TJ4B5jzmQvLhpQ8Lx6aANQ==' where username in ('system', 'admin', 'test') and password='7iaw3Ur350mqGo7jwQrpkj9hiYB3Lkc/iBml1JQODbJ6wYX4oOHV+E+IvIh/1nsUNzLDBMxfqa2Ob1f1ACio/w=='

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
		if (not exists(select * from [AUTH].[UserClaim] where User=@adminSubjectId AND Type='email' AND Value='admin@ebadmin.eb'))
			BEGIN
				INSERT [AUTH].[UserClaim] ([UserId], [Type], [Value]) VALUES (@adminSubjectId, 'email', 'admin@ebadmin.eb')
			END
	END