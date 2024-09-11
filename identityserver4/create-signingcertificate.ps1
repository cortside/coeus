Param (
	[Parameter(Mandatory = $false)][string]$filepath = "IdentityServer.pfx",
	[Parameter(Mandatory = $false)][string]$password = "1234",
	[Parameter(Mandatory = $false)][int]$years = 10
)

$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';



$Certificate=New-SelfsignedCertificate `
	-KeyExportPolicy Exportable `
	-Subject "CN=IdentityServerSignature" `
	-KeySpec Signature `
	-KeyAlgorithm RSA `
	-KeyLength 2048 `
	-HashAlgorithm SHA256 `
	-CertStoreLocation "cert:\LocalMachine\My" `
	-NotAfter (Get-Date).AddYears($years)

$Pwd = ConvertTo-SecureString -String $password -Force -AsPlainText 
Export-PfxCertificate -Cert $Certificate -FilePath $filepath -Password $Pwd
