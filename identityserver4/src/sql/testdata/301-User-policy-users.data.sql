
-- FMS User Roles

if (not exists(select * from auth.[user] WHERE Username = 'contractorServicesTeamUser'))
	BEGIN
		INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) 
		VALUES (N'79a3516e-b9a1-4782-87b7-7586529d5260', N'Active', N'contractorServicesTeamUser', N'3qjVxEtx1opWjFLTjW/N1Ai549ykpW86SObmI2MNv8P84nisFcoMS6wpnh5wS9I5TJ4B5jzmQvLhpQ8Lx6aANQ==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)
	END

if (not exists(select * from auth.[user] WHERE Username = 'contractorServicesTeamAdmin'))
	BEGIN
		INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) 
		VALUES (N'317cfd3b-a727-4c87-bcc6-946114bfafab', N'Active', N'contractorServicesTeamAdmin', N'3qjVxEtx1opWjFLTjW/N1Ai549ykpW86SObmI2MNv8P84nisFcoMS6wpnh5wS9I5TJ4B5jzmQvLhpQ8Lx6aANQ==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)
	END

-- MD User Roles

if (not exists(select * from auth.[user] WHERE Username = 'loanDocumentRequestUser'))
	BEGIN
		INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) 
		VALUES (N'998a5114-1e36-40dc-ab52-230e077d0215', N'Active', N'loanDocumentRequestUser', N'3qjVxEtx1opWjFLTjW/N1Ai549ykpW86SObmI2MNv8P84nisFcoMS6wpnh5wS9I5TJ4B5jzmQvLhpQ8Lx6aANQ==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)
	END

if (not exists(select * from auth.[user] WHERE Username = 'singleSignOnUser'))
	BEGIN
		INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) 
		VALUES (N'e3dd5b64-c90c-4864-b598-f7c4e6d17df8', N'Active', N'singleSignOnUser', N'3qjVxEtx1opWjFLTjW/N1Ai549ykpW86SObmI2MNv8P84nisFcoMS6wpnh5wS9I5TJ4B5jzmQvLhpQ8Lx6aANQ==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)
	END

if (not exists(select * from auth.[user] WHERE Username = 'notificationsUser'))
	BEGIN
		INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) 
		VALUES (N'55716bb7-238d-477b-8ff2-e342f3ac380a', N'Active', N'notificationsUser', N'3qjVxEtx1opWjFLTjW/N1Ai549ykpW86SObmI2MNv8P84nisFcoMS6wpnh5wS9I5TJ4B5jzmQvLhpQ8Lx6aANQ==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)
	END

if (not exists(select * from auth.[user] WHERE Username = 'externalAdmin'))
	BEGIN
		INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) 
		VALUES (N'674de0ab-9612-44e1-9fb3-738a02f45b53', N'Active', N'externalAdmin', N'3qjVxEtx1opWjFLTjW/N1Ai549ykpW86SObmI2MNv8P84nisFcoMS6wpnh5wS9I5TJ4B5jzmQvLhpQ8Lx6aANQ==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)
	END

if (not exists(select * from auth.[user] WHERE Username = 'supportUser'))
	BEGIN
		INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) 
		VALUES (N'ebf1d0fb-44f2-4ffb-a76a-9990a0dd6735', N'Active', N'supportUser', N'3qjVxEtx1opWjFLTjW/N1Ai549ykpW86SObmI2MNv8P84nisFcoMS6wpnh5wS9I5TJ4B5jzmQvLhpQ8Lx6aANQ==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)
	END

if (not exists(select * from auth.[user] WHERE Username = 'informationalUser'))
	BEGIN
		INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) 
		VALUES (N'872fb725-9f94-40fa-83e6-2523eaede408', N'Active', N'informationalUser', N'3qjVxEtx1opWjFLTjW/N1Ai549ykpW86SObmI2MNv8P84nisFcoMS6wpnh5wS9I5TJ4B5jzmQvLhpQ8Lx6aANQ==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)
	END

if (not exists(select * from auth.[user] WHERE Username = 'onCallEngineer'))
	BEGIN
		INSERT [AUTH].[user] ([UserId], [UserStatus], [Username], [Password], [Salt], [LoginCount], [LastLogin], [LastLoginIPAddress], [EffectiveDate], [ExpirationDate], [SecurityToken], [TokenExpiration], [TermsOfUseAcceptanceDate], [CreateDate], [CreateUserId], [LastModifiedDate], [LastModifiedUserId], [ProviderName], [ProviderSubjectId]) 
		VALUES (N'32fd0108-69e5-459c-a2cb-9c0b9497df3b', N'Active', N'onCallEngineer', N'3qjVxEtx1opWjFLTjW/N1Ai549ykpW86SObmI2MNv8P84nisFcoMS6wpnh5wS9I5TJ4B5jzmQvLhpQ8Lx6aANQ==', N'', 0, NULL, NULL, CAST(N'2017-05-16T16:59:29.270' AS DateTime), NULL, NULL, NULL, CAST(N'2017-08-04T16:43:06.480' AS DateTime), CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', CAST(N'2017-05-16T16:59:29.270' AS DateTime), N'c7e2dcfc-f1aa-49e5-a6ff-e7b880cf1186', NULL, NULL)
	END