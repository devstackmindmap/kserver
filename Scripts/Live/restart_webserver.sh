#!/bin/bash
#killall dotnet

/KServer/Scripts/Live/kill_server.sh g

echo "launching GameServer"

cd /KServer/GameServer
dotnet WebServer.dll Live   $(</KServer/game_dataversion)   >/KServer/logs/wlog.out 2>/KServer/logs/wlog.err &
 