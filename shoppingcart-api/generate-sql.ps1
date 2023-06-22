# common repository functions
Import-Module .\Repository.psm1
$config = Get-RepositoryConfiguration

#set variables
$repo = $config.repository.name
$project = $config.database.dbContextProject
$startup = $config.database.startupProject
$context = $config.database.dbContext

echo "Generating transactional SQL migrations for $project..."

$begin = @" 
PRINT 'Before TRY'
BEGIN TRY
	BEGIN TRAN
	PRINT 'First Statement in the TRY block'

"@

$end = @" 
	PRINT 'Last Statement in the TRY block'
	COMMIT TRAN
END TRY
BEGIN CATCH
    PRINT 'In CATCH Block'
    IF(@@TRANCOUNT > 0)
        ROLLBACK TRAN;

    THROW; -- Raise error to the client.
END CATCH
PRINT 'After END CATCH'
GO
"@

## make sure dotnet ef is installed and up to date
dotnet tool update --global dotnet-ef

## get list of migrations
$migrations = (dotnet ef migrations list --no-build --project "$project" --startup-project "$startup" --context "$context")

$previousMigration = "0"
foreach ($migration in $migrations) {
	if ($migration.StartsWith("20")) {
		$migration = $migration -replace ' \(Pending\)', ''

		echo "Generating migration $migration..."
	
		$exists = test-path src/sql/table/$migration.migration.sql
		if (!$exists) {
			## generate single migration
			dotnet ef migrations script $previousMigration $migration --no-build --idempotent --project "$project" --startup-project "$startup" --context "$context" --output src/sql/table/$migration.migration.sql

			$exists = test-path src/sql/table/$migration.migration.sql
			if ($exists) {
				## read output file, replace GO statments and prepend and append transaction syntax
				$text = Get-Content src/sql/table/$migration.migration.sql -Raw 
				$text = $text.replace("GO`r`n", "")
				"$begin$text$end" | Out-File src/sql/table/$migration.migration.sql -Encoding UTF8
			} else {
				echo "error generating migration"
			}
		} else {
			echo "skipping, file already exists"
		}
	
		## set last for next loop
		$previousMigration = $migration 
	}	
}

echo "done"
