[cmdletbinding()]
param(
	[parameter(Mandatory = $true)][string]$migration
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
.\update-database.ps1

echo "done"
