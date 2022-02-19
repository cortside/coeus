[cmdletbinding()]
param(
	[parameter(Mandatory=$true)][string]$migration
)

$repo = "Acme.ShoppingCart"
$project = "src/$repo.Data"
$startup = "src/$repo.WebApi"
$context = "DatabaseContext"

echo "creating new migration $migration for $context context in project $project"

dotnet ef migrations add $migration --project "$project" --startup-project "$startup" --context "$context"

dotnet build ./src

.\generate-sql.ps1

echo "done"
