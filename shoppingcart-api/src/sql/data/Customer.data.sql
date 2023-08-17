-- customer data
IF((SELECT count(*) FROM [dbo].[Customer])=0)
BEGIN
	INSERT INTO [dbo].[Customer]([CustomerResourceId],[FirstName],[LastName],[Email],[CreatedDate],[CreateSubjectId],[LastModifiedDate],[LastModifiedSubjectId])
     VALUES (newid(), 'Test','User','test@test.com',getutcdate(),'00000000-0000-0000-0000-000000000000',getutcdate(),'00000000-0000-0000-0000-000000000000')  
END
