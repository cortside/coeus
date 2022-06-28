-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://quickhash.com/

declare @name nvarchar(400)
set @name='user-api'

-- 34mwIWK348WeV8ZA3
declare @secret nvarchar(100)
set @secret = '7zom6SqaRcBx/t7crhsYna7R61hKwv5I4OQPabh1TTU='

if (not exists(select * from auth.[ApiResources] where name=@name))
  BEGIN
	declare @id int
	INSERT [AUTH].[ApiResources] ([Description], [DisplayName], [Enabled], [Name]) VALUES (@name, @name, 1, @name)
	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ApiClaims] ([ApiResourceId], [Type]) VALUES (@id, N'role')
	INSERT [AUTH].[ApiScopes] ([ApiResourceId], [Description], [DisplayName], [Emphasize], [Name], [Required], [ShowInDiscoveryDocument]) VALUES (@id, @name, @name, 0, @name, 0, 1)
	INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'Resource', @secret)
  END

if (exists(select * from [AUTH].[ApiResources] where Name=@name))
  BEGIN
  	set @id = (SELECT Id FROM [AUTH].[ApiResources] WHERE Name=@name)

	declare @oldsecret nvarchar(100)
	set @oldsecret = 'ef3a26e92a9a45c071fededcae1b189daed1eb584ac2fe48e0e40f69b8754d35'
	if (exists(SELECT * FROM [AUTH].[ApiSecrets] where ApiResourceId=@id AND Type = 'SharedSecret' AND Value = @oldsecret))
		BEGIN
			UPDATE [AUTH].[ApiSecrets]
			SET Value = @secret
			Where ApiResourceId=@id AND Type = 'SharedSecret' AND Value = @oldsecret
		END

	if (not exists(SELECT * FROM [AUTH].[ApiSecrets] where ApiResourceId=@id AND Type = 'SharedSecret'))
		BEGIN
			INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'SharedSecret', @secret)
		END
  END