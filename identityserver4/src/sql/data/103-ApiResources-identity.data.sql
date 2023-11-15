-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://dotnetfiddle.net/h3aeqd

declare @name nvarchar(400), @id int
set @name='identity'

if (not exists(select * from auth.[ApiResources] where name=@name))
  BEGIN
	INSERT [AUTH].[ApiResources] ([Description], [DisplayName], [Enabled], [Name]) VALUES (@name, @name, 1, @name)
	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ApiResourceClaims] ([ApiResourceId], [Type]) VALUES (@id, N'role')
	INSERT [AUTH].[ApiScopes] ([ApiResourceId], [Description], [DisplayName], [Emphasize], [Name], [Required], [ShowInDiscoveryDocument]) VALUES (@id, @name, @name, 0, @name, 0, 1)
	
  -- "[Value]" contains a string that was hased using SHA256 and then base64 encoded. 
	-- That initial string is just a random string that works as a password: yT4pw:A@++Z{>`8
	-- Generate the [Value] using that password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)

--declare @s varchar(max), @hb varbinary(128), @h64 varchar(128);
--select @s = '2016-01-012016-12-31123456789012000EUR';
--set @hb = hashbytes('sha2_256', @s);
--set @h64 = cast(N'' as xml).value('xs:base64Binary(sql:variable("@hb"))', 'varchar(128)');
--select @hb as [BinaryHash], @h64 as [64Hash];

	INSERT [AUTH].[ApiResourceSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, @name, CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'Resource', N'lam6cNU2H3t93VlA1M11H0fZFAOUDZXqpsjpCgSlPOc=')
  END

select top 1 @id = Id from [AUTH].[ApiResources] where [Name] = @name

if not exists (select top 1 1 from Auth.ApiResourceSecrets where ApiResourceId = @id and [Description] = 'identity introspection') begin
	insert into Auth.ApiResourceSecrets (ApiResourceId, [Description], Expiration, [Type], [Value]) values (@id, 'identity introspection', '2099-12-31', 'SharedSecret', 'K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=')
end