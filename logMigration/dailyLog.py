
# 부스트타임 연장 엔터룸 타입 재입장 후퇴 정보등을 마이그레이션
#usecardhistory(battleType, usedCardId, resultUse, count, date)
#crontab 에 스케쥴로 돌리기.

from pymongo import MongoClient
from bson.son import SON
import re
import mysql.connector, pytz
from  db import *
import datetime as datetime

def main():
	try: 
		with open('/KServer/logMigration/config.json') as json_file:
			jsondata = json.load(json_file)

		
		client = MongoClient(
			host =jsondata['host'],
			port = jsondata['port'],
			# replica=replica set
            # username=user
            # password=password
            # authSource=auth database
			)

		db = client[jsondata['database']]
				
		print('MongoDB connected')


		#print(datetime.date.today() - datetime.timedelta(days=1) )

		date = datetime.date.today() - datetime.timedelta(days=1) 

		#for date in dates:
		pipeline = list()
		pipeline.append( {'$match' : { 'message': 'EnterRoom', 'timestamp': re.compile(r"^"+str(date)) }} )
		pipeline.append( {'$group' : { '_id' : {'BattleType': '$fields.BattleType' },'cnt' : { '$sum' : 1}	}} )
		
		result = db.Action.aggregate(pipeline)
			
		with dbConnector(auto_trans=True) as commander:
			for doc in result:
				battleType = doc['_id']['BattleType']
				count = doc['cnt']
				
				commander.execute('insert into enter_room(BattleType, count, date) values( %s, %s, %s)', battleType, count, str(date))

		#쿼리 재입장
		pipeline = list()
		pipeline.append( {'$match' : { 'message': 'TryReEnterRoom', 'timestamp': re.compile(r"^"+str(date)) }} )
		pipeline.append( {'$group' : { '_id' : {'TryReEnterRoom': '$message' },'cnt' : { '$sum' : 1}	}} )

		result = db.Action.aggregate(pipeline)
		count = doc['cnt']
		with dbConnector(auto_trans=True) as commander:				
			commander.execute('insert into reenter_room(count, date) values( %s, %s)', count, str(date))

		# 퇴각 
		pipeline = list()
		pipeline.append( {'$match' : { 'message': 'Retreat', 'timestamp': re.compile(r"^"+str(date)) }} )
		pipeline.append( {'$group' : { '_id' : {'BattleType': '$fields.BattleType' },'cnt' : { '$sum' : 1}	}} )

		result = db.Action.aggregate(pipeline)
		battleType = doc['_id']['BattleType']
		count = doc['cnt']
		with dbConnector(auto_trans=True) as commander:				
			commander.execute('insert into reenter_room(count, date) values( %s, %s)', count, str(date))

	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()

