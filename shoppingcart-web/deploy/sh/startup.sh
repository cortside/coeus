#!/bin/bash

# show network information
ifconfig

# copy files from setting volume if they exist
if [[ -d /settings/$SERVICE_NAME ]]; then
	cp -R /settings/$SERVICE_NAME/* /usr/share/nginx/html
fi

# expand env var tokens in files with env var values
find /usr/share/nginx/html -type f -exec /bin/sh -c 'envsubst \''${EXTERNAL_HOST}\'' < $1 > $1.tmp && mv $1.tmp $1' -- {} \;

# extra setup at container startup
if [[ -f /app/init.sh ]]; then
	/app/init.sh
fi

# start nginx
nginx -g "daemon off;"
