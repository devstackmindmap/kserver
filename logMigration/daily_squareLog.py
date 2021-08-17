#당일 스퀘어 오브젝트
#
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
		#with open('./config_local.json') as json_file:
		with open('/KServer/logMigration/config.json') as json_file:
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
			'message': 'Start',
			'timestamp': re.compile(r"^"+str(date)) 
		   }} )

		pipeline.append( {'$group' : {
		   '_id' : {
		    "MonsterLevel": "$fields.MonsterLevel",
            "MonsterId": "$fields.MonsterId",
            "ObjectLevel": "$fields.ObjectLevel",
            "ActiveLevel": "$fields.ActivatedLevel",
            "CoreLevel": "$fields.CoreLevel",
            "AgencyLevel": "$fields.AgencyLevel",
		   },
		   "Count" : {"$sum":1}     
	   }} )
		
		results = db.Square.aggregate(pipeline)

					
		with dbConnector(dbname='KPI', auto_trans=True) as commander:
			for result in results:
				
				
				MonsterLevel = result['_id']['MonsterLevel']
				MonsterId = result['_id']['MonsterId']
				ObjectLevel = result['_id']['ObjectLevel']
				ActiveLevel = result['_id']['ActiveLevel']
				CoreLevel = result['_id']['CoreLevel']
				AgencyLevel = result['_id']['AgencyLevel']
				Count = result['Count']

				commander.execute('insert into daily_square_start(MonsterLevel, MonsterId, ObjectLevel, ActiveLevel, CoreLevel, AgencyLevel, Count,  Date) values(%s, %s, %s, %s, %s, %s, %s, %s)' ,
					MonsterLevel, MonsterId, ObjectLevel, ActiveLevel, CoreLevel, AgencyLevel, Count, str(date))


# 이곳부터 에너쥐
		pipeline = list()
		pipeline.append( {'$match' : {
			'message': 'Energy',
			'timestamp': re.compile(r"^"+str(date)) 
		   }} )

		pipeline.append( {'$group' : {
		   '_id' : {
		    "CoreLevel" : "$fields.CoreLevel",
			"AgencyLevel" : "$fields.AgencyLevel"
		   },
		    "totalBoxEnergy" : {"$sum" : {"$toInt" : "$fields.BoxEnergy"}},
			"totalPowerEnergy" : {"$sum" : {"$toInt" : "$fields.PowerEnergy"}},
			"totalPower" : {"$sum" : {"$toInt" : "$fields.Power"}},
			"totalBoxLevel" : {"$sum" : {"$toInt" : "$fields.BoxLevel"}},     
			"AvgPower" : {"$avg" : {"$toInt" : "$fields.Power"}},
			"AvgBoxLevel" : {"$avg" : {"$toInt" : "$fields.BoxLevel"}},     
			"AvgBoxEnergy" : {"$avg" : {"$toInt" : "$fields.BoxEnergy"}},
			"MaxPower" : {"$max" : {"$toInt" : "$fields.Power"}},
			"MaxBoxLevel" : {"$max" : {"$toInt" : "$fields.BoxLevel"}},     
			"Count" : {"$sum" :1},   
	   }} )
		
		results = db.Square.aggregate(pipeline)
					
		with dbConnector(dbname='KPI', auto_trans=True) as commander:
			for result in results:
				
				CoreLevel = result['_id']['CoreLevel']
				AgencyLevel = result['_id']['AgencyLevel']

				TotalBoxEnergy = result['totalBoxEnergy']
				TotalPowerEnergy = result['totalPowerEnergy']
				TotalPower = result['totalPower']
				TotalBoxLevel = result['totalBoxLevel']
				AvgPower = result['AvgPower']
				AvgBoxEnergy = result['AvgBoxEnergy']
				AvgBoxLevel = result['AvgBoxLevel']
				MaxPower = result['MaxPower']
				MaxBoxLevel = result['MaxBoxLevel']
				Count = result['Count']

				commander.execute('insert into daily_square_energy(CoreLevel, AgencyLevel, TotalBoxEnergy, TotalBoxLevel, TotalPowerEnergy, TotalPower, AvgPower, AvgBoxEnergy, AvgBoxLevel, MaxPower, MaxBoxLevel, Count,  Date) values(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)' ,
					CoreLevel, AgencyLevel, TotalBoxEnergy, TotalBoxLevel, TotalPowerEnergy, TotalPower, AvgPower, AvgBoxEnergy, AvgBoxLevel, MaxPower, MaxBoxLevel, Count, str(date))


#스퀘어 오브젝트 박스 오픈 정보
		pipeline = list()
		pipeline.append( {'$match' : {
			"message" : "Stop",
			'timestamp': re.compile(r"^"+str(date)) 
		   }} )

		pipeline.append( {'$group' : {
		   '_id' : {
		   	    "ActivatedLevel" : "$fields.ActivatedLevel",
				"BoxLevel" : "$fields.BoxLevel"
		   },
			"Count" : {"$sum" :1},   
	   }} )
		
		results = db.Square.aggregate(pipeline)
					
		with dbConnector(dbname='KPI', auto_trans=True) as commander:
			for result in results:
				
				ActivatedLevel = result['_id']['ActivatedLevel']
				BoxLevel = result['_id']['BoxLevel']
	
				Count = result['Count']

				commander.execute('insert into daily_square_boxOpen(ActivatedLevel, BoxLevel, LogType, Count, Date) values(%s, %s, %s, %s, %s)' ,
					ActivatedLevel, BoxLevel, 1, Count, str(date))

#스퀘어 오브젝트 박스 파괴 정보
		pipeline = list()
		pipeline.append( {'$match' : {
			"message" : "Invaded",
	        "fields.Destoryed" : "True",
			#'timestamp': re.compile(r"^"+str(date)) 
		   }} )

		pipeline.append( {'$group' : {
		   '_id' : {
		   	    "ActivatedLevel" : "$fields.ActivatedLevel",
				"BoxLevel" : "$fields.BoxLevel"
		   },
			"Count" : {"$sum" :1},   
	   }} )
		
		results = db.Square.aggregate(pipeline)
					
		with dbConnector(dbname='KPI', auto_trans=True) as commander:
			for result in results:
				
				ActivatedLevel = result['_id']['ActivatedLevel']
				BoxLevel = result['_id']['BoxLevel']
	
				Count = result['Count']

				commander.execute('insert into daily_square_boxopen(ActivatedLevel, BoxLevel, LogType, Count, Date) values(%s, %s, %s, %s, %s)' ,
					ActivatedLevel, BoxLevel, 2, Count, str(date))



	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()


