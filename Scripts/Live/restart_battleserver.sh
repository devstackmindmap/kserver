#!/bin/bash
#killall mono

/KServer/Scripts/Live/kill_server.sh b

echo "launching BattleServer"
cd /KServer/BattleServer/bin/Release
mono ./BattleServer.exe Live $(</KServer/battle_dataversion) >/KServer/logs/blog.out 2>/KServer/logs/blog.err &
