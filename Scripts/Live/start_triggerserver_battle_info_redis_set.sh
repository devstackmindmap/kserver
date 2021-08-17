#!/bin/bash

cd /KServer/TriggerServer/bin/Release
mono ./TriggerServer.exe Live -f servercheck -p 40699  >/KServer/logs/tblog.out 2>/KServer/logs/tblog.err &
