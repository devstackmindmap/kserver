#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Release/netcoreapp3.0/ServiceManager.dll trigger 40694

/KServer/Scripts/Review/kill_server.sh t

echo "launching TriggerServer"
cd /KServer/TriggerServer/bin/Debug
mono ./TriggerServer.exe Staging -f square servercheck -d $(<~/trigger_dataversion)   >/KServer/logs/tlog.out 2>/KServer/logs/tlog.err &
