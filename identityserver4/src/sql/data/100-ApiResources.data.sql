-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://approsto.com/sha-generator/

declare @apiResourceId int, @apiResourceName nvarchar(100) = 'eboa.webapi'

if not exists (select top 1 1 from [AUTH].[ApiResources] where [Name] = @apiResourceName) begin

	SET IDENTITY_INSERT [AUTH].[ApiResources] ON 

	INSERT [AUTH].[ApiResources] ([Id], [Description], [DisplayName], [Enabled], [Name]) VALUES (3, N'EBOA.WebApi', N'EBOA.WebApi', 1, @apiResourceName)

	SET IDENTITY_INSERT [AUTH].[ApiResources] OFF

	SET IDENTITY_INSERT [AUTH].[ApiClaims] ON 

	INSERT [AUTH].[ApiClaims] ([Id], [ApiResourceId], [Type]) VALUES (3, 3, N'role')

	SET IDENTITY_INSERT [AUTH].[ApiClaims] OFF

	SET IDENTITY_INSERT [AUTH].[ApiScopes] ON 

	INSERT [AUTH].[ApiScopes] ([Id], [ApiResourceId], [Description], [DisplayName], [Emphasize], [Name], [Required], [ShowInDiscoveryDocument]) VALUES (3, 3, N'EBOA.WebApi', N'EBOA.WebApi', 0, N'eboa.webapi', 0, 1)

	SET IDENTITY_INSERT [AUTH].[ApiScopes] OFF

	SET IDENTITY_INSERT [AUTH].[ApiSecrets] ON 

	-- D<#H;@)NTue7N#aK
	INSERT [AUTH].[ApiSecrets] ([Id], [ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (3, 3, N'EBOA.WebApi', CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'Resource', N'CdcGzCcmgPabVLLnKxSsN1ErZ7J0c2Vmk4bexDArnFM=')

	SET IDENTITY_INSERT [AUTH].[ApiSecrets] OFF

end

select top 1 @apiResourceId = Id from [AUTH].[ApiResources] where [Name] = @apiResourceName

if not exists (select top 1 1 from Auth.ApiSecrets where ApiResourceId = @apiResourceId and [Description] = 'EBOA.WebApi introspection') begin
	insert into Auth.ApiSecrets (ApiResourceId, [Description], Expiration, [Type], [Value]) values (@apiResourceId, 'EBOA.WebApi introspection', '2099-12-31', 'SharedSecret', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=')
end

if not exists (select * from auth.apiclaims where apiresourceid=@apiResourceId and type='groups')
  begin
	INSERT [AUTH].[ApiClaims] ([ApiResourceId], [Type]) VALUES (@apiResourceId, N'groups')
  end

