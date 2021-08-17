mono ./BattleServer/BattleServer.exe Dev1 0 &
mono ./MatchingServer/MatchingServer.exe Dev1 0 &
mono ./PubSubServer/PubSubServer.exe Dev1 0 &
mono ./TriggerServer/TriggerServer.exe Dev1 -f battleservercheck -p 40699 -d 0 &
mono ./TriggerServer/TriggerServer.exe Dev1 -f pubsubservercheck -p 40700 -d 0 &