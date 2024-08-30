Param (
)

$ErrorActionPreference = "Stop"

function Update-Myself {
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true, Position = 0)]
        [string]$SourcePath
    )
	
    #check that the destination file exists
    if (Test-Path $SourcePath) {	
		#The path of THIS script
		$CurrentScript = $MyInvocation.ScriptName
		if (!($SourcePath -eq $CurrentScript )) {
			if ($(Get-Item $SourcePath).LastWriteTimeUtc -gt $(Get-Item $CurrentScript ).LastWriteTimeUtc) {
				write-host "Updating $SourcePath"
				Copy-Item $SourcePath $CurrentScript 
				#If the script was updated, run it with orginal parameters
				&$CurrentScript $script:args
				exit
			}
		}
    }
    write-host "No update required"
}

# Remove a file or folder quietly
# Like linux "rm -rf"
function Remove-IfItemExists($item) {
  if (Test-Path $item) {
    echo "Removing $item"
    Remove-Item $item  -r -force
  }
}

$configfile = "repository.json"
if (!(Test-Path -path $configfile)) {
	Write-Output "repository.json not found"
	exit 1
}

if ((Test-Path -path temp)) {
	Write-Output "Removing existing temp directory"
	rm .\temp\ -Recurse -Force
}

mkdir temp
# make sure latest version of cortside.templates is installed
#dotnet new --install cortside.templates
git clone https://github.com/cortside/coeus.git temp/coeus
# hack to set last write time to last git commit instead of time repo was cloned
cd temp/coeus/shoppingcart-api
(get-item .\update-fromtemplate.ps1).LastWriteTime = get-date((git log -1 --format=%aI .\update-fromtemplate.ps1))
cd ../../..

Update-Myself .\temp\coeus\shoppingcart-api\update-fromtemplate.ps1

$config = get-content $configfile | ConvertFrom-Json 

$service = $config.service
$repository = $config.repository.name
$database = $config.database.name

if ($service -eq "" -or $repository -eq "") {
	echo "missing parameters"
	exit 1
}

$hasDatabase = $false
if ($database -ne "") {
	$hasDatabase = $true
}	
		
echo "service: $service"
echo "repository: $repository"
echo "database: $database"
echo "hasDatabase: $hasDatabase"
		
cp .\temp\coeus\shoppingcart-api\clean.ps1
cp .\temp\coeus\shoppingcart-api\format.ps1
cp .\temp\coeus\shoppingcart-api\create-release.ps1
cp .\temp\coeus\shoppingcart-api\generate-changelog.ps1
cp .\temp\coeus\shoppingcart-api\update-nugetpackages.ps1
cp .\temp\coeus\shoppingcart-api\src\.editorconfig .\src\.editorconfig
cp .\temp\coeus\shoppingcart-api\.gitignore

Remove-IfItemExists update-template.ps1

#cp .\temp\coeus\shoppingcart-api\.gitattributes
#git commit -m "Saving files before refreshing line endings" .gitattributes
#git add --renormalize .

cp .\temp\coeus\shoppingcart-api\build.ps1
cp .\temp\coeus\shoppingcart-api\dependabot.ps1
cp .\temp\coeus\shoppingcart-api\update-targetframework.ps1

#((Get-Content -path build.ps1 -Raw) -replace 'Acme.ShoppingCart',$repository) | Set-Content -NoNewline -Path build.ps1 -Encoding utf8

if ($hasDatabase) {
	cp .\temp\coeus\shoppingcart-api\add-migration.ps1
	cp .\temp\coeus\shoppingcart-api\generate-sql.ps1
	cp .\temp\coeus\shoppingcart-api\generate-sqltriggers.ps1
	#git mv .\Generate-SqlTriggers.ps1 .\generate-sqltriggers.ps1
	cp .\temp\coeus\shoppingcart-api\remove-migration.ps1
	cp .\temp\coeus\shoppingcart-api\repository.psm1
	cp .\temp\coeus\shoppingcart-api\update-database.ps1

#	((Get-Content -path generate-sqltriggers.ps1 -Raw) -replace 'Acme.ShoppingCart',$repository) | Set-Content -NoNewline -Path generate-sqltriggers.ps1 -Encoding utf8

	if (Test-Path -path "src/sql/TriggerScripts") {
		rm src/sql/TriggerScripts/GenerateTriggers.sql
		rm "src/sql/TriggerScripts/delete all triggers.sql"
		rm src/sql/proc/spWriteStringToFile.proc.sql
		rm src/sql/TriggerScripts
		rm src/sql/proc
	}

	if (Test-Path -path "src/version.json") {
		rm src/version.json
	}
	if (Test-Path -path "set-params.ps1") {
		((Get-Content -path set-params.ps1 -Raw) -replace '\\src\\version.json','\repository.json') | Set-Content -NoNewline -Path set-params.ps1 -Encoding utf8
	}
}

if (Test-Path -path "update-version.ps1") {
	rm update-version.ps1
}

if (Test-Path -path ".\\src\\sql\\table\\AuditLog.table.sql") {
	git mv src\sql\table\AuditLog.table.sql src\sql\table\001-AuditLog.table.sql
}

if (Test-Path -path ".\\src\\sql\\table\\000-AuditLogTransaction.table.sql") {
	cp .\temp\coeus\shoppingcart-api\src\sql\table\000-AuditLogTransaction.table.sql src\sql\table
	cp .\temp\coeus\shoppingcart-api\src\sql\table\001-AuditLog.table.sql src\sql\table
}

# cleanup
rm .\temp\ -Recurse -Force

git status
