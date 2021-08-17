#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Debug/netcoreapp3.0/ServiceManager.dll battle 30654

/KServer/Scripts/Dev2/kill_server.sh b

echo "launching BattleServer"
cd /KServer/BattleServer/bin/Debug
mono ./BattleServer.exe Dev2  $(<~/battle_dataversion) >/KServer/logs/blog.out 2>/KServer/logs/blog.err &
