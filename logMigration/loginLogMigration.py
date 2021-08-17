#당일 당일 로그인 한 유저와 로그인 카운트를 저장.
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

		date = datetime.date.today() - datetime.timedelta(days=1) 
		
		#for date in dates:
		pipeline = list()
		pipeline.append( {'$match' : { 'timestamp': re.compile(r"^"+str(date)) }} )
		pipeline.append( {'$group' : { '_id' : {'userId' : '$fields.UserId'	},'cnt' : { '$sum' : 1}	}} )
		
		result = db.Login.aggregate(pipeline)
			
		with dbConnector(auto_trans=True) as commander:
			for doc in result:
				
				commander.execute('insert into login_log(UserId, count, date) values(%s,%s,%s)',str(doc['_id']['userId']), doc['cnt'], date)


	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()

