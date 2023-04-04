# these values are referenced in scripts that use dotnet ef commands

$repo = "Acme.ShoppingCart"
$project = "src/$repo.Data"
$startup = "src/$repo.WebApi"
$context = "DatabaseContext"

$repoConfig = if (Test-Path repository.json) { get-content repository.json | ConvertFrom-Json }
