-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://quickhash.com/

declare @id int
declare @name nvarchar(400) = 'loan-api'
declare @secret nvarchar(500) = N'psjSYVnahd8Mktirssl6dYw8F5UpHK7BYIBH/ATA18o='  -- [PAaEyZEXC&5jK{

if (not exists(select * from auth.[ApiResources] where name=@name))
  BEGIN
	INSERT [AUTH].[ApiResources] ([Description], [DisplayName], [Enabled], [Name]) VALUES (@name, @name, 1, @name)
	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ApiClaims] ([ApiResourceId], [Type]) VALUES (@id, N'role')
	INSERT [AUTH].[ApiScopes] ([ApiResourceId], [Description], [DisplayName], [Emphasize], [Name], [Required], [ShowInDiscoveryDocument]) VALUES (@id, @name, @name, 0, @name, 0, 1)
	INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'Resource', @secret)
  END

select @id=id from auth.ApiResources where name=@name

-- for introspection
if not exists(select * from auth.ApiSecrets where ApiResourceId=@id and type='SharedSecret')
  BEGIN
	insert into Auth.ApiSecrets (ApiResourceId, [Description], Expiration, [Type], [Value]) values (@id, @name, '2099-12-31', 'SharedSecret', @secret)
  END

-- so that groups claims are in the token
if not exists (select * from auth.apiclaims where apiresourceid=@id and type='groups')
  begin
	INSERT [AUTH].[ApiClaims] ([ApiResourceId], [Type]) VALUES (@id, N'groups')
  end
