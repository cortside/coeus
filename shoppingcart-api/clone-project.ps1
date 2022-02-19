[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)][string]$myNewProjectName
)

$oldProjectName = "WebApiStarter"
$myNewProjectName = "ShoppingCart"
  
$myExtensions = @("*.cs", "*.csproj", "*.sln", "*.ps1 -exclude clone-project.ps1", "*.sh", "*.json", "*.xml", "*.ncrunchsolution", "*.user")

foreach ($extension in $myExtensions) {
  # Replace the names in the files and the files
  $configFiles = iex "Get-ChildItem -Path .\* $extension -rec"
  foreach ($file in $configFiles) {
    ((Get-Content $file.PSPath) -replace "$oldProjectName", "$myNewProjectName" ) |
    out-file $file.PSPath
    rename-item -path $file.FullName -NewName ($file.name -replace "$oldProjectName", "$myNewProjectName" )
  }
}

$filter = "*$oldProjectName*"
gci . -rec -Directory -Filter $filter | foreach-object { rename-item -path $_.FullName -NewName ($_.name -replace "$oldProjectName", "$myNewProjectName" ) }

# $myNewProjectName = 'Organization'
  
# $myExtensions = @("*.cs", "*.csproj", "*.sln", "*.ps1 -exclude clone-project.ps1", "*.sh", "*.json", "*.xml", "*.ncrunchsolution", "*.user")

# foreach ($extension in $myExtensions) {
  # # Replace the names in the files and the files
  # $configFiles = iex "Get-ChildItem -Path .\* $extension -rec"
  # foreach ($file in $configFiles) {
    # ((Get-Content $file.PSPath) -replace "Acme", "$myNewProjectName" ) |
    # out-file $file.PSPath
    # rename-item -path $file.FullName -NewName ($file.name -replace 'Acme', "$myNewProjectName")
  # }
# }

# gci . -rec -Directory -Filter *Acme* | foreach-object { rename-item -path $_.FullName -NewName ($_.name -replace 'Acme', "Organization") }



# make sure all file encoding is good
& "$PSScriptRoot\convert-encoding.ps1" -filePaths (gci -Exclude .git*, *.dll -Recurse | where { $_.Attributes -ne 'Directory' } | % { $_.FullName })
