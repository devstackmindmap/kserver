#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Debug/netcoreapp3.0/ServiceManager.dll trigger 40694

/KServer/Scripts/Dev2/kill_server.sh sq

echo "launching TriggerServer"
cd /KServer/TriggerServer/bin/Debug
mono ./TriggerServer.exe Dev2 -f square -d $(</KServer/trigger_dataversion)   >/KServer/logs/tlog.out 2>/KServer/logs/tlog.err &
