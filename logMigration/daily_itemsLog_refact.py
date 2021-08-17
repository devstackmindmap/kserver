# 로그 이름 Item 에서 남겨진 로그 처리
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
		## 조각 처리 시작
		print(str(date)+' : Piece Get 처리')
		#mongoQuery = { "message": "PieceUse",'timestamp': re.compile(r"^"+str(date)) }
		#mongoQuery = { "message": "PieceUse" }
		pipeline.append({'$match' : { 'message': 'PieceGet', 'timestamp': re.compile(r"^"+str(date)) }})
		#pipeline.append({'$match' : { 'message': 'PieceGet' }})
		pipeline.append({'$group': {  '_id' : {  'TableName': '$fields.TableName',  'ClassId' : '$fields.ClassId',  'Count' : '$fields.Count',  'LogCategory' : '$fields.LogCategory'  },  'callcount' : {'$sum' : 1},  'count' : {'$sum' : {'$toInt' : '$fields.Count'} }    }})


		#results = db.Item.find(mongoQuery)
		results = db.Item.aggregate(pipeline)


		with dbConnector(auto_trans=True) as commander:
			for doc in results:
				
				tableName = doc['_id']['TableName']
				classId = doc['_id']['ClassId']
				embago =  doc['_id']['Count']
				category = doc['_id']['LogCategory']
				callcount = doc['callcount']
				count = doc['count']

				if embago == "0":
					commander.execute('insert into daily_item_piece_log(logType, tableName, classId, count, callcount, category, date) values(1, %s, %s, %s, %s, %s, %s) On duplicate Key update count = count + %s , callcount = callcount + %s;'
						   , tableName, classId, count,callcount, category, str(date), count ,callcount)
				else:
					commander.execute('insert into daily_item_piece_log(logType, tableName, classId, count, callcount, category, date) values(1, %s, %s, %s, %s, %s, %s) On duplicate Key update count = count + %s , callcount = callcount + %s;'
						   , tableName, classId, count, callcount, category, str(date), count,callcount )
		
		print(str(date)+' : Piece Use 처리')
		pipeline = []
		pipeline.append({'$match' : { 'message': 'PieceUse', 'timestamp': re.compile(r"^"+str(date)) }})
		#pipeline.append({'$match' : { 'message': 'PieceUse' }})
		pipeline.append({'$group': {  '_id' : {  'TableName': '$fields.PieceType',  'ClassId' : '$fields.ClassId',  'Count' : '$fields.Count',  'LogCategory' : '$fields.LogCategory'  },  'callcount' : {'$sum' : 1},  'count' : {'$sum' : {'$toInt' : '$fields.Count'} }    }})


		#results = db.Item.find(mongoQuery)
		results = db.Item.aggregate(pipeline)


		with dbConnector(auto_trans=True) as commander:
			for doc in results:
				
				tableName = doc['_id']['TableName']
				classId = doc['_id']['ClassId']
				embago =  doc['_id']['Count']
				category = doc['_id']['LogCategory']
				callcount = doc['callcount']
				count = doc['count']

				commander.execute('insert into daily_item_piece_log(logType, tableName, classId, count, callcount, category, date) values(1, %s, %s, %s, %s, %s, %s) On duplicate Key update count = count + %s , callcount = callcount + %s;'
				   , tableName, classId, count, callcount, category, str(date), count, callcount )



		## material 처리 시작
		pipeline = list()
		print(str(date)+' : Material Get 처리')
		pipeline.append({'$match' : { 'message': 'MaterialGet', 'timestamp': re.compile(r"^"+str(date)) }})
		#pipeline.append({'$match' : { 'message': 'MaterialGet' }})
		pipeline.append({'$group': {  '_id' : {  "MaterialType": "$fields.MaterialType",  'LogCategory' : '$fields.LogCategory'  },  'callcount' : {'$sum' : 1},  'count' : {'$sum' : {'$toInt' : '$fields.Count'} }    }})


		#results = db.Item.find(mongoQuery)
		results = db.Item.aggregate(pipeline)


		with dbConnector(auto_trans=True) as commander:
			for doc in results:
				
				MaterialType = doc['_id']['MaterialType']
				category = doc['_id']['LogCategory']
				callcount = doc['callcount']
				count = doc['count']

				commander.execute('insert into daily_item_material_log(logType, materialType, count, callcount, category, date) values(1, %s, %s, %s, %s, %s) On duplicate Key update count = count + %s , callcount = callcount + %s;'
						   , MaterialType,  count, callcount, category, str(date), count, callcount )
		
		print(str(date)+' : Material Use 처리')

		
		pipeline = []
		pipeline.append({'$match' : { 'message': 'MaterialUse', 'timestamp': re.compile(r"^"+str(date)) }})
		#pipeline.append({'$match' : { 'message': 'MaterialUse' }})
		pipeline.append({'$group': {  '_id' : {  "MaterialType": "$fields.MaterialType",  'LogCategory' : '$fields.LogCategory'  },  'callcount' : {'$sum' : 1},  'count' : {'$sum' : {'$toInt' : '$fields.Count'} }    }})
		
		results = db.Item.aggregate(pipeline)


		with dbConnector(auto_trans=True) as commander:
			for doc in results:
				
				MaterialType = doc['_id']['MaterialType']
				category = doc['_id']['LogCategory']
				callcount = doc['callcount']
				count = doc['count']

				commander.execute('insert into daily_item_material_log(logType, materialType, count, callcount, category, date) values(2, %s, %s, %s, %s, %s) On duplicate Key update count = count + %s , callcount = callcount + %s;'
						   , MaterialType,  count, callcount, category, str(date), count, callcount )


		## ETC item 처리 시작
		pipeline = list()
		print(str(date)+' : ETC Item Get 처리')
		#pipeline.append({'$match' : { 'message': 'PieceGet', 'timestamp': re.compile(r"^"+str(date)) }})
		pipeline.append({'$match': { "$or": [
			{ "message": "SkinGet" },
            {"message": "EmoticonGet"},
            {"message": "ContentGet"},
            {"message": "UserProfile"}
			],'timestamp': re.compile(r"^"+str(date))}})
		pipeline.append({
		   '$group': {
			'_id': {
            "LogType": "$message",
            "ClassId": "$fields.ClassId",
            "LogCategory": "$fields.LogCategory"
			},
			"Count": {
				'$sum': 1
			},
		    }})


		#results = db.Item.find(mongoQuery)
		results = db.Item.aggregate(pipeline)


		with dbConnector(auto_trans=True) as commander:
			for doc in results:
				
				ItemLogType = doc['_id']['LogType']
				ClassId = doc['_id']['ClassId']
				Category = doc['_id']['LogCategory']
				Count = doc['Count']

				commander.execute('insert into daily_item_etc_log(LogType, ClassId, Category, count, date) values(%s, %s, %s, %s, %s);'
						   , ItemLogType,  ClassId, Category, Count, str(date) )
		

		
	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()



