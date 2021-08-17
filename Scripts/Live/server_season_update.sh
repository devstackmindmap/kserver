#!/bin/bash

/bin/dotnet /KServer/ServerSeasonUpdator/bin/Release/netcoreapp3.0/ServerSeasonUpdator.dll $1 $2 >> /KServer/logs/ServerUpdate.log

