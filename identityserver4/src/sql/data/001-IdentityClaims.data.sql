if not exists (select * from auth.IdentityResourceClaims)
 begin
	SET IDENTITY_INSERT [AUTH].[IdentityResourceClaims] ON 
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (1, 1, N'sub')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (2, 2, N'name')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (3, 2, N'family_name')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (4, 2, N'given_name')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (5, 2, N'middle_name')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (6, 2, N'nickname')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (7, 2, N'preferred_username')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (8, 2, N'profile')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (9, 2, N'picture')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (10, 2, N'website')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (11, 2, N'gender')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (12, 2, N'birthdate')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (13, 2, N'zoneinfo')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (14, 2, N'locale')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (15, 2, N'updated_at')
	INSERT [AUTH].[IdentityResourceClaims] ([Id], [IdentityResourceId], [Type]) VALUES (16, 3, N'role')
	SET IDENTITY_INSERT [AUTH].[IdentityResourceClaims] OFF
  end

if not exists (select * from auth.IdentityResourceClaims where type='groups' and IdentityResourceId=3)
  begin
	INSERT [AUTH].[IdentityResourceClaims] ([IdentityResourceId], [Type]) VALUES (3, N'groups')
  end
