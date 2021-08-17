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
		# with open('/KServer/logMigration/config.json') as json_file:
		with open('./config_local.json') as json_file:
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


		with dbConnector(dbname='account', auto_trans=True) as commander:
			# userIds = commander.execute('select distinct userId from account.pay_pending limit 10')

		# print(userIds)
		
			# for userid in userIds:

			pipeline = {'fields.LogCategory' : 'ProductBuyReal', "fields.MaterialType" : "2"   }

			results1 = db.Item.find(pipeline)

			for result in results1:
				userid = result['fields']['UserId']
				count = result['fields']['Count']
				logtype = 'Buy'
				datetimee = result['fields']['create']

				commander.execute('insert into kpi.refund_log(userId, LogType, Count, datetime) values(%s,%s,%s,%s)', userid, logtype, count, datetimee )
				# print(result['fields']['Count'])
				# print(result['fields']['TotalCount'])


			pipeline = {'message' : 'MaterialUse', "fields.MaterialType" : "2" }

			results2 = db.Item.find(pipeline)

			for result in results2:
				userid = result['fields']['UserId']
				count = result['fields']['Count']
				MaterType = result['fields']['MaterialType']
				logtype = 'FreeUse'
				datetimee = result['fields']['create']

				commander.execute('insert into kpi.refund_log(userId, LogType, Count, datetime) values(%s,%s,%s,%s)', userid, logtype, count, datetimee )

			pipeline = {'message' : 'MaterialUse', "fields.MaterialType" : "3" }

			results3 = db.Item.find(pipeline)

			for result in results3:
				userid = result['fields']['UserId']
				count = result['fields']['Count']
				MaterType = result['fields']['MaterialType']
				# if MaterType == 2:
				# 	logtype = 'FreeUse'
				# elif MaterType ==3:
				logtype = 'PaidUse'
				datetimee = result['fields']['create']

				commander.execute('insert into kpi.refund_log(userId, LogType, Count, datetime) values(%s,%s,%s,%s)', userid, logtype, count, datetimee )		

			
			pipeline = {'message' : 'MaterialGet', "fields.MaterialType" : "2", 'fields.LogCategory' : {'$not' :  re.compile(r'^'+'ProductBuyReal' ) }}

			results4 = db.Item.find(pipeline)

			for result in results4:
				userid = result['fields']['UserId']
				count = result['fields']['Count']
				MaterType = result['fields']['MaterialType']
				# if MaterType == 2:
				# 	logtype = 'FreeUse'
				# elif MaterType ==3:
				logtype = 'FreeGet'
				datetimee = result['fields']['create']

				commander.execute('insert into kpi.refund_log(userId, LogType, Count, datetime) values(%s,%s,%s,%s)', userid, logtype, count, datetimee )		

		

	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')





if __name__ == "__main__":
	main()


