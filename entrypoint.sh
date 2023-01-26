#!/bin/sh
envsubst < appsettings.json.template > appsettings.json
dotnet $1
exec "$@"
