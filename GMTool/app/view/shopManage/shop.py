from django.shortcuts import render
from django.http import HttpResponse
from app.infra.db import *
from datetime import datetime, timedelta
import json
import redis
from GMTool.settings import g_commonconfig, g_runMode


def myconverter(o):
    if isinstance(o, datetime):
        return o.__str__()


def getShopInfo(request):
	if request.method == 'GET':
		return render(
			request,
			'app/shop/getShopInfo.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		settings = response_data['settings']
		datas = {}
		if msg == 'getShopInfo':
			datas = getShopData(settings)
			datas = json.dumps(datas, ensure_ascii=False,default=myconverter).encode('utf-8')

		elif msg == 'updateShop':
			updateList = response_data['updateList']
			updateShopInfo(settings, updateList)

		elif msg == 'deleteShopInfo':
			deleteList = response_data['deleteList']
			deleteShopInfo(settings, deleteList)

		return HttpResponse(datas, content_type='application/json; charset=utf-8')

def addShopInfo(request):
	if request.method == 'GET':
		return render(
			request,
			'app/shop/addShopInfo.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		data = response_data['updateMap']
		settings = response_data['settings']
		userInfoType = settings['shopInfoType']

		with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True) as commander:
			
			if userInfoType == 'allList':
				commander.execute("INSERT INTO _products_all_list(productId, aosStoreProductId, iosStoreProductId, startDateTime, endDateTime, productTableType, storeType,  productType, countOfPurchases, materialType, cost, saleCost ) VALUES(%s, %s, %s, %s, %s, %s, %s ,%s, %s ,%s, %s ,%s)", 
					  str(data['productId']), str(data['aosStoreProductId']), str(data['iosStoreProductId']), str(data['startDateTime']), str(data['endDateTime']),  str(data['productTableType']), str(data['storeType']),  str(data['productType'])
					  , str(data['countOfPurchases']), str(data['materialType']), str(data['cost']), str(data['saleCost']))

			elif userInfoType == 'productsText':
				commander.execute("INSERT INTO _products_text(productId, languageType, productText) VALUES(%s, %s, %s)", str(data['productId']), str(data['languageType']), str(data['productText']))

			elif userInfoType == 'products':
				commander.execute("INSERT INTO _products(productId, rewardType, rewardId ) VALUES(%s, %s, %s)", str(data['productId']), str(data['rewardType']), str(data['rewardId']))

			elif userInfoType == 'rewards':
				commander.execute("INSERT INTO _rewards(rewardId, itemId) VALUES(%s, %s)",  str(data['rewardId']), str(data['itemId']))

			elif userInfoType == 'items':
				commander.execute("INSERT INTO _items(itemId, itemType, id, minCount, MaxCount, probability) VALUES(%s, %s, %s, %s, %s, %s)", 
					str(data['itemId']), str(data['itemType']), str(data['id']), str(data['minCount']), str(data['MaxCount']), str(data['probability']))

			elif userInfoType == 'eventDigital':
				#commander.execute("INSERT INTO _products_event_digital(productId, startDateTime, endDateTime, storeType, productBannerType
				#, productType, materialType, saleCost, cost, countOfPurchases) VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s)",
				#	str(data['productId']), str(data['startDateTime']), str(data['endDateTime']), str(data['storeType']), str(data['productBannerType']), str(data['productType']), str(data['materialType']), str(data['saleCost']), str(data['cost']), str(data['countOfPurchases']) )
				commander.execute("INSERT INTO _products_event_digital(productId, startDateTime, endDateTime, storeType,  productType, materialType, saleCost, cost, countOfPurchases) VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s)",
						str(data['productId']), str(data['startDateTime']), str(data['endDateTime']), str(data['storeType']),  str(data['productType']), str(data['materialType']), str(data['saleCost']), str(data['cost']), str(data['countOfPurchases']) )
			elif userInfoType == 'eventReal':
				commander.execute("INSERT INTO _products_event_real(productId, startDateTime, endDateTime, platform, storeProductId, storeType, productType, saleCost, cost, countOfPurchases) VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)",
					str(data['productId']), str(data['startDateTime']), str(data['endDateTime']), str(data['platform']), str(data['storeProductId']), str(data['storeType']), str(data['productType']), str(data['saleCost']), str(data['cost']), str(data['countOfPurchases']) )

			elif userInfoType == 'fixDigital':
				commander.execute("INSERT INTO _products_fix_digital(productId, storeType,  productType, materialType, cost) VALUES(%s, %s, %s, %s, %s)",
					str(data['productId']), str(data['storeType']),  str(data['productType']), str(data['materialType']),  str(data['cost']) )

			elif userInfoType == 'fixReal':
				commander.execute("INSERT INTO _products_fix_real(productId, platform, storeProductId, storeType, productType,  cost) VALUES(%s, %s, %s, %s, %s, %s)",
					str(data['productId']), str(data['platform']), str(data['storeProductId']), str(data['storeType']),  str(data['productType']), str(data['cost']) )

			elif userInfoType == 'userDigital':
				commander.execute("INSERT INTO _products_user_digital(productId, saleDurationHour, storeType, productType, materialType, saleCost, cost, countOfPurchases) VALUES(%s, %s, %s, %s, %s, %s, %s, %s)",
					str(data['productId']), str(data['saleDurationHour']), str(data['storeType']), str(data['productType']), str(data['materialType']), str(data['saleCost']),  str(data['cost']), str(data['countOfPurchases']) )

			elif userInfoType == 'userReal':
				commander.execute("INSERT INTO _products_user_real(productId, platform, storeProductId, saleDurationHour, storeType, productType,  saleCost, cost, countOfPurchases) VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s)",
					str(data['productId']), str(data['platform']), str(data['storeProductId']), str(data['saleDurationHour']), str(data['storeType']), str(data['productType']), str(data['saleCost']),  str(data['cost']), str(data['countOfPurchases']) )


		return HttpResponse({}, content_type='application/json; charset=utf-8')

def getShopData(settings):
	userInfoType = settings['shopInfoType']
	rows = []

	with dbConnector(runMode=settings['runMode'], dbname='account') as commander:

		if userInfoType == 'allList':
			rows = commander.execute("SELECT seq, productId, aosStoreProductId, iosStoreProductId, startDateTime, endDateTime, productTableType, storeType,  productType, countOfPurchases, materialType, cost, saleCost FROM _products_all_list" )

		elif userInfoType == 'productsText':
			rows = commander.execute("SELECT productId, languageType, productText FROM _products_text ")

		elif userInfoType == 'products':
			rows = commander.execute("SELECT seq, productId, rewardType, rewardId FROM _products")

		elif userInfoType == 'rewards':
			rows = commander.execute("SELECT seq, rewardId, itemId FROM _rewards ")

		elif userInfoType == 'items':
			rows = commander.execute("SELECT seq, itemId, itemType, id, minCount, MaxCount, probability  FROM _items")

		elif userInfoType == 'eventDigital':
			rows = commander.execute("SELECT productId, startDateTime, endDateTime, storeType,  productType, materialType, saleCost, cost, countOfPurchases FROM _products_event_digital")

		elif userInfoType == 'eventReal':
			rows = commander.execute("SELECT productId, startDateTime, endDateTime, platform, storeProductId, storeType,  productType, saleCost, cost, countOfPurchases FROM _products_event_real")

		elif userInfoType == 'fixDigital':
			rows = commander.execute("SELECT productId,  storeType,  productType, materialType, cost FROM _products_fix_digital")

		elif userInfoType == 'fixReal':
			rows = commander.execute("SELECT productId,  platform, storeProductId, storeType,  productType, cost FROM _products_fix_real")

		elif userInfoType == 'userDigital':
			rows = commander.execute("SELECT productId, saleDurationHour, storeType, productType, countOfPurchases, materialType, cost, saleCost FROM _products_user_digital")
		
		elif userInfoType == 'userReal':
			rows = commander.execute("SELECT productId, platform, storeProductId, saleDurationHour, storeType, productType, countOfPurchases, cost, saleCost FROM _products_user_real")

		return rows

def updateShopInfo(settings, datas):
	userInfoType = settings['shopInfoType']

	with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True) as commander:
		for data in datas:
			if userInfoType == 'allList':
				commander.execute("UPDATE _products_all_list SET productId = %s , aosStoreProductId = %s, iosStoreProductId = %s, startDateTime = %s, endDateTime = %s, productTableType = %s, storeType = %s,  productType = %s, countOfPurchases =%s, materialType = %s,  saleCost = %s , cost = %s WHERE seq = %s", 
					  str(data['productId']), str(data['aosStoreProductId']), str(data['iosStoreProductId']), str(data['startDateTime']),str(data['endDateTime']),  str(data['productTableType']),str(data['storeType']), str(data['productType']),str(data['countOfPurchases']),str(data['materialType']),  str(data['saleCost']),  str(data['cost']), str(data['seq']) )


			elif userInfoType == 'productsText':
				commander.execute("UPDATE _products_text SET productText= %s  WHERE productId = %s and languageType= %s", 
					    str(data['productText']), str(data['productId']), str(data['languageType']) )

			elif userInfoType == 'products':
				commander.execute("UPDATE _products SET productId = %s, rewardType = %s, rewardId = %s WHERE seq = %s", 
					  str(data['productId']), str(data['rewardType']), str(data['rewardId']), str(data['seq']))

			elif userInfoType == 'rewards':
				commander.execute("UPDATE _rewards SET rewardId =%s, itemId =%s  WHERE seq = %s",
					 str(data['rewardId']), str(data['itemId']), str(data['seq']))

			elif userInfoType == 'items':
				commander.execute("UPDATE _items SET itemId = %s, itemType = %s, id = %s, minCount =%s, MaxCount =%s, probability  =%s WHERE seq = %s;"
					  ,str(data['itemId']), str(data['itemType']), str(data['id']), str(data['minCount']), str(data['MaxCount']), str(data['probability']), str(data['seq']))

			elif userInfoType == 'eventDigital':
				commander.execute("UPDATE _products_event_digital SET  startDateTime = %s, endDateTime = %s, storeType = %s,  productType = %s, materialType = %s, saleCost = %s, cost = %s, countOfPurchases = %s WHERE productId = %s;"
					  ,str(data['startDateTime']), str(data['endDateTime']), str(data['storeType']),  str(data['productType']), str(data['materialType']), str(data['saleCost']), str(data['cost']), str(data['countOfPurchases']), str(data['productId']))

			elif userInfoType == 'eventReal':
				commander.execute("UPDATE _products_event_real SET  startDateTime = %s, endDateTime = %s, platform = %s, storeProductId = %s, storeType = %s,  productType = %s, saleCost = %s, cost = %s, countOfPurchases = %s WHERE productId = %s;"
					  ,str(data['startDateTime']), str(data['endDateTime']), str(data['platform']), str(data['storeProductId']), str(data['storeType']),  str(data['productType']), str(data['saleCost']), str(data['cost']), str(data['countOfPurchases']), str(data['productId']))

			elif userInfoType == 'fixDigital':
				commander.execute("UPDATE _products_fix_digital SET  storeType = %s,  productType = %s, materialType = %s, cost = %s WHERE productId = %s;"
					  , str(data['storeType']),  str(data['productType']), str(data['materialType']), str(data['cost']), str(data['productId']))

			elif userInfoType == 'fixReal':
				commander.execute("UPDATE _products_fix_real SET   platform = %s, storeProductId = %s, storeType = %s, productType = %s,  cost = %s WHERE productId = %s;"
					  , str(data['platform']), str(data['storeProductId']), str(data['storeType']),  str(data['productType']),  str(data['cost']), str(data['productId']))

			elif userInfoType == 'userDigital':
				rows = commander.execute("UPDATE _products_user_digital set  saleDurationHour = %s, storeType = %s, productType = %s, materialType = %s, saleCost = %s, cost = %s, countOfPurchases = %s where productId = %s;",
							 str(data['saleDurationHour']), str(data['storeType']), str(data['productType']), str(data['materialType']), str(data['saleCost']),  str(data['cost']), str(data['countOfPurchases']),  str(data['productId']))
		
			elif userInfoType == 'userReal':
				rows = commander.execute("UPDATE _products_user_real SET  storeProductId = %s, saleDurationHour = %s, storeType = %s, productType = %s, saleCost = %s, cost = %s, countOfPurchases = %s WHERE platform = %s and productId = %s",
							 str(data['storeProductId']), str(data['saleDurationHour']), str(data['storeType']), str(data['productType']), str(data['saleCost']),  str(data['cost']), str(data['countOfPurchases']),  str(data['platform']),  str(data['productId']))
				 

def deleteShopInfo(settings, datas):
	userInfoType = settings['shopInfoType']

	with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True) as commander:
		for data in datas:
			if userInfoType == 'allList':
				commander.execute("DELETE FROM _products_all_list WHERE seq = %s;", str(data['seq']) )

			elif userInfoType == 'productsText':
				commander.execute("DELETE FROM _products_text WHERE productId = %s", str(data['productId']) )

			elif userInfoType == 'products':
				commander.execute("DELETE FROM _products WHERE seq = %s", str(data['seq']))

			elif userInfoType == 'rewards':
				commander.execute("DELETE FROM _rewards WHERE seq = %s", str(data['seq']))

			elif userInfoType == 'items':
				commander.execute("DELETE FROM _items  WHERE seq = %s;", str(data['seq']))

			elif userInfoType == 'eventDigital':
				commander.execute("DELETE FROM _products_event_digital  WHERE productId = %s;", str(data['productId']))

			elif userInfoType == 'eventReal':
				commander.execute("DELETE FROM _products_event_real  WHERE productId = %s;", str(data['productId']))

			elif userInfoType == 'fixDigital':
				commander.execute("DELETE FROM _products_fix_digital  WHERE productId = %s;", str(data['productId']))

			elif userInfoType == 'fixReal':
				commander.execute("DELETE FROM _products_fix_real  WHERE productId = %s;", str(data['productId']))

			elif userInfoType == 'userDigital':
				commander.execute("DELETE FROM _products_user_digital  WHERE productId = %s;", str(data['productId']))

			elif userInfoType == 'userReal':
				commander.execute("DELETE FROM _products_user_real WHERE productId = %s and platform = %s;", str(data['productId']), str(data['platform']))


def refreshShopInfo(request):
	if request.method == 'GET':
		return render(
			request,
			'app/shop/refreshShopInfo.html'
		)
	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		
		settings = response_data['settings']
		
		utcNow = datetime.utcnow()
		utcNowAddTime = datetime.utcnow() + timedelta(hours=3)

		with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True) as commander:
			#rows = commander.execute("SELECT productId, aosStoreProductId, iosStoreProductId, startDateTime, endDateTime, productTableType, storeType, productType, countOfPurchases, materialType, saleCost, cost FROM _products_all_list  WHERE endDateTime > %s  AND startDateTime <= %s;",
			#		 utcNow, utcNowAddTime);

			#commander.execute("truncate table _products_fix_digital;")
			#commander.execute("truncate table _products_event_digital;")
			#commander.execute("truncate table _products_fix_real;")
			#commander.execute("truncate table _products_event_real;")
			

			rows = commander.execute("SELECT productId, aosStoreProductId, iosStoreProductId, startDateTime, endDateTime, productTableType, storeType, productType, countOfPurchases, materialType, saleCost, cost FROM _products_all_list"	 );

			for row in rows:
				if row['productTableType'] == 1:
					commander.execute("INSERT INTO _products_fix_digital(productId, storeType, productType, materialType,  cost) VALUES(%s,%s,%s,%s,%s) ON DUPLICATE KEY UPDATE "+
					   " storetype = %s,  productType =%s, materialType =%s, cost = %s;",
					  str(row['productId']),  str(row['storeType']),  str(row['productType']), str(row['materialType']),  str(row['cost']),
					   str(row['storeType']),  str(row['productType']), str(row['materialType']),  str(row['cost']))
				elif row['productTableType'] == 2:
					commander.execute("INSERT INTO _products_event_digital (productId, startDateTime, endDateTime, storeType,  productType, materialType, saleCost, cost, countOfPurchases) "+
					   " VALUES(%s,%s,%s,%s, %s,%s,%s,%s,%s) ON DUPLICATE KEY UPDATE  startDateTime =%s, endDateTime = %s,  storetype = %s,  productType =%s, materialType = %s, saleCost =%s, cost = %s, countOfPurchases =%s;",
					  str(row['productId']), str(row['startDateTime']), str(row['endDateTime']),  str(row['storeType']),  str(row['productType']),str(row['materialType']), str(row['saleCost']), str(row['cost']),str(row['countOfPurchases']),
					  str(row['startDateTime']), str(row['endDateTime']),  str(row['storeType']),  str(row['productType']),str(row['materialType']), str(row['saleCost']), str(row['cost']),str(row['countOfPurchases']))
				elif row['productTableType'] == 3:
					commander.execute("INSERT INTO _products_fix_real (productId, platform, storeProductId, storeType,  productType,  cost) "+
					   " VALUES(%s,%s,%s,%s, %s,%s) ON DUPLICATE KEY UPDATE platform = %s, " +
					   " storeProductId = %s, storetype = %s, productType =%s, cost = %s;",
					  str(row['productId']), 1,  str(row['aosStoreProductId']), str(row['storeType']), str(row['productType']),  str(row['cost']),
					  1, str(row['aosStoreProductId']), str(row['storeType']),  str(row['productType']),  str(row['cost']))
					commander.execute("INSERT INTO _products_fix_real (productId, platform, storeProductId, storeType,  productType,  cost) "+
					   " VALUES(%s,%s,%s,%s, %s,%s) ON DUPLICATE KEY UPDATE platform = %s, " +
					   " storeProductId = %s, storetype = %s, productType =%s, cost = %s;",
					  str(row['productId']), 2,  str(row['iosStoreProductId']), str(row['storeType']), str(row['productType']),  str(row['cost']),
					  2, str(row['iosStoreProductId']), str(row['storeType']),  str(row['productType']),  str(row['cost']))
				elif row['productTableType'] == 4:
					commander.execute("INSERT INTO _products_event_real(productId, platform, storeProductId, startDateTime, endDateTime, storeType,  productType, saleCost, cost, countOfPurchases)"+
					   " VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s) ON DUPLICATE KEY UPDATE platform = %s, storeProductId = %s, startDateTime = %s, endDateTime = %s, storetype = %s,  productType = %s,  saleCost = %s, cost = %s, countOfPurchases = %s;",
					  str(row['productId']), 1, str(row['aosStoreProductId']), str(row['startDateTime']), str(row['endDateTime']), str(row['storeType']),  str(row['productType']), str(row['saleCost']), str(row['cost']),  str(row['countOfPurchases']),
					  1, str(row['aosStoreProductId']), str(row['startDateTime']), str(row['endDateTime']), str(row['storeType']), str(row['productType']), str(row['saleCost']), str(row['cost']),  str(row['countOfPurchases']))
					commander.execute("INSERT INTO _products_event_real(productId, platform, storeProductId, startDateTime, endDateTime, storeType,  productType, saleCost, cost, countOfPurchases)"+
					   " VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s) ON DUPLICATE KEY UPDATE platform = %s, storeProductId = %s, startDateTime = %s, endDateTime = %s, storetype = %s,  productType = %s,  saleCost = %s, cost = %s, countOfPurchases = %s;",
					  str(row['productId']), 2, str(row['iosStoreProductId']), str(row['startDateTime']), str(row['endDateTime']), str(row['storeType']),  str(row['productType']), str(row['saleCost']), str(row['cost']),  str(row['countOfPurchases']),
					  2, str(row['iosStoreProductId']), str(row['startDateTime']), str(row['endDateTime']), str(row['storeType']), str(row['productType']), str(row['saleCost']), str(row['cost']),  str(row['countOfPurchases']))

			return HttpResponse({}, content_type='application/json; charset=utf-8') 