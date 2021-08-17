#!/bin/bash
#killall mono

/KServer/Scripts/Live/kill_server.sh m

echo "launching MatchingServer"
cd /KServer/MatchingServer/bin/Release
mono ./MatchingServer.exe Live $(</KServer/match_dataversion) 1 2 >/KServer/logs/mlog.out 2>/KServer/logs/mlog.err &
