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
write-verbose "outside gci"
gci -Recurse @("*.Tests.csproj") | % {
	write-verbose "before if"
	if ($_.Directory.FullName -like "*Tests") {
		write-verbose "in the if"
		$directory = Resolve-Path -path $_.Directory.FullName -Relative
		write-verbose $directory
		$args = "test $directory --no-build --no-restore"
		write-verbose $args
		Invoke-Exe -cmd dotnet -args $args
	}
}
