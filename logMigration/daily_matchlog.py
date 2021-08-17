#matching log 
#
#crontab 에 스케쥴로 돌리기.  (이것들을 같이 굴릴 스크립트 작업?)

from pymongo import MongoClient
from bson.son import SON
import re
import mysql.connector, pytz
from  db import *
import datetime as datetime
import json
from grade import *

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
		pipeline.append( {'$match' : { 'message': 'MatchingSuccess', 'timestamp': re.compile(r"^"+str(date)) }} )
		#pipeline.append( {'$match' : { 'message': 'MatchingSuccess' }} )
		pipeline.append( {'$group': {'_id': {'BattleType': '$fields.BattleType', 'EnemyType' : '$fields.EnemyType', 'RankPoint' : '$fields.EnemyTeamRankPoint'}, 'count' : {'$sum': 1}}} )
		
		result = db.Matching.aggregate(pipeline)
			
		with dbConnector(auto_trans=True) as commander:
			for doc in result:
				BattleType = doc['_id']['BattleType']
				EnemyType = doc['_id']['EnemyType']
				RankPoint = doc['_id']['RankPoint']
				RankTire = matchGrade(int(RankPoint))
				Count = int(doc['count'])


				commander.execute('insert into daily_matchlog(battleType, EnemyType, RankTire, Count, date) values(%s, %s, %s, %s, %s) On duplicate Key update Count = Count + %s', BattleType, EnemyType, RankTire, Count, str(date), Count)


	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()



