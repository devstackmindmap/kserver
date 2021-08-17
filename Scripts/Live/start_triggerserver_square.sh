#!/bin/bash

cd /KServer/TriggerServer/bin/Release
mono ./TriggerServer.exe Live -f square -d $(</KServer/trigger_dataversion)   >/KServer/logs/tlog.out 2>/KServer/logs/tlog.err &
