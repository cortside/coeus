-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://dotnetfiddle.net/h3aeqd

declare @id int
declare @name nvarchar(400) = 'prequalification-api'

if (not exists(select * from auth.[ApiResources] where name=@name))
  BEGIN
	INSERT [AUTH].[ApiResources] ([Description], [DisplayName], [Enabled], [Name]) VALUES (@name, @name, 1, @name)
	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ApiClaims] ([ApiResourceId], [Type]) VALUES (@id, N'role')
	INSERT [AUTH].[ApiScopes] ([ApiResourceId], [Description], [DisplayName], [Emphasize], [Name], [Required], [ShowInDiscoveryDocument]) VALUES (@id, @name, @name, 0, @name, 0, 1)
	-- S$X}ZD6f<Mj)#pxJ
	INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'Resource', N'hU3pq5tXfem8M/yeW/9bpb1RNHsOI8v5avMGU1c91lQ=')
  END

select @id=id from auth.ApiResources where name=@name

-- so that groups claims are in the token
if not exists (select * from auth.apiclaims where apiresourceid=@id and type='groups')
  begin
	INSERT [AUTH].[ApiClaims] ([ApiResourceId], [Type]) VALUES (@id, N'groups')
  end

if (exists(select * from [AUTH].[ApiResources] where Name=@name))
  BEGIN
  	set @id = (SELECT Id FROM [AUTH].[ApiResources] WHERE Name=@name)

	if (not exists(SELECT * FROM [AUTH].[ApiSecrets] where ApiResourceId=@id AND Type = 'SharedSecret'))
		BEGIN
			-- S$X}ZD6f<Mj)#pxJ
			INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'SharedSecret', N'hU3pq5tXfem8M/yeW/9bpb1RNHsOI8v5avMGU1c91lQ=')
		END
  END
