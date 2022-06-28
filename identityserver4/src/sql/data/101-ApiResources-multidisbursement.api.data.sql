-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://quickhash.com/

declare @name nvarchar(400)
declare @id int
set @name='multidisbursement.api'

if (not exists(select * from auth.[ApiResources] where name=@name))
  BEGIN
	INSERT [AUTH].[ApiResources] ([Description], [DisplayName], [Enabled], [Name]) VALUES (N'multidisbursement.api', N'multidisbursement.api', 1, N'multidisbursement.api')
	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ApiClaims] ([ApiResourceId], [Type]) VALUES (@id, N'role')
	INSERT [AUTH].[ApiScopes] ([ApiResourceId], [Description], [DisplayName], [Emphasize], [Name], [Required], [ShowInDiscoveryDocument]) VALUES (@id, N'multidisbursement.api', N'multidisbursement.api', 0, N'multidisbursement.api', 0, 1)
	-- xQeHP^VLvS9h429g
	INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, N'multidisbursement.api', CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'Resource', N'eGUMhEp6W3ewk/bytbXKfjIDYBdB8WqW7p2DtWkrN14=')
  END

if (exists(select * from [AUTH].[ApiResources] where Name=@name))
  BEGIN
  	set @id = (SELECT Id FROM [AUTH].[ApiResources] WHERE Name=@name)

	if (not exists(SELECT * FROM [AUTH].[ApiSecrets] where ApiResourceId=@id AND Type = 'SharedSecret'))
		BEGIN
			-- xQeHP^VLvS9h429g
			INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'SharedSecret', N'eGUMhEp6W3ewk/bytbXKfjIDYBdB8WqW7p2DtWkrN14=')
		END
  END