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
		with open('/KServer/LogMigration/config.json') as json_file:
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


		with dbConnector(dbname='account', auto_trans=True) as commander:
			ItemCostData = commander.execute('select ProductId, aosStoreProductId, iosStoreProductId, MaterialType, cost from _products_all_list; ')

		
		# 디지털 재화 구매 테이블
		pipeline = list()
		pipeline.append( {'$match' : {
			'message': 'ProductBuyDigital',
			'timestamp': re.compile(r"^"+str(date)) 
		   }} )

		pipeline.append( {'$group' : {
		   '_id' : {
			   'ProductId' : '$fields.ProductId'
		   },
		   'count' : { '$sum' : 1}
	   }} )
		
		results = db.Item.aggregate(pipeline)

					
		with dbConnector(dbname='KPI', auto_trans=True) as commander:
			for result in results:
				
				ProductId = int(result['_id']['ProductId'])
				Count = int(result['count'])
				
				for itemCost in ItemCostData:
					
					if ProductId == int(itemCost['ProductId']):
						totalcost = itemCost['cost'] * Count
						commander.execute('insert into daily_buy_digital(productId, materialType, totalcost, cost, count, date) values(%s, %s, %s, %s, %s, %s) on duplicate key update totalcost = totalcost + %s , count = count + %s',
							ProductId, itemCost['MaterialType'], totalcost, itemCost['cost'] , Count, str(date), totalcost, Count )

		#리얼 재화 구매 테이블
		pipeline = list()
		pipeline.append( {'$match' : {
			'message': 'ProductBuyReal',
			'timestamp': re.compile(r"^"+str(date)) 
		   }} )

		pipeline.append( {'$group' : {
		   '_id' : {
			   'ProductId' : '$fields.ProductId',
			   "PlatformType": "$fields.PlatformType",
		   },
		   'count' : { '$sum' : 1}
	   }} )
		
		results = db.Item.aggregate(pipeline)

					
		with dbConnector(dbname='KPI', auto_trans=True) as commander:
			for result in results:
				
				ProductId = int(result['_id']['ProductId'])
				PlatformType = int(result['_id']['PlatformType'])
				Count = int(result['count'])

				
				for itemCost in ItemCostData:
					
					if ProductId == int(itemCost['ProductId']):
						totalcost = itemCost['cost'] * Count
						commander.execute('insert into daily_buy_real(productId, platformType, totalcost, cost, count, date) values(%s, %s, %s, %s, %s, %s) on duplicate key update totalcost = totalcost + %s , count = count + %s',
							ProductId, PlatformType, totalcost, itemCost['cost'] , Count, str(date), totalcost, Count )


	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')

if __name__ == "__main__":
	main()


