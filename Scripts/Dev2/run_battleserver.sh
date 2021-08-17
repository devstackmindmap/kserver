/KServer/Scripts/Common/kill_battleserver.sh

cd /KServer/BattleServer/bin
mono ./BattleServer.exe Dev2 >../log.out 2>../log.err &
