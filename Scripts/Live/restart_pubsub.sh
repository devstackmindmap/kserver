#!/bin/bash
#killall mono

/KServer/Scripts/Live/kill_server.sh p

echo "launching PubSubServer"

cd /KServer/PubSubServer/bin/Release
mono ./PubSubServer.exe Live $(</KServer/pubsub_dataversion)  >/KServer/logs/plog.out 2>/KServer/logs/plog.err &
