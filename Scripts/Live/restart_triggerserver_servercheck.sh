#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Release/netcoreapp3.0/ServiceManager.dll trigger 40699

/KServer/Scripts/Live/kill_server.sh bsc
/KServer/Scripts/Live/kill_server.sh psc

echo "launching TriggerServer : Battle Server Scheck"

cd /KServer/TriggerServer/bin/Release
mono ./TriggerServer.exe Live -f battleservercheck -p 40699 -d $(</KServer/trigger_dataversion)  >/KServer/logs/tblog.out 2>/KServer/logs/tblog.err &


echo "launching TriggerServer : pubsub Server Scheck"

mono ./TriggerServer.exe Live -f pubsubServercheck  -p 40700 -d $(</KServer/trigger_dataversion)  >/KServer/logs/tblog.out 2>/KServer/logs/tplog.err &