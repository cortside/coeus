[cmdletbinding()]
param(
)

. .\repository.ps1

echo "removing last migration from $context context in project $project"

dotnet ef migrations remove --project "$project" --startup-project "$startup" --context "$context"

echo "done"
