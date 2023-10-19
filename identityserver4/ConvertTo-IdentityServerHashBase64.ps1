<#
.SYNOPSIS
  Converts a password string into a hashed+base64-encoded version that can be directly inserted into the IdentityServer user database
.EXAMPLE
  ConvertTo-IdentityServerHashBase64.ps1 'NoC@nHazMyPa55werdz'
  ---------
  Returns the SHA256+base64-encoded password hash for NoC@nHazMyPa55werdz -- this value can then be inserted into the IdentityServer database (ClientSecrets table, Value field).
.DESCRIPTION
  Usage: ConvertTo-IdentityServerHashBase64 -Password [string]
.NOTES
  Author: Steve Witt
  Version 1.0: 07/01/2021 06:36:34 PM
  Converts a password string into a hashed+base64-encoded version that can be directly inserted into the IdentityServer user database. See examples.
  PREREQUISITES:
    A password to convert. Use create-password.ps1 to generate a random one, if needed.
  HELPFUL INFO:
    Here is an example query that can be used to find an existing value in an IdentityServer database:
      SELECT [IdentityServerTestB].[AUTH].[ClientSecrets].[Value]
      FROM [IdentityServerTestB].[AUTH].[Clients]
      LEFT JOIN [IdentityServerTestB].[AUTH].[ClientSecrets] ON [IdentityServerTestB].[AUTH].[Clients].[Id] = [IdentityServerTestB].[AUTH].[ClientSecrets].[ClientId]
      WHERE [IdentityServerTestB].[AUTH].[Clients].[ClientId] = 'automation.nonlender1'
    Dan Witt has a Linux version that he uses to do the same thing. Here's an example of that:
      pass=$(openssl rand -base64 32)
      pass=${pass//[\/+=]/}
      pass=${pass::16}
      clientSecret=$(echo -n $pass | openssl dgst -binary -sha256 | openssl base64)
#>
[cmdletBinding()]
Param
(
		[Parameter(Mandatory = $true)][string]$password
)

$hasher = [System.Security.Cryptography.HashAlgorithm]::Create('sha256')
$hash = $hasher.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($Password))
[Convert]::ToBase64String($hash)
