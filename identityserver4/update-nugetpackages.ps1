[cmdletBinding()]
param(
	[switch]$update=[switch]::Present
)

dotnet outdated ./src --version-lock Major -u
