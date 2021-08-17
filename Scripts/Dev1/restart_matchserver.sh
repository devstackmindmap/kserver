#!/bin/bash
#dotnet /KServer/ServiceManager/bin/Debug/netcoreapp3.0/ServiceManager.dll match 40554

/KServer/Scripts/Dev1/kill_server.sh m

echo "launching MatchingServer"
cd /KServer/MatchingServer/bin/Debug
mono ./MatchingServer.exe Dev1  $(<~/match_dataversion) 1 1 >/KServer/logs/mlog.out 2>/KServer/logs/mlog.err &
