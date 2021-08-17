#매일 카드 사용 정보.
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
		pipeline.append( {'$match' : { 'message': 'CardUse', 'timestamp': re.compile(r"^"+str(date)) }} )
		pipeline.append( {'$group' : { '_id' : {'BattleType': '$fields.BattleType','UsedCardId': '$fields.CardStatId',	'ResultUse' : '$fields.CardUseResult' },'cnt' : { '$sum' : 1}	}} )
		
		result = db.Action.aggregate(pipeline)
			
		with dbConnector(auto_trans=True) as commander:
			for doc in result:
				battleType = doc['_id']['BattleType']
				usedCardId = doc['_id']['UsedCardId']
				resultUse = doc['_id']['ResultUse']
				count = doc['cnt']
				commander.execute('insert into usecardhistory(battleType, usedCardId, resultUse, count, date) values(%s, %s, %s, %s, %s)', battleType, usedCardId, resultUse, count, str(date))


	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()

