DECLARE @ticketID NVARCHAR(100) = 'PRJ-XXXX'

BEGIN TRY
	BEGIN TRANSACTION;
		DECLARE @now DATETIME = GETUTCDATE()
		DECLARE @subjectId UNIQUEIDENTIFIER

		IF NOT EXISTS (SELECT * FROM [<DataBaseName>].[dbo].[Subject] WHERE UserPrincipalName = @ticketID)
		BEGIN
			INSERT INTO [ <DataBaseName>] [dbo] [Subject] (SubjectId, [Name], GivenName, FamilyName, UserPrincipalName, CreatedDate
			VALUES (NewID(), @ticketID, null, null, @ticketID, @now);

			SET @subjectId = (SELECT SubjectId FROM [<DataBaseName>] [dbol [Subject] WHERE UserPrincipalName = @ticketID)
		END

		-- Main SQL goes here - be sure any inserts or updates include
		UPDATE [<DataBaseName>].[dbo].[<TABLE>]
		SET
		LastModifiedSubjectId = @subjectId,
		LastModifiedDate = @now,
		<FIELDS TO UPDATE HERE>
		FROM [<DataBaseName>].[dbo].[<TABLE>]

	-- ROLLBACK TRAN -- uncomment this rollback for testing
	COMMIT TRAN -- comment this commit when you uncomment the line above
END TRY
BEGIN CATCH
	PRINT 'In CATCH Block'
	IF(@@TRANCOUNT > 0)
		ROLLBACK TRAN;

	THROW; -- Raise error to the client.
END CATCH
GO
