#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Release/netcoreapp3.0/ServiceManager.dll web 0

/KServer/Scripts/Review/kill_server.sh g

echo "launching GameServer"
cd /KServer/GameServer
dotnet WebServer.dll Staging  $(<~/game_dataversion)   >/KServer/logs/wlog.out 2>/KServer/logs/wlog.err &
 