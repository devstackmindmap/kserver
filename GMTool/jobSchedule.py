from app.infra.db import *
from datetime import datetime
import json
import redis
from GMTool.settings import g_commonconfig, g_runMode
from pyfcm import FCMNotification

#push_service = FCMNotification(api_key=API_KEY)


def getNoticeList():
	with dbConnector(runMode=g_runMode, dbname='knightrun_gmtool', auto_trans=True) as commander:
		query = 'SELECT seq, runMode, noticeMessage FROM knightrun_gmtool.publicNotice WHERE count > 0 AND startTime BETWEEN date_add(now(), interval -1 hour) AND now();'
		rows = commander.execute(query)

	return rows
		

def sendNoticeMessage(seq, runMode, message ):
	Host = g_commonconfig[runMode]['PUBSUB']['Redis']
	Port = g_commonconfig[runMode]['PUBSUB']['Port']
	Auth = g_commonconfig[runMode]['PUBSUB']['Auth']

	if(Auth):
		r = redis.StrictRedis(host=Host, port=Port, password=Auth) 
		p = r.pubsub()
	else:
		r = redis.StrictRedis(host=Host, port=Port) 
		p = r.pubsub()

	r.publish('PublicNotice', message) 
	with dbConnector(runMode=g_runMode, dbname='knightrun_gmtool', auto_trans=True) as commander:
		query = 'UPDATE publicNotice set count= count - 1 WHERE seq = %s;'
		commander.execute(query, seq)


def main():
	try:
		rows = getNoticeList()
		for row in rows:
			seq = row['seq']
			runMode = row['runMode']
			message = row['noticeMessage']
			sendNoticeMessage(seq, runMode, message)
			print(datetime.now())
			print(row)
	except Exception as e:
		print(tracebak.format_exc())
	finally:
		print('end')

if __name__ == "__main__":
	main()

