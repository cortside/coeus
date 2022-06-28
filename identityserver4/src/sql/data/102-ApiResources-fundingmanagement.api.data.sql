-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://quickhash.com/

declare @name nvarchar(400)
declare @id int
set @name='fundingmanagement.api'

if (not exists(select * from auth.[ApiResources] where name=@name))
  BEGIN
	INSERT [AUTH].[ApiResources] ([Description], [DisplayName], [Enabled], [Name]) VALUES (N'fundingmanagement.api', N'fundingmanagement.api', 1, N'fundingmanagement.api')
	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ApiClaims] ([ApiResourceId], [Type]) VALUES (@id, N'role')
	INSERT [AUTH].[ApiScopes] ([ApiResourceId], [Description], [DisplayName], [Emphasize], [Name], [Required], [ShowInDiscoveryDocument]) VALUES (@id, N'fundingmanagement.api', N'fundingmanagement.api', 0, N'fundingmanagement.api', 0, 1)
	
    -- "[Value]" contains a string that was hased using SHA256 and then base64 encoded. 
	-- That initial string is just a random string that works as a password: yT4pw:A@++Z{>`8
	-- Generate the [Value] using that password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, N'fundingmanagement.api', CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'Resource', N'lam6cNU2H3t93VlA1M11H0fZFAOUDZXqpsjpCgSlPOc=')
  
END

if (exists(select * from [AUTH].[ApiResources] where Name=@name))
  BEGIN
  	set @id = (SELECT Id FROM [AUTH].[ApiResources] WHERE Name=@name)

	if (not exists(SELECT * FROM [AUTH].[ApiSecrets] where ApiResourceId=@id AND Type = 'SharedSecret'))
		BEGIN
			-- yT4pw:A@++Z{>`8
			INSERT [AUTH].[ApiSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'SharedSecret', N'lam6cNU2H3t93VlA1M11H0fZFAOUDZXqpsjpCgSlPOc=')
		END
  END