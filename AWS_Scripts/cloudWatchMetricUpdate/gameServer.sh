#!/bin/bash
INST=$'Game01'
USEDMEMORY=$(free -m | awk 'NR==2{printf "%.2f\t", $3*100/$2 }')
TCP_CONN=$(netstat -an | wc -l)
TCP_CONN_PORT_40654=$(netstat -an | grep 40654 | wc -l)
USERS=$(uptime |awk '{ print $6 }')
IO_WAIT=$(iostat | awk 'NR==4 {print $5}')

aws cloudwatch put-metric-data --metric-name memory-usage --dimensions Instance=$INST  --namespace "Custom" --value $USEDMEMORY
aws cloudwatch put-metric-data --metric-name Tcp_connections --dimensions Instance=$INST  --namespace "Custom" --value $TCP_CONN
aws cloudwatch put-metric-data --metric-name TCP_connection_on_port_40654 --dimensions Instance=$INST  --namespace "Custom" --value $TCP_CONN_PORT_40654
aws cloudwatch put-metric-data --metric-name No_of_users --dimensions Instance=$INST  --namespace "Custom" --value $USERS
aws cloudwatch put-metric-data --metric-name IO_WAIT --dimensions Instance=$INST  --namespace "Custom" --value $IO_WAIT
