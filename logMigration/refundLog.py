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


		with dbConnector(dbname='kpi', auto_trans=True) as commander:
			# userIds = commander.execute('select distinct userId from account.pay_pending limit 10')

		# print(userIds)
		
			# for userid in userIds:

			pipeline = {"fields.MaterialType" : {'$in' : ["2",'3']  }}

			results = db.Item.find(pipeline)

			for result in results:
				userid = result['fields']['UserId']
				count = result['fields']['Count']
				message = result['message']
				logCategory = result['fields']['LogCategory']
				materialType = result['fields']['MaterialType']
				total = result['fields']['TotalCount']
				logtype = 'Get'
				if message == 'MaterialUse':
					logtype = 'Use'

				datetimee = result['fields']['create']

				commander.execute('insert into kpi.refund_log2(userId, materialType, LogType, LogCategory, Count, total, datetime) values(%s,%s,%s,%s,%s,%s,%s)', userid, materialType, logtype, logCategory, count, total, datetimee )


	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')





if __name__ == "__main__":
	main()


