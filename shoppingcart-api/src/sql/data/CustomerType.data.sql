-- customer type data
IF OBJECT_ID (N'CustomerType', N'U') IS NOT NULL
BEGIN
	IF((SELECT count(*) FROM [dbo].[CustomerType])=0)
	BEGIN
		INSERT INTO [dbo].[CustomerType]([Name],[Description],[TaxExempt],[CreatedDate],[CreateSubjectId],[LastModifiedDate],[LastModifiedSubjectId])
		VALUES ('Internal','Internal Employee Customer',1,getutcdate(),'00000000-0000-0000-0000-000000000000',getutcdate(),'00000000-0000-0000-0000-000000000000')
		
		INSERT INTO [dbo].[CustomerType]([Name],[Description],[TaxExempt],[CreatedDate],[CreateSubjectId],[LastModifiedDate],[LastModifiedSubjectId])
		VALUES ('Individual','Individual Customer',0,getutcdate(),'00000000-0000-0000-0000-000000000000',getutcdate(),'00000000-0000-0000-0000-000000000000')
		
		INSERT INTO [dbo].[CustomerType]([Name],[Description],[TaxExempt],[CreatedDate],[CreateSubjectId],[LastModifiedDate],[LastModifiedSubjectId])
		VALUES ('Non-Profit','Non-Profit Customer',1,getutcdate(),'00000000-0000-0000-0000-000000000000',getutcdate(),'00000000-0000-0000-0000-000000000000')
	END
END

-- default customer type on customer to first customer type
IF COL_LENGTH('Customer','CustomerTypeId') IS NOT NULL
BEGIN
	EXEC sp_executesql N'UPDATE Customer set CustomerTypeId = (select top 1 CustomerTypeId from CustomerType) where CustomerTypeId is null'	
END
