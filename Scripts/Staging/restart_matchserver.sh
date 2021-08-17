#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Release/netcoreapp3.0/ServiceManager.dll match 40554

/KServer/Scripts/Review/kill_server.sh m

echo "launching MatchingServer"
cd /KServer/MatchingServer/bin/Debug
mono ./MatchingServer.exe Staging $(<~/match_dataversion) 1 1 >/KServer/logs/mlog.out 2>/KServer/logs/mlog.err &
