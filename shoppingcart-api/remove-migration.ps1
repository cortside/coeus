[cmdletbinding()]
param(
)

# common repository functions
Import-Module .\repository.psm1 -Force
$config = Get-RepositoryConfiguration

#set variables
$repo = $config.repository.name
$project = $config.database.dbContextProject
$startup = $config.database.startupProject
$context = $config.database.dbContext


Write-Output "removing last migration from $context context in project $project"

dotnet ef migrations remove --project "$project" --startup-project "$startup" --context "$context"

Write-Output "done"
