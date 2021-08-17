#매일 카드 사용 정보.
#usecardhistory(battleType, usedCardId, resultUse, count, date)
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
		with open('/KServer/logMigretion/config.json') as json_file:
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
		pipeline.append( {'$match' : { 'message': 'DoSpell', 'timestamp': re.compile(r"^"+str(date)) }} )
		pipeline.append( {'$group' : { '_id' : {'UnitId' : '$fields.UnitId', 'TargetUnitId' : '$fields.TargetUnitId', 'CardId' : '$fields.CardId', 'SkillId' : '$fields.SkillId' }, 'TotalHP' : {'$sum': {'$toInt' : '$fields.HP' }}, 'TotalPrvHP' :{'$sum': {'$toInt' : "$fields.PrevHP"}}, 'avgDamage' : {'$avg' : {'$toInt' : "$fields.Damage"}}, 'MaxDamage' : {'$max' : {'$toInt' : "$fields.Damage"}}, 'MinDamage' : {'$min' : {'$toInt' : "$fields.Damage"}}, 'cnt' : { '$sum' : 1}	}} )
		
		result = db.SkillBehavior.aggregate(pipeline)
			
		with dbConnector(auto_trans=True) as commander:
			for doc in result:
				UnitId = doc['_id']['UnitId']
				TargetUnitId = doc['_id']['TargetUnitId']
				CardId = doc['_id']['CardId']
				SkillId = doc['_id']['SkillId']
				TotalHP = doc['TotalHP']
				TotalPrvHP = doc['TotalPrvHP']
				avgDamage = doc['avgDamage']
				MaxDamage = doc['MaxDamage']
				MinDamage = doc['MinDamage']
				count = doc['cnt']
				commander.execute('insert into dospell_log(unitId, targetUnitId, cardId, skillId, totalPrvHP, totalHP, avgDamage, MaxDamage, MinDamage, count, date) values(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)'
					  , UnitId, TargetUnitId, CardId, SkillId, int(TotalPrvHP), int(TotalHP),  int(avgDamage), int(MaxDamage), int(MinDamage), count, str(date))


	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()


