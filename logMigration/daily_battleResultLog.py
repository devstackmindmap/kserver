#당일 상점 구매 정보 로그
#login_log (userId, cnt, date)
#crontab 에 스케쥴로 돌리기.

from pymongo import MongoClient
from bson.son import SON
import re
import mysql.connector, pytz
from  db import *
import datetime as datetime
import json



def main():
	try: 
		with open('/KServer/logMigration/config.json') as json_file:
		#with open('./config.json') as json_file:
			jsondata = json.load(json_file)

		
		client = MongoClient(
			host =jsondata['host'],
			port = jsondata['port'],
			#host = '47.75.102.249',
			#port = 27017,
			# replica=replica set
            # username=user
            # password=password
            # authSource=auth database
			)

		db = client[jsondata['database']]
				
		print('MongoDB connected')


		date = datetime.date.today() - datetime.timedelta(days=1) 

				
		pipeline = list()
		pipeline.append( {'$match' : {
			'message': 'BattleEndResult',
			'timestamp': re.compile(r"^"+str(date)) 
		   }} )

		pipeline.append( {'$group' : {
		   '_id' : {
			'Winner' : '$fields.Winner',
			'BattleType' : '$fields.BattleType',
			'Player1Units' : '$fields.Player1Units',
			'Player2Units' : '$fields.Player2Units',
			'Player1Cards' : '$fields.Player1Cards',
			'Player2Cards' : '$fields.Player2Cards',
			'Player1UnitLevels' : '$fields.Player1UnitLevels',
			'Player2UnitLevels' : '$fields.Player2UnitLevels',
		   },
		   'count' : { '$sum' : 1},
			'avgPlayTime' : {'$avg' : { '$subtract' : [ {'$toLong' : {'$toDate' : '$fields.EndTime'}} , {'$toLong' : {'$toDate' : '$fields.StartTime'}} ] }},
			'maxPlayTime' : {'$max' : { '$subtract' : [ {'$toLong' : {'$toDate' : '$fields.EndTime'}} , {'$toLong' : {'$toDate' : '$fields.StartTime'}} ] }},
			'minPlayTime' : {'$min' : { '$subtract' : [ {'$toLong' : {'$toDate' : '$fields.EndTime'}} , {'$toLong' : {'$toDate' : '$fields.StartTime'}} ] }},       
	   }} )
		
		results = db.BattleStatus.aggregate(pipeline)

					
		with dbConnector(dbname='KPI', auto_trans=True) as commander:
			for result in results:
				
				
				winner = result['_id']['Winner']
				BattleType = result['_id']['BattleType']
				Player1Units = result['_id']['Player1Units']
				Player2Units = result['_id']['Player2Units']
				Player1Cards = result['_id']['Player1Cards']

				if 'Player2Cards' in result['_id']:
					Player2Cards = result['_id']['Player2Cards']
				else:
					Player2Cards = 'none'

				Player1UnitLevels = result['_id']['Player1UnitLevels']
				
				if 'Player2UnitLevels' in result['_id']:
					Player2UnitLevels = result['_id']['Player2UnitLevels']
				else:
					Player2UnitLevels = 'none'

				count = result['count']
				avgTime = result['avgPlayTime']
				maxTime = result['maxPlayTime']
				minTime = result['minPlayTime']

				if winner == 'Player1':
					commander.execute('insert into daily_winners_deck(battleType, resultType, winnerUnits, winnerUnitLevels, winnercards, loseUnits, loseUnitLevels, losecards, count, avgPlayTime, maxPlayTime, minPlayTime, date) values(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)' ,
					int(BattleType), 1, Player1Units, Player1UnitLevels, Player1Cards, Player2Units, Player2UnitLevels, Player2Cards, count, int(avgTime), int(maxTime), int(minTime), str(date))
				elif  winner == 'Player2':
					commander.execute('insert into daily_winners_deck(battleType, resultType, winnerUnits, winnerUnitLevels, winnercards, loseUnits, loseUnitLevels, losecards, count, avgPlayTime, maxPlayTime, minPlayTime, date) values(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)' ,
					int(BattleType), 1, Player2Units, Player2UnitLevels, Player2Cards, Player1Units, Player1UnitLevels, Player1Cards, count, int(avgTime), int(maxTime), int(minTime), str(date))
				else:
					commander.execute('insert into daily_winners_deck(battleType, resultType, winnerUnits, winnerUnitLevels, winnercards, loseUnits, loseUnitLevels, losecards, count, avgPlayTime, maxPlayTime, minPlayTime, date) values(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)' ,
					int(BattleType), 2, Player1Units, Player1UnitLevels, Player1Cards, Player2Units, Player2UnitLevels, Player2Cards, count, int(avgTime), int(maxTime), int(minTime), str(date))

	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()


