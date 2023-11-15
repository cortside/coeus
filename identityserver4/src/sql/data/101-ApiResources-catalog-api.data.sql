-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://dotnetfiddle.net/h3aeqd

declare @id int
declare @name nvarchar(400)
set @name='catalog-api'

if (not exists(select * from auth.[ApiResources] where name=@name))
  BEGIN
	INSERT [AUTH].[ApiResources] ([Description], [DisplayName], [Enabled], [Name]) VALUES (@name, @name, 1, @name)
	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ApiResourceClaims] ([ApiResourceId], [Type]) VALUES (@id, N'role')
	INSERT [AUTH].[ApiScopes] ([ApiResourceId], [Description], [DisplayName], [Emphasize], [Name], [Required], [ShowInDiscoveryDocument]) VALUES (@id, @name, @name, 0, @name, 0, 1)
	-- secret
	INSERT [AUTH].[ApiResourceSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'Resource', N'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=')
  END

if (exists(select * from [AUTH].[ApiResources] where Name=@name))
  BEGIN
  	set @id = (SELECT Id FROM [AUTH].[ApiResources] WHERE Name=@name)

	if (not exists(SELECT * FROM [AUTH].[ApiResourceSecrets] where ApiResourceId=@id AND Type = 'SharedSecret'))
		BEGIN
			-- secret
			INSERT [AUTH].[ApiResourceSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'SharedSecret', N'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=')
		END
  END

-- insert any missing api resource scopes
insert into auth.ApiResourceScopes (ApiResourceId, Scope)
select r.id, r.Name
from auth.ApiResources r
left join auth.ApiResourceScopes rs on rs.ApiResourceId=r.Id and rs.Scope=r.Name
where rs.Scope is null
