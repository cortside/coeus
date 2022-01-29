[cmdletbinding()]
param(
[string]$branch = "local",
[string]$buildCounter = "0",
[string]$msbuildconfig = "Debug"
)

dotnet build ./src
