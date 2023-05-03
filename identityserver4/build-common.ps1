Function check-result {
	if ($LastExitCode -ne 0) {
		$text = "ERROR: Exiting with error code $LastExitCode"
		Write-Error "##teamcity[buildStatus status='$text']"
    	if ($env:TEAMCITY_VERSION) { [System.Environment]::Exit(1) } else { throw $text } 
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

function New-BuildJson{
Param (
	[string]$versionjsonpath,
  	[string]$buildjsonpath,
    [string]$buildCounter
)

	$version = Get-Content $versionjsonpath -raw | Convertfrom-json
	$BuildObject = New-Object -TypeName psobject        
	$Build = New-Object -TypeName psobject
	$builditems = [ordered] @{
		"version" = ""
		"timestamp" = ""
		"tag" = ""
		"suffix" = ""
		"octopusPrefix" = ""
	}
	
	$LeadingVersion = $version.version
	$NewBuildVersion = "$LeadingVersion.$buildCounter"
	$buildTime = (Get-Date).ToUniversalTime().ToString("u")
	$builditems.version = $NewBuildVersion
	$builditems.timestamp = $buildTime 
	$builditems.octopusPrefix = $version.octopusPrefix
	
	Foreach ( $item in $builditems.Keys ) {
		 $build | Add-Member -MemberType NoteProperty -Name $item  -Value $builditems.$item
	}

	$BuildObject | Add-Member -MemberType NoteProperty -Name Build -Value $build
	
	$BuildObject| convertto-json -depth 5 | out-file $buildjsonpath -force 
	
	return $buildobject
}

function Set-DockerTag {
Param(
	[string]$branch,
    [string]$buildNumber,
    [string]$buildjsonpath
)

	$build = Get-Content $buildjsonpath -raw | Convertfrom-json

	$lastOctet = $buildNumber.lastindexof(".")
    $LeadingVersion = $buildNumber.substring(0,$lastOctet)
	$dockertagprefix = "$LeadingVersion-"
	
	# overrides by branch
	if ($branch.contains("master"))
	{
		$suffix =$null
		$dockertagprefix = "$buildNumber"	
		$msbuildconfig = "Release"
		$OctopusChannel = "$($build.Build.octopusPrefix)-Production"
	}
	elseif ($branch.contains("feature") -or $branch.contains("hotfix") -or $branch.contains("bugfix"))
	{
	    write-output "using $branch"
	    $JiraKey = ($branch | Select-String -Pattern '((?<!([A-Z]{1,10})-?)[A-Z]+-\d+)' | % matches).value
		
	    if ($JiraKey -eq $null)
	    {
	        write-error "Please re-create branch via jira for valid jira ticket"
	    }
	    else
	    {
             $JiraBuildKey = $JiraKey.Replace("-","")	  
			 $suffix = "$JiraBuildkey"
			 $OctopusChannel = "$($build.Build.octopusPrefix)-Interim"
	    }
	}
	elseif ($branch.contains("develop") -or $branch.contains("Develop"))
	{
			$suffix = "develop"
			$OctopusChannel = "$($build.Build.octopusPrefix)-Development"
	}
	elseif ($branch.contains("release"))
	{
			$suffix = "release"
			$OctopusChannel = "$($build.Build.octopusPrefix)-Staging"
	}
    elseif ($branch.contains("local"))
	{
			$suffix = "local"
			$OctopusChannel = "$($build.Build.octopusPrefix)-Interim"
	}
	else
	{
	    write-warning "using custom $branch please use one of the existing base branches or properly use a defined branch type"
	}

	 $dockertag = "${dockertagprefix}${suffix}"
    
     $build.build.tag = $dockertag 
	 $build.build.suffix = $suffix
  
     $build | convertto-json -depth 5 | out-file $buildjsonpath -force 
	 
	 # After the outfile as I do not need this on the JSON for build, only for CI 
	 $build.build | Add-Member -MemberType NoteProperty -Name channel  -Value $OctopusChannel 
	 
     return $build
}
