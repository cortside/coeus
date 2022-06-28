-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://quickhash.com/

declare @name nvarchar(400)
set @name='termscalculator-api'

if (not exists(select * from auth.[ApiResources] where name=@name))
  BEGIN
	declare @id int
	INSERT [AUTH].[ApiResources] ([Description], [DisplayName], [Enabled], [Name]) VALUES (@name, @name, 1, @name)
	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ApiClaims] ([ApiResourceId], [Type]) VALUES (@id, N'role')
	INSERT [AUTH].[ApiScopes] ([ApiResourceId], [Description], [DisplayName], [Emphasize], [Name], [Required], [ShowInDiscoveryDocument]) VALUES (@id, @name, @name, 0, @name, 0, 1)
	-- tLulX0Va5TZVrVnR
	INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'Resource', N'/sUe5oqqWV7W49mvCicHqrRWHP99s2MuYbe2izERous=')
  END
