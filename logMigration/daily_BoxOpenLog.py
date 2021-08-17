#Infusion Box Open Log
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


		

		date = datetime.date.today() - datetime.timedelta(days=1) 

		
		pipeline = list()

		pipeline.append({ '$match': { "message": "InfusionBoxOpen" ,'timestamp': re.compile(r"^"+str(date)) }})
		#pipeline.append({ '$match': { "message": "InfusionBoxOpen" }})
		pipeline.append({'$group' : { '_id' : { 'BoxId' : '$fields.BoxId' },'count' : {'$sum' : 1 } }})
		
		results = db.InfusionBox.aggregate(pipeline)
		
		for result in results:
			print(result)
			with dbConnector(auto_trans=True) as commander:
				commander.execute('insert into daily_open_infusionbox_count(boxID, count, date) values(%s, %s, %s) On duplicate Key update Count = %s', result['_id']['BoxId'],result['count'], str(date), result['count'])


		mongoQuery = { "message": "InfusionBoxOpen",'timestamp': re.compile(r"^"+str(date)) }
		#mongoQuery = { "message": "InfusionBoxOpen" }

		results = db.InfusionBox.find(mongoQuery)


		tempResult = list()

		for doc in results:

			BoxId = doc['fields']['BoxId']
			InfusionBoxOpenType = doc['fields']['InfusionBoxOpenType']
			ItemResult = doc['fields']['ItemResultList']
			tempList = ItemResult.split(',')

			for temp in tempList:
				tt = temp.split(':')
				tempResult.append( {'BoxId' : BoxId , 'InfusionBoxOpenType' : InfusionBoxOpenType , 'ItemCode' :tt[0], 'Amount':tt[1] }  )

		
		
		
		with dbConnector(auto_trans=True) as commander:
			for line in tempResult:
				boxId = line['BoxId']
				boxOpenType = line['InfusionBoxOpenType']
				ItemCode = line['ItemCode']
				Amount = int(line['Amount'])

				if Amount == 0:
					commander.execute('insert into daily_itemEmbargo(boxId, boxOpenType, ItemCode, removeEmbargoCount, date) values(%s, %s, %s, 1, %s) On duplicate Key update removeEmbargoCount = removeEmbargoCount + 1'
					   , boxId, boxOpenType, ItemCode, str(date))
				elif Amount > 0:	
					commander.execute('insert into daily_infusionbox_item(boxId, boxOpenType, ItemCode, amount , count, date) values(%s, %s, %s, %s, 1, %s) On duplicate Key update amount = amount + %s, count= count +1'
					   , boxId, boxOpenType, ItemCode, Amount, str(date), Amount )

		
		
	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()


