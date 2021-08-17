from pymongo import MongoClient
from bson.son import SON
import re
import mysql.connector, pytz
from  db import *
import datetime as datetime

def main():
	try: 
		client = MongoClient(
			host ='52.79.34.162',
			port = 27017,
			# replica=replica set
            # username=user
            # password=password
            # authSource=auth database
			)

		db = client['KnightRun']
				
		print('MongoDB connected')


		dates = ['2020-02-13','2020-02-17']

		#print(datetime.date.today() - datetime.timedelta(days=1) )

		date = datetime.date.today() - datetime.timedelta(days=1) 

		#for date in dates:
		pipeline = list()
		pipeline.append( {'$match' : { 'timestamp': re.compile(r"^"+date) }} )
		pipeline.append( {'$group' : { '_id' : {'userId' : '$fields.UserId'	},'cnt' : { '$sum' : 1}	}} )
			
		
		result = db.Login.aggregate(pipeline)
			
		with dbConnector(auto_trans=True) as commander:
			for doc in result:
				#print(doc['_id']['userId'])
				commander.execute('insert into login_log(UserId, count, date) values(%s,%s,%s)',str(doc['_id']['userId']), doc['cnt'], date)


	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()


