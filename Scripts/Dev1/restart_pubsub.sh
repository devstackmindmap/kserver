#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Debug/netcoreapp3.0/ServiceManager.dll pubsub 40594

/KServer/Scripts/Dev1/kill_server.sh p

echo "launching PubSubServer"
cd /KServer/PubSubServer/bin/Debug
mono ./PubSubServer.exe Dev1  $(<~/pubsub_dataversion)  >/KServer/logs/plog.out 2>/KServer/logs/plog.err &
