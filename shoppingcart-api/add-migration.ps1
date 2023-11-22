[cmdletbinding()]
param(
	[parameter(Mandatory = $true)][string]$migration,
	[Parameter(Mandatory = $false)][switch]$updateDatabase
)

# common repository functions
Import-Module .\Repository.psm1
$config = Get-RepositoryConfiguration

#set variables
$repo = $config.repository.name
$project = $config.database.dbContextProject
$startup = $config.database.startupProject
$context = $config.database.dbContext

echo "creating new migration $migration for $context context in project $project"
dotnet tool update --global dotnet-ef

dotnet ef migrations add $migration --project "$project" --startup-project "$startup" --context "$context"

dotnet build ./src

.\generate-sql.ps1
.\generate-sqltriggers.ps1
if ($updateDatabase.IsPresent) {
	.\update-database.ps1
} else {
	echo "Run `./update-database.ps1` to execute the sql scripts and update the database OR include -updateDatabase when running this script"
}

echo "done"
