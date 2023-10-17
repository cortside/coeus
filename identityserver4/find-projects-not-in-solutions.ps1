<#
.SYNOPSIS
    Find c# projects in subdirectories not included in solution
    with all migrations applied
#>
[cmdletBinding()]
Param
(
	[Parameter(Mandatory = $false)][string]$sln = "./src/Acme.ShoppingCart.sln"
)

Function Global:Get-ProjectInSolution {
    [CmdletBinding()] param (
        [Parameter()][string]$Solution
    )
    $SolutionPath = Join-Path (Get-Location) $Solution
    $SolutionFile = Get-Item $SolutionPath
    $SolutionFolder = $SolutionFile.Directory.FullName

    Get-Content $Solution |
        Select-String 'Project\(' |
        ForEach-Object {
            $projectParts = $_ -Split '[,=]' | ForEach-Object { $_.Trim('[ "{}]') }
            [PSCustomObject]@{
                File = $projectParts[2]
                Guid = $projectParts[3]
                Name = $projectParts[1]
            }
        } |
        Where-Object File -match "csproj$" |
        ForEach-Object {
            Add-Member -InputObject $_ -NotePropertyName FullName -NotePropertyValue (Join-Path $SolutionFolder $_.File) -PassThru
        }
}

Get-ProjectInSolution $sln | select-object Fullname | sort > projects.txt

# add projects not already in solution file
gci *.csproj -r | select-object fullname | %{ 
	$in = Select-String -Path .\projects.txt -SimpleMatch $_.FullName; 
	if ($in -eq $null) {
		echo "Adding $_ to $sln"
		dotnet sln $sln add $_.FullName
	} 
}

