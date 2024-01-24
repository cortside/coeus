$tokens = @{ 
	"shoppingcart-api" = $env:SHOPPINGCART_API_TOKEN;
	"shoppingcart-web" = $env:SHOPPINGCART_WEB_TOKEN; 
	"identityserver4" = $env:IDENTITYSERVER4_TOKEN; 
	"identityServer6" = $env:IDENTITYSERVER6_TOKEN; 
	"policyserver" = $env:POLICYSERVER_TOKEN; 
}

$buildNumber = $env:APPVEYOR_BUILD_NUMBER;

if (-not (Test-Path env:APPVEYOR_PULL_REQUEST_NUMBER)) {
	$branch = $Env:APPVEYOR_REPO_BRANCH;
	if ($branch -ne "master") {
		$target = "develop";
		if ($branch -eq "develop" -or $branch -like "release/*" -or $branch -like "hotfix/*") {
			$target = "master";
		}
	}
} else {
	$branch = $Env:APPVEYOR_PULL_REQUEST_HEAD_REPO_BRANCH;
	$target = $Env:APPVEYOR_REPO_BRANCH;
	$commit = $Env:APPVEYOR_PULL_REQUEST_HEAD_COMMIT;
	$pullRequestId = $Env:APPVEYOR_PULL_REQUEST_NUMBER;
}

echo "building version $version from branch $branch targeting $target (pullRequestId=$pullRequestId, commit=$commit)";
Write-Host Starting build

$files = ""
if ( $env:APPVEYOR_PULL_REQUEST_NUMBER ) {
  Write-Host Pull request $env:APPVEYOR_PULL_REQUEST_NUMBER
  $files = $(git --no-pager diff --name-only ..$target)
} else {
  Write-Host Branch $env:APPVEYOR_REPO_BRANCH
  $files = $(git diff --name-only HEAD~1)
}

Write-Host Changed files: 
$dirs = @{}
$files | ForEach-Object {
  Write-Host $_
  $dir = $_ -replace "\/[^\/]+$", ""
  $dir = $dir -replace "/", "\"
  $rootdir = $dir.split("\",3)[0]
  if (Test-Path "$rootdir\build-dockerimages.ps1") {
	Write-Host "Storing $rootdir for build"
	$dirs.Set_Item($rootdir, 1)
  } else {
	$dir = $dir -replace "\\[^\\]+$", ""
	#if (Test-Path "$rootdir\build-dockerimages.ps1") {
	if (Test-Path "$rootdir\Dockerfile.*") {
	  Write-Host "Storing $rootdir for build"
	  $dirs.Set_Item($rootdir, 1)
	}
  }
}

$dirs.GetEnumerator() | Sort-Object Name | ForEach-Object {
	$dir = $_.Name
	Write-Host ""
	Write-Host "=========================="
	Write-Host "Building in directory $dir"
	Write-Host "=========================="

	cd $dir
	Write-Host "Current directory: $pwd"
	
	$env:SONAR_TOKEN = $tokens[$dir];
	.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber -pushImage -target $target -commit $commit -pullRequestId $pullRequestId;
	cd $PSScriptRoot
}

Write-Host ""
Write-Host "=========================="
Write-Host "done"
Write-Host "=========================="