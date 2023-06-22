|Script   | Arguments | Example | Description |
|---|---|---|---|
| add-migration.ps1 | -migration \<migration name\> | ./add-migration.ps1 -migration "My New Table" | Add an EF migration.  This will also call scripts to generate the sql script for the migration, the script to generate sql triggers and call the script to run all sql scripts to update the database.   |
| build-dockerimages.ps1 |  |  | Ability to build project inside docker container and output docker image for running built project |
| build.ps1 |  |  |  |
| clean.ps1 |  |  | Clean build and publish artifacts, i.e. bin and obj directories.  Also cleans local package download directories and publish artifacts. |
| convert-encoding.ps1 |  |  | Use to convert source file encoding to UTF-8 |
| create-release.ps1 |  |  | Create release branch and increment develop branch.  Generates update to CHANGELOG.md. |
| dependabot.ps1 |  |  | Use to automate attempt to update nuget package dependencies.  Script will create a branch if build and tests pass after update. |
| export-seeddata.ps1 |  |  | Export CSV files to be used for seed data in integration data from a database. |
| find-projects-not-in-solutions.ps1 |  |  | Find projects in directory that are not included in a solution.  Good to use for finding abandoned code files. |
| fix-encoding.ps1 |  |  | Use to convert source file encoding to UTF-8 |
| format.ps1 |  |  | Format source code to comply with rules from .editorconfig.  Uses dotnet format tool. |
| generate-changelog.ps1 |  |  | Update CHANGELOG.md based on commits not in master. |
| generate-datadictionary.ps1 |  |  | Generate CSV file with data from vwDataDictionary view. |
| generate-sql.ps1 |  |  | Generate sql scripts from EF migrations to be run by script to updated database. |
| generate-sqltriggers.ps1 |  |  | Generate sql triggers scripts generated from temporary database to be run by script to updated database. |
| git-prune.ps1 |  |  |  |
| git-pruneremote.ps1 |  |  |  |
| prebuild.ps1 |  |  |  |
| remove-migration.ps1 |  |  | Remove last EF migration |
| run.ps1 |  |  |  |
| runtests.ps1 |  |  |  |
| set-rabbitqueues.ps1 |  |  |  |
| start-rabbitmq.ps1 |  |  |  |
| update-database.ps1 |  |  | Update database based on sql scripts that may include generated ones and manually created or modified ones. |
| update-nugetpackages.ps1 |  |  | Update nuget packages to latest version.  Defaults to latest non-prerelease version within major version. |
| update-targetframework.ps1 |  |  | Update projects to specified target version.  Useful for when new .Net version is released. |
| update-template.ps1 |  |  | Update repository based on changes in templates. |
