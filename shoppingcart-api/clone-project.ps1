[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)][string]$myNewProjectName
)
  
$myExtensions = @("*.cs", "*.csproj", "*.sln", "*.ps1 -exclude clone-project.ps1", "*.sh", "*.json", "*.xml", "*.ncrunchsolution", "*.user")

foreach ($extension in $myExtensions) {
  # Replace the names in the files and the files
  $configFiles = iex "Get-ChildItem -Path .\* $extension -rec"
  foreach ($file in $configFiles) {
    ((Get-Content $file.PSPath) -replace "WebApiStarter", "$myNewProjectName" ) |
    out-file $file.PSPath
    rename-item -path $file.FullName -NewName ($file.name -replace 'WebApiStarter', "$myNewProjectName")
  }
}

gci . -rec -Directory -Filter *Template* | foreach-object { rename-item -path $_.FullName -NewName ($_.name -replace 'WebApiStarter', "$myNewProjectName") }

$myNewProjectName = 'Organization'
  
$myExtensions = @("*.cs", "*.csproj", "*.sln", "*.ps1 -exclude clone-project.ps1", "*.sh", "*.json", "*.xml", "*.ncrunchsolution", "*.user")

foreach ($extension in $myExtensions) {
  # Replace the names in the files and the files
  $configFiles = iex "Get-ChildItem -Path .\* $extension -rec"
  foreach ($file in $configFiles) {
    ((Get-Content $file.PSPath) -replace "Acme", "$myNewProjectName" ) |
    out-file $file.PSPath
    rename-item -path $file.FullName -NewName ($file.name -replace 'Acme', "$myNewProjectName")
  }
}

gci . -rec -Directory -Filter *Acme* | foreach-object { rename-item -path $_.FullName -NewName ($_.name -replace 'Acme', "Organization") }



# make sure all file encoding is good
& "$PSScriptRoot\convert-encoding.ps1" -filePaths (gci -Exclude .git*, *.dll -Recurse | where { $_.Attributes -ne 'Directory' } | % { $_.FullName })
