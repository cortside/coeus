DECLARE @clientId NVARCHAR(400)
SET @clientId='acme'
DECLARE @id INT
SELECT @id = id FROM [AUTH].[Clients] WHERE [ClientId]=@clientId
IF (NOT EXISTS(SELECT * FROM [AUTH].[ClientScopes] WHERE [ClientId]=@id and [Scope]=N'common.communications'))
  BEGIN
	INSERT [AUTH].[ClientScopes] ([ClientId], [Scope]) VALUES (@id, N'common.communications')
  END
