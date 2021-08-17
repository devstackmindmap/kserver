#평타 테미지 유닛 크리티컬시 정보.
#
#crontab 에 스케쥴로 돌리기.  (이것들을 같이 굴릴 스크립트 작업?)

from pymongo import MongoClient
from bson.son import SON
import re
import mysql.connector, pytz
from  db import *
import datetime as datetime

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
		pipeline.append( {'$match' : { 'message': 'AttackResult', 'timestamp': re.compile(r"^"+str(date)) }} )
		pipeline.append( {'$group': {'_id': {'UnitId': '$fields.UnitId','IsCritical' : '$fields.IsCritical'},'TotalDamage': {'$sum': {'$toInt' : '$fields.Damage'} },'attackCount' : {'$sum': 1},'avgDamage' : {'$avg' : {'$toInt' : '$fields.Damage'}}}} )
		
		result = db.NormalAttack.aggregate(pipeline)
			
		with dbConnector(auto_trans=True) as commander:
			for doc in result:
				unitId = doc['_id']['UnitId']
				isCritical = doc['_id']['IsCritical']
				if isCritical == 'False':
					isCritical = False
				else:
					isCritical = True
				totalDamage = int(doc['TotalDamage'])
				attackCount = int(doc['attackCount'])
				avgDamage = int(doc['avgDamage'])

				commander.execute('insert into normalattack(unitId, isCritical, totalDamage, attackCount, avgDamage, date) values(%s, %s, %s, %s, %s, %s)', unitId, isCritical, totalDamage, attackCount, avgDamage, str(date))


	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()


