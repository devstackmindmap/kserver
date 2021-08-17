#!/bin/bash

case $1 in
    "b") servername="BattleServer" 
         modulename=$servername ;;
    "t") servername="TriggerServer"
         modulename=$servername  ;;
    "p") servername="PubSubServer"
         modulename=$servername  ;;
    "m") servername="MatchingServer"
         modulename=$servername  ;;
    "g") servername="WebServer"
         modulename=$servername  ;;
    "sq") servername="square"
         modulename="TriggerServer"  ;;
    "bsc") servername="battleservercheck"
         modulename="TriggerServer" ;;
    "psc") servername="pubsubservercheck"
         modulename="TriggerServer" ;;
esac


tmpfile="/tmp/processlist_"$servername

echo $1 $2 " terminate processing start"


function mykill
{
    pgrep -af $servername > $tmpfile

    echo "Try Kill Process $1"

    while read cmd; do

        IFS=' ' read -ra args <<< "$cmd"

        pid=${args[0]}
        module=${args[2]}
        runmode=${args[3]}
        data=${args[4]}

        if [[ $module == *"$modulename"* ]]; then
            kill $1  $pid
            echo "Killed $1 : $cmd"
        fi

    done < $tmpfile
}

mykill

pgrep -af $servername > $tmpfile
interval=0
while [[ $(<$tmpfile) != ""  ]]; do
    echo "waiting"
    sleep 1
    ((interval++))
    echo $interval $(<$tmpfile)
    if (( interval > 5 )); then
        mykill -9
        echo "terminated" 
    fi
    
    rm -f $tmpfile
    pgrep -af $servername > $tmpfile
done

rm -f $tmpfile
