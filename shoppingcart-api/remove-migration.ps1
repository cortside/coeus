[cmdletbinding()]
param(
)

# common repository functions
Import-Module .\Repository.psm1
$config = Get-RepositoryConfiguration

#set variables
$repo = $config.repository.name
$project = $config.database.dbContextProject
$startup = $config.database.startupProject
$context = $config.database.dbContext


echo "removing last migration from $context context in project $project"

dotnet ef migrations remove --project "$project" --startup-project "$startup" --context "$context"

echo "done"
