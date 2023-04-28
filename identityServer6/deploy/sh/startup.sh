#!/bin/bash

ifconfig
find . -maxdepth 1 -type f -executable
ls -Al

./Acme.IdentityServer
