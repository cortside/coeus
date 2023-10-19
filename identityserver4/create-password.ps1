<#
.SYNOPSIS
  Creates a random password to use for clients, with ConvertTo-IdentityServerHashBase64.ps1
.EXAMPLE
  ./create-password.ps1
  ---------
  Outputs a randomly generated string with a default 16 digit alphanumeric upper & lower characters.
.DESCRIPTION
  Usage: ./create-password.ps1 -length 32
#>
[cmdletBinding()]
Param
(
    [Parameter(Mandatory = $false)][int]$length = 16
)

-join (((48..57) + (65..90) + (97..122)) * 80 | Get-Random -Count $length | % { [char]$_ })
