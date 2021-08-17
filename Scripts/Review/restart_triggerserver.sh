#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Release/netcoreapp3.0/ServiceManager.dll trigger 40694

/KServer/Scripts/Review/kill_server.sh t

echo "launching TriggerServer"
cd /KServer/TriggerServer/bin/Release
mono ./TriggerServer.exe Review -f square servercheck -d $(<~/trigger_dataversion)   >/KServer/logs/tlog.out 2>/KServer/logs/tlog.err &
