#!/bin/bash

/bin/dotnet /KServer/UserSeasonUpdator/bin/Release/netcoreapp3.0/UserSeasonUpdator.dll $1 $2 >> /KServer/logs/UserSeasonUpdate.log
