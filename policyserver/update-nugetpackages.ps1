[cmdletBinding()]
param(
	[switch]$update=[switch]::Present
)

dotnet tool update --global dotnet-outdated-tool
dotnet outdated ./src --version-lock Major --pre-release Never --upgrade
