#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Release/netcoreapp3.0/ServiceManager.dll trigger 40699

/KServer/Scripts/Dev2/kill_server.sh bsc
/KServer/Scripts/Dev2/kill_server.sh psc

echo "launching TriggerServer : Battle Server Scheck"

cd /KServer/TriggerServer/bin/Debug
mono ./TriggerServer.exe Dev2 -f battleservercheck -p 40699 -d $(<~/trigger_dataversion)  >/KServer/logs/tblog.out 2>/KServer/logs/tblog.err &


echo "launching TriggerServer : pubsub Server Scheck"

mono ./TriggerServer.exe Dev2 -f pubsubservercheck  -p 40700 -d $(<~/trigger_dataversion)  >/KServer/logs/tblog.out 2>/KServer/logs/tplog.err &