[CmdletBinding()]
Param
(
	[Parameter(Mandatory=$false)][string]$branch = "local",
	[Parameter(Mandatory=$false)][string]$buildCounter = "0",
	[Parameter(Mandatory=$false)][string]$msbuildconfig = "Debug"
)

Function Get-Result {
	if ($LastExitCode -eq 0) {
		return $true
	}

	$text = "ERROR: Exiting with error code $LastExitCode"
	Write-Error "$text"

	return $false
}

Function Invoke-Exe {
	Param (
		[Parameter(Mandatory=$true)][string] $cmd,
		[Parameter(Mandatory=$true)][string] $args
	)

	Write-Host "Executing: `"$cmd`" --% $args"
	Invoke-Expression "& `"$cmd`" --% $args"

	if (Get-Result -eq $false) {
		exit 1
	}
}

Function New-BuildJson {
	Param (
		[Parameter(Mandatory=$true)][string]$versionjsonpath,
		[Parameter(Mandatory=$true)][string]$buildjsonpath,
		[Parameter(Mandatory=$true)][string]$buildCounter
	)

	$version = Get-Content $versionjsonpath -raw | ConvertFrom-Json
	$buildobject = New-Object -TypeName psobject
	$build = New-Object -TypeName psobject
	$builditems = [ordered] @{
		"version" = ""
		"timestamp" = ""
		"tag" = ""
		"suffix" = ""
	}

	$NewBuildVersion = "$($version.version).$buildCounter"
	$buildTime = (Get-Date).ToUniversalTime().ToString("u")
	$builditems.version = $NewBuildVersion
	$builditems.timestamp = $buildTime
	$builditems.Keys |% { $build | Add-Member -MemberType NoteProperty -Name $_ -Value $builditems.$_ } > $null
	
	$buildobject | Add-Member -MemberType NoteProperty -Name Build -Value $build
	$buildobject | ConvertTo-Json -Depth 5 | Out-File $buildjsonpath -force

	return $buildobject
}

function Set-DockerTag {
	Param(
		[Parameter(Mandatory=$true)][string]$branch,
		[Parameter(Mandatory=$true)][string]$buildNumber,
		[Parameter(Mandatory=$true)][string]$buildjsonpath
	)

	$lastOctet = $buildNumber.lastindexof(".")
	$LeadingVersion = $buildNumber.substring(0,$lastOctet)
	$dockertagprefix = "$LeadingVersion-"
	$nugetfeedkey = $env:DevelopNugetFeedKey
	$nugetfeed = $env:DevelopNugetFeed
        $OctopusChannel = "Non-Production"
        $OctopusAutoDeployEnvironment = ""

	# overrides by branch
	if ($branch -like "master")
	{
		$suffix =$null
		$dockertagprefix = "$buildNumber"
		$msbuildconfig = "Release"
		$OctopusChannel = "Production"
                $OctopusAutoDeployEnvironment = "dev, tools, Demo, Integration, UAT"
		$nugetfeedkey = $env:MasterNugetFeedkey
		$nugetfeed = $env:MasterNugetFeed
	}
	elseif ($branch -like "feature/*" -or $branch -like "hotfix/*" -or $branch -like "bugfix/*")
	{
		Write-Output "using $branch"
		$JiraKey = ($branch | Select-String -Pattern '((?<!([A-Z]{1,10})-?)[A-Z]+-\d+)' | % matches).value

		if ($null -eq $JiraKey)
		{
			write-error "Please re-create branch via jira for valid jira ticket"
		}
		else
		{
			$JiraBuildKey = $JiraKey.Replace("-","")
			$suffix = "$JiraBuildkey"
		}
	}
	elseif ($branch -like "develop")
	{
		$suffix = "develop"
                $OctopusAutoDeployEnvironment = "dev"
	}
	elseif ($branch -like "release*")
	{
		$suffix = "release"
                $OctopusAutoDeployEnvironment = "dev, tools, Demo, Integration, UAT"
	}
	elseif ($branch.contains("local"))
	{
		$suffix = "local"
	}
	else
	{
		write-warning "using custom $branch please use one of the existing base branches or properly use a defined branch type"
	}

	$build = Get-Content $buildjsonpath -raw | Convertfrom-json
	$build.build.tag = "${dockertagprefix}${suffix}"
	$build.build.suffix = $suffix

	$build | convertto-json -depth 5 | out-file $buildjsonpath -force

	# After the outfile as I do not need this on the JSON for build, only for CI
	$build.build | Add-Member -MemberType NoteProperty -Name channel  -Value $OctopusChannel
	$build.build | Add-Member -MemberType NoteProperty -Name nugetfeed  -Value $nugetfeed
	$build.build | Add-Member -MemberType NoteProperty -Name nugetfeedkey  -Value $nugetfeedkey
	$build.build | Add-Member -MemberType NoteProperty -Name buildconfig  -Value $msbuildconfig
        $build.build | Add-Member -MemberType NoteProperty -Name OctopusAutoDeployEnvironment -Value $OctopusAutoDeployEnvironment

	return $build
}

# generate build.json
$BuildNumber = (New-BuildJson -versionJsonPath $PSScriptRoot\src\version.json -BuildJsonPath $PSScriptRoot\src\build.json -buildCounter $buildCounter).build.version
Write-Host "##teamcity[buildNumber '$BuildNumber']"
$build = Set-DockerTag -branch $branch -buildNumber $BuildNumber -BuildJsonPath $PSScriptRoot\src\build.json
$dockertag = $build.build.tag
$suffix = $build.build.suffix
$nugetfeed = $build.build.nugetfeed
$nugetfeedkey = $build.build.nugetfeedkey
$OctopusChannel = $build.build.channel
$msbuildconfig = $build.build.buildconfig
$OctopusAutoDeployEnvironment = $build.build.OctopusAutoDeployEnvironment
"Dockertag for this build: $dockertag",
	"Suffix for this build: $suffix",
	"OctopusChannel for this build: $OctopusChannel",
        "Octopus Auto Deploy Channel: $OctopusAutoDeployEnvironment",
	"Using NugetFeed: $nugetfeed" | Write-Output

Write-Host "##teamcity[setParameter name='env.dockertag' value='$dockertag']"
Write-Host "##teamcity[setParameter name='env.OctopusChannel' value='$OctopusChannel']"
Write-Host "##teamcity[setParameter name='env.suffix' value='$suffix']"
Write-Host "##teamcity[setParameter name='env.MsBuildConfig' value='$msbuildconfig']"
Write-Host "##teamcity[setParameter name='env.NugetFeedPublish' value='${nugetfeed}']"
Write-Host "##teamcity[setParameter name='env.NugetFeedPublishKey' value='${nugetfeedkey}']"
Write-Host "##teamcity[setParameter name='env.OctopusAutoDeployEnvironment' value='$OctopusAutoDeployEnvironment']"

$OctopusVersion = if ($suffix) { "$BuildNumber-$suffix" } else { "$BuildNumber" }
Write-Host "##teamcity[setParameter name='env.OctopusVersion' value='$OctopusVersion']"

# copy generated build.json to needed applications
Copy-Item .\src\build.json .\src\Cortside.IdentityServer.WebApi\build.json -force
Copy-Item .\src\build.json .\src\Cortside.IdentityServer.WebApi.IntegrationTests\build.json -force

