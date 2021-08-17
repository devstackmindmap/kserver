#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Release/netcoreapp3.0/ServiceManager.dll trigger 40694

/KServer/Scripts/Live/kill_server.sh sq

echo "launching TriggerServer : SquareObject"

cd /KServer/TriggerServer/bin/Release
mono ./TriggerServer.exe Live -f square -d $(</KServer/trigger_dataversion)   >/KServer/logs/tlog.out 2>/KServer/logs/tlog.err &
