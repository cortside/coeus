.\makecert.exe -r -pe -n "CN=IdentityServer" -b 08/01/2018 -e 12/31/2099 -sky signature -a sha512 -len 2048 -sv IdentityServer.pvk IdentityServer.cer
.\pvk2pfx.exe -pvk IdentityServer.pvk -spc IdentityServer.cer -pfx IdentityServer.pfx
