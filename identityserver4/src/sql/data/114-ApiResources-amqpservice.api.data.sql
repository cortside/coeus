-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://quickhash.com/

declare @name nvarchar(400)
declare @id int
declare @pwHash nvarchar(400)
set @name ='amqpservice.api'
set @pwHash = N'OQQ3gCbym84jY1mxtN9tyPZVNbkIADgDfYDhDIoNw5I='

if (not exists(select * from auth.[ApiResources] where name=@name))
  BEGIN
	INSERT [AUTH].[ApiResources] ([Description], [DisplayName], [Enabled], [Name]) VALUES (@name, @name, 1, @name)
	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ApiClaims] ([ApiResourceId], [Type]) VALUES (@id, N'role')
	INSERT [AUTH].[ApiScopes] ([ApiResourceId], [Description], [DisplayName], [Emphasize], [Name], [Required], [ShowInDiscoveryDocument]) VALUES (@id, @name, @name, 0, @name, 0, 1)
	
    -- "[Value]" contains a string that was hased using SHA256 and then base64 encoded. 
	-- That initial string is just a random string that works as a password: npnzvkwDujsjluMZ
	-- Generate the [Value] using that password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'Resource', @pwHash)
  
END

if (exists(select * from [AUTH].[ApiResources] where Name=@name))
  BEGIN
  	set @id = (SELECT Id FROM [AUTH].[ApiResources] WHERE Name=@name)

	if (not exists(SELECT * FROM [AUTH].[ApiSecrets] where ApiResourceId=@id AND Type = 'SharedSecret'))
		BEGIN
			-- npnzvkwDujsjluMZ
			INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'SharedSecret', @pwHash)
		END
  END
