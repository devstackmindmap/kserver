#!/bin/bash
# dotnet /KServer/ServiceManager/bin/Release/netcoreapp3.0/ServiceManager.dll battle 30654

/KServer/Scripts/Review/kill_server.sh b

echo "launching BattleServer"
cd /KServer/BattleServer/bin/Release
mono ./BattleServer.exe Review $(<~/battle_dataversion) >/KServer/logs/blog.out 2>/KServer/logs/blog.err &
