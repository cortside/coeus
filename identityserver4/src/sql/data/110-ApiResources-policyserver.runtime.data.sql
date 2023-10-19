-- passwords are SHA256 hash base64 endcoded
-- online hash generator
-- https://dotnetfiddle.net/h3aeqd

declare @name nvarchar(400)
set @name='policyserver.runtime'

if (not exists(select * from auth.[ApiResources] where name=@name))
  BEGIN
	declare @id int
	INSERT [AUTH].[ApiResources] ([Description], [DisplayName], [Enabled], [Name]) VALUES (N'policyserver.runtime', N'policyserver.runtime', 1, N'policyserver.runtime')
	set @id = SCOPE_IDENTITY()

	INSERT [AUTH].[ApiResourceClaims] ([ApiResourceId], [Type]) VALUES (@id, N'role')
	INSERT [AUTH].[ApiScopes] ([ApiResourceId], [Description], [DisplayName], [Emphasize], [Name], [Required], [ShowInDiscoveryDocument]) VALUES (@id, N'policyserver.runtime', N'policyserver.runtime', 0, N'policyserver.runtime', 0, 1)
	

  -- "[Value]" contains a base64 encoding of a SHA256 hash of a random string.
	-- Generate the [Value] using that password in https://quickhash.com/ (use SHA-256 hashing, with the output of base64 encoding)
	-- policypass
	INSERT [AUTH].[ApiResourceSecrets] ([ApiResourceId], [Description], [Expiration], [Type], [Value]) VALUES (@id, N'policyserver.runtime', CAST(N'2099-12-31T00:00:00.0000000' AS DateTime2), N'Resource', N'V3J3sVs+undm/4pZyp16++QO+TQSHmxjx/jfshX2qtM=')
  
	END
