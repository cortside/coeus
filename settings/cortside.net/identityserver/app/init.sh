echo "this is init.sh"

cp appsettings.json appsettings.old.json

ls -Al *.json

jq 'del(.. | .externalProviders?)' appsettings.old.json  > appsettings.json

ls -Al *.json

echo "init.sh done"