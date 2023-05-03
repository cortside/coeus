[cmdletbinding()]
Param()

Function check-result {
	if ($LastExitCode -ne 0) {
		$e = [char]27
		$start = "$e[1;31m"
		$end = "$e[m"
		$text = "ERROR: Exiting with error code $LastExitCode"
		Write-Error "$start$text$end"
		return $false
		exit 1 
	}
	return $true
}

Function Invoke-Exe {
Param(
    [parameter(Mandatory=$true)][string] $cmd,
    [parameter(Mandatory=$true)][string] $args
	
)
	Write-Host "Executing: `"$cmd`" --% $args"
	Invoke-Expression "& `"$cmd`" --% $args"
	$result = check-result
	if (!$result) {
		throw "ERROR executing EXE"
	}
}

# recursively find test assemblies and invoke them
dotnet test ./src --no-restore
