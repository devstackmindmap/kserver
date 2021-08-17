from db import *
from datetime import datetime, time
import json
import redis
from settings import g_commonconfig, g_runMode, API_KEY, game_gamedbconfig
from pyfcm import FCMNotification

push_service = FCMNotification(api_key=API_KEY)


def getPushKeyList(row):
	cond = row['cond']
	runMode = row['runMode']
	now = datetime.now()
	nowTime = now.time()
	nowDate = now.date()
	
	nums = game_gamedbconfig[runMode]['GameDBSetting']['UserDBSetting']
	keys = []

	for num in nums:
		with gdbConnector(runMode=runMode, dbname=num, auto_trans=True) as commander:
			if cond  == 'allUser':
				if nowTime  >= time(11,30) or nowTime <= time(23,00):
					query = 'SELECT pushKey FROM knightrun.pushkeys WHERE pushAgree = 1 and nightPushAgree = 1 and pushkey != "";'
					res = commander.execute(query)
					keys.extend(res)
					
				else:
					query = 'SELECT pushKey FROM knightrun.pushkeys WHERE pushAgree = 1 and pushkey != "" ;'
					res = commander.execute(query)
					keys.extend(res)
					
			if cond == 'lastLogin5':
				if nowTime  >= time(11,30) or nowTime <= time(23,00):
					query = 'SELECT a.pushKey  FROM knightrun.pushkeys as a inner join account.accounts as b on a.userId = b.userId WHERE a.pushAgree = 1 and pushkey != "" and a.nightPushAgree = 1 and b.loginDateTime between {0} and {1};'.format(nowDate + timedelta(days=-10), nowDate + timedelta(days=-5))
					res = commander.execute(query)
					keys.extend(res)
					
				else:
					query = 'SELECT a.pushKey  FROM knightrun.pushkeys as a inner join account.accounts as b on a.userId = b.userId WHERE a.pushAgree = 1 and pushkey != "" and b.loginDateTime between {0} and {1};'.format(nowDate + timedelta(days=-10), nowDate + timedelta(days=-5))
					res = commander.execute(query)
					keys.extend(res)
					
	return keys

def sendPushMessage(keys, row):
	keyLength = len(keys)
	count = 0
	ids=[]
	title = row['title']
	message = row['message']
	for key in keys:
		ids.append(key['pushKey'])
		count = count +1
		if ((count%1000) == 0 or count == keyLength):
			result = push_service.notify_multiple_devices(registration_ids = ids, message_title =title , message_body=message)
			pushCastId = result['multicast_ids'][0]
			successCount = result['success']
			failureCount = result['failure']
			with dbConnector(runMode=g_runMode, dbname='knightrun_gmtool', auto_trans=True) as command:
				command.execute("INSERT INTO push_log(pushCastId, reservationNum, title, message, successCount, failure, sendDateTime) VALUES(%s, %s, %s, %s, %s, %s, now())", pushCastId, row['seq'], title, message, successCount, failureCount)
				command.execute("UPDATE pushReservation set sendLog = %s where seq = %s", pushCastId, row['seq'])
			ids = []
	return 
	
	

def main():
	with dbConnector(runMode=g_runMode, dbname='knightrun_gmtool') as commander:
		query = 'SELECT seq, title, message, cond, runMode FROM pushReservation WHERE sendLog < 1 and reservationTime between date_add(now(), interval -10 Minute) and now();'
		rows = commander.execute(query)
		for row in rows:
			keys = getPushKeyList(row)
			if(keys):
				sendPushMessage(keys, row)


main()