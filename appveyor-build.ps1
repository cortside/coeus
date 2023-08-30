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
  $files = $(git --no-pager diff --name-only FETCH_HEAD $(git merge-base FETCH_HEAD main))
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
  Write-Host "Checking $dir for build script"
  if (Test-Path "$dir\build-dockerimages.ps1") {
	Write-Host "Storing $dir for build"
	$dirs.Set_Item($dir, 1)
  } else {
	$dir = $dir -replace "\\[^\\]+$", ""
	#if (Test-Path "$dir\build-dockerimages.ps1") {
	if (Test-Path "$dir\Dockerfile.*") {
	  Write-Host "Storing $dir for build"
	  $dirs.Set_Item($dir, 1)
	}
  }
}

$dirs.GetEnumerator() | Sort-Object Name | ForEach-Object {
	$dir = $_.Name
	Write-Host Building in directory $dir
	
	cd $dir
	# need to create dictionary to look these up by $dir name
	$env:SONAR_TOKEN = $env:SHOPPINGCART_API_TOKEN;
	.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber -pushImage -target $target -commit $commit -pullRequestId $pullRequestId;
	cd $PSScriptRoot
}

cd shoppingcart-api;
$env:SONAR_TOKEN = $env:SHOPPINGCART_API_TOKEN;
#.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber -pushImage -target $target -commit $commit -pullRequestId $pullRequestId;
cd ..;
cd shoppingcart-web;
$env:SONAR_TOKEN = $env:SHOPPINGCART_WEB_TOKEN;
#.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber -pushImage -target $target -commit $commit -pullRequestId $pullRequestId;
cd ..;
cd identityServer6;
$env:SONAR_TOKEN = $env:IDENTITYSERVER6_TOKEN;
#.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber -pushImage -target $target -commit $commit -pullRequestId $pullRequestId;
cd ..;
cd policyserver;
$env:SONAR_TOKEN = $env:POLICYSERVER_TOKEN;
#.\build-dockerimages.ps1 -branch $branch -buildCounter $buildNumber -pushImage -target $target -commit $commit -pullRequestId $pullRequestId;
cd ..;
