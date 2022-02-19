[cmdletBinding()]
param(
	[switch]$update=[switch]::Present
)

gci *Test*.csproj -Recurse | % { if (select-string -inputobject $_ -Pattern "coverlet.msbuild") { echo "remove coverlet.msbuild to $_.Fullname"; dotnet remove $_.FullName package coverlet.msbuild } }
gci *Test*.csproj -Recurse | %{ if (-not (select-string -inputobject $_ -Pattern "coverlet.collector")){ echo "add coverlet.collector to $_.Fullname"; dotnet add $_.FullName package coverlet.collector }}

dotnet tool update --global dotnet-outdated-tool
dotnet outdated ./src --version-lock Major --pre-release Never --upgrade
