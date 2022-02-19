$a = Get-Content './src/version.json' -raw | ConvertFrom-Json

echo $a.version
$version = [version] $a.version

$versionStringIncremented =  [string] [version]::new(
  $version.Major,
  $version.Minor+1
)

echo $versionStringIncremented
$a.version = $versionStringIncremented

$a | ConvertTo-Json -depth 32| set-content './src/version.json'

