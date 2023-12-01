echo "this is init.sh"

mv appsettings.json appsettings.old.json
jq 'del(.. | .externalProviders?)' appsettings.old.json  > appsettings.json

echo "init.sh done"