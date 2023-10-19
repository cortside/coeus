./clean.ps1

./convert-encoding.ps1 -filePaths ((gci *.ps1 -Recurse) | % { $_.FullName })
./convert-encoding.ps1 -filePaths ((gci *.cs -Recurse) | % { $_.FullName })
./convert-encoding.ps1 -filePaths ((gci *.csproj -Recurse) | % { $_.FullName })
./convert-encoding.ps1 -filePaths ((gci *.json -Recurse) | % { $_.FullName })
./convert-encoding.ps1 -filePaths ((gci *.sln -Recurse) | % { $_.FullName })
./convert-encoding.ps1 -filePaths ((gci *.sql -Recurse) | % { $_.FullName })

git status
