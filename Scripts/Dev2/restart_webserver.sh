#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Debug/netcoreapp3.0/ServiceManager.dll web 0

/KServer/Scripts/Dev2/kill_server.sh g

echo "launching GameServer"
cd /KServer/GameServer
dotnet WebServer.dll Dev2  $(<~/game_dataversion)  >/KServer/logs/wlog.out 2>/KServer/logs/wlog.err &
 