# these values are referenced in scripts that use dotnet ef commands
Function Get-RepositoryConfiguration {
	$configfile = "repository.json"
	if (!(Test-Path $configfile)) {
		throw "config file $configfile not found"
	}
	$config = get-content $configfile | ConvertFrom-Json 

	$repo = $config.repository.name
	$project = $config.database.dbContextProject
	$startup = $config.database.startupProject
	$context = $config.database.dbContext
	$defaultDatabase = $config.database.name

	echo "====="
	echo "Repository: $repo"
	echo "Default Database: $defaultDatabase"
	echo "DbContext: $context"
	echo "DbContext Project: $project"
	echo "Startup Project: $startup"
	echo "====="
	
	return $config
}
