from app.infra.db import *
from datetime import datetime, time
import json
from GMTool.settings import g_commonconfig, g_runMode, API_KEY, game_gamedbconfig
from pyfcm import FCMNotification

push_service = FCMNotification(api_key=API_KEY)


def sendPushMessage(keys):
	keyLength = len(keys)
	count = 0
	ids=[]
	title = '스퀘어 오브젝트가 습격당했습니다!'
	message = ''
	for key in keys:
		ids.append(key['pushKey'])
		
		count = count +1
		if ((count%1000) == 0 or count == keyLength):
			result = push_service.notify_multiple_devices(registration_ids = ids, message_title =title , message_body=message)
			pushCastId = result['multicast_ids'][0]
			successCount = result['success']
			failureCount = result['failure']
			print('pushId : ', pushCastId )
			print('pushSuccess : ', successCount)
			print('pushFailuer : ', failureCount)
			ids = []
	return 
	
	

def main():
	try:
		now = datetime.now()
		nowTime = now.time()
		
		with dbConnector(runMode=g_runMode, dbname='account') as commander:
			query = 'select seq from _maintenance_time where  starttime <= now() and endTime >= now();'
			rows = commander.execute(query)
			if len(rows) < 1:
				if nowTime  >= time(11,30) and nowTime <= time(23,00):
					if nowTime <= time(13,00):
						with dbConnector(runMode=g_runMode, dbname='knightrun') as commander:
							query = 'select pushKey from pushkeys where pushagree = 1 and nightPushAgree = 1 and pushkey != "" and userId in (select userId from knightrun.square_object_schedule where isActivated  = 1 and nextInvasionTime <= now() and  nextInvasionTime >= date_sub(now(), Interval 5 Minute));'
							rows = commander.execute(query)
							sendPushMessage(rows)
				else:
					with dbConnector(runMode=g_runMode, dbname='knightrun') as commander:
						query = 'select pushKey from pushkeys where pushagree = 1 and pushkey != "" and userId in (select userId from knightrun.square_object_schedule where isActivated  = 1 and  nextInvasionTime <= now() and  nextInvasionTime >= date_sub(now(), Interval 5 Minute));'
						rows = commander.execute(query)
						sendPushMessage(rows)
		
	except Exception as e:
		print(tracebak.format_exc())
	finally:
		
		print('SQUARE PUSH SEND FIN	')		
	


if __name__ == "__main__":
	main()
