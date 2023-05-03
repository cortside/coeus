Param (
        [Parameter(Mandatory = $false)][string]$old = "Cortside",
        [Parameter(Mandatory = $false)][string]$new = "Acme"
)

$myExtensions = @("*.cs", "*.csproj", "*.sln", "*.ps1 -exclude clone-project.ps1", "*.sh", "*.json", "*.xml", "*.ncrunchsolution", "*.user", "*.toml", "*.cshtml")

foreach ($extension in $myExtensions) {
  # Replace the names in the files and the files
  $configFiles = iex "Get-ChildItem -Path .\* $extension -rec"
  foreach ($file in $configFiles) {
      ((Get-Content $file.PSPath) -replace "$old", "$new" ) |
      out-file $file.PSPath
	  
      rename-item -path $file.FullName -NewName ($file.name -replace "$old", "$new")
  }
}

gci . -rec -Directory -Filter "*$old*" | foreach-object { rename-item -path $_.FullName -NewName ($_.name -replace "$old", "$new") }
#$newLower = $new.tolower()
#((Get-Content .\deploy\docker\docker-compose.yml) -replace "REPLACE_ME", "$newLower") | out-file -encoding ascii .\deploy\docker\docker-compose.yml

gci . -rec -Directory -Filter "*$old*" | foreach-object { rename-item -path $_.FullName -NewName ($_.name -replace "$old", "$new") }
