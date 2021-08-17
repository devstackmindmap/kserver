#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Release/netcoreapp3.0/ServiceManager.dll pubsub 40594

/KServer/Scripts/Review/kill_server.sh p

echo "launching PubSubServer"
cd /KServer/PubSubServer/bin/Release
mono ./PubSubServer.exe Review $(<~/pubsub_dataversion)  >/KServer/logs/plog.out 2>/KServer/logs/plog.err &
