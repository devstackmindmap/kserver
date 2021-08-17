from GMTool.settings import g_appStaticDir, g_tableUploadMode, g_commonconfig, g_runMode, s3_DevBucket, s3_LiveBucket, AWS_ACCESS_KEY, AWS_SECRET_KEY
from app.infra.db import *
from app.infra.utility import *
import json, datetime, time, os, sys, requests, socket
import os, sys
import hashlib
import csv
import boto3



def updateTableAsFileMode(tableType, fileFormat, runMode, comment, file_content, bundleVersion):
	cur_utc = str(int(time.time()))
	local_time = getLocalTime()

	#fileName = tableType + '_' + cur_utc + '.' + fileFormat
	fileName = tableType + '.' + fileFormat
	path = g_appStaticDir +'table/' + runMode + '/'+ bundleVersion+ '/' +fileName
	s3_path = 'table/' + runMode + '/'+ bundleVersion+ '/' +fileName
	listpath = g_appStaticDir +'table/' + runMode + '/'
	bundlePath = g_appStaticDir +'table/' + runMode + '/' +bundleVersion+ '/'
	s3_bundlePath = 'table/' + runMode + '/' +bundleVersion+ '/'

	if not os.path.isdir(bundlePath):
		os.mkdir(bundlePath)

	with open(path, 'w', encoding='utf8') as tableFile:
		tableFile.write(file_content)

	s3upload(file_content, s3_path, runMode)

	#url = g_commonconfig[g_runMode]['ServerSetting']['TableCdnUrl'] + 'static/table/' + runMode + '/' +bundleVersion+ '/' +fileName
	url =  g_commonconfig[runMode]['ServerSetting']['TableCdnUrl'] + s3_bundlePath + fileName

	fileHash = getHash(path)
	fileSize = os.path.getsize(path)
	fileExtension = os.path.splitext(path)
	fileExtensionType = fileExtension[1].replace(".","")
	
	with dbConnector(auto_trans=True) as commander:
		commander.execute(
			"INSERT INTO game_table(name, url, bundleVersion, runMode, comment, createDate, mdChecksum, Bytes, fileExtensionType) VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s)", 
			str(tableType), str(url), str(bundleVersion), str(runMode), str(comment), str(local_time), str(fileHash), str(fileSize), str(fileExtensionType)
		)
	table_rows = {}
	with dbConnector() as commander:
		arg = (
			'Table',
			0,
			runMode,
			bundleVersion
		)
		commander.callproc('p_getFileLatestVersionOptimaizer', arg)
		table_rows = commander.getLastRows()
		totalversion = 0
		for row in table_rows:
			if totalversion < row['Version']:
				totalversion = row['Version']

		table_add = {'TotalVersion' : totalversion, 'CDN': table_rows}
		with open(bundlePath+'table.json', 'w', encoding="utf-8") as make_file:
			json.dump(table_add, make_file, ensure_ascii=False, indent="\t")
			
		
		with open(bundlePath+'table.json','rb') as f:
			s3upload(f, s3_bundlePath+'table.json', runMode)
		

def updateGameTableVersion(runMode):
	headers = {'Content-Type': 'application/json; charset=utf-8'}
	data = {
		'Command': 'UpdateVersion'
	}

	success_web_set = set()

	rows = getActiveServerListOfTable(runMode)

	for row in rows:
		url = row['ip']
		serverName = row['serverName']

		try:
			if serverName == 'Web':
				res = requests.post(url + 'UpdateWebTable', headers=headers, data=json.dumps(data), timeout=3)
				if res.status_code == 200:
					success_web_set.add(url)

			elif serverName == 'Battle':
				if sendJsonDataBySocket(url, "UpdateBattleTable", data) is not None:
					success_web_set.add(url)

		except Exception as e:
			print(e)

	return success_web_set

def initTableVersionToWeb(runMode):
	headers = {'Content-Type': 'application/json; charset=utf-8'}
	data = {
		'Command': 'InitVersion'
	}

	success_web_set = set()

	with dbConnector(auto_trans=True) as commander:
		commander.execute("UPDATE server_list SET tableVersion = 0 WHERE runMode = %s", runMode)

		rows = getActiveServerListOfTable(runMode)
		for row in rows:
			url = row['ip']
			serverName = row['serverName']
			
			try:
				if serverName == 'Web':
					res = requests.post(url + 'UpdateWebTable', headers=headers, data=json.dumps(data), timeout=3)
					if res.status_code == 200:
						success_web_set.add(url)

				elif serverName == 'Battle':
					if sendJsonDataBySocket(url, "UpdateBattleTable", data) is not None:
						success_web_set.add(url)

			except Exception as e:
				print(e)

	return success_web_set
		
def getActiveServerListOfTable(runMode):
	with dbConnector() as commander:
		rows = commander.execute("SELECT hostName, serverName, ip, runMode, tableVersion, tableUpdateTime, isRunning FROM server_list WHERE runMode = %s AND isRunning = 1", runMode)
		if rows:
			return rows

	return []


def s3upload(file_content, path, runMode):
	s3_conn = boto3.resource(
		's3',
		aws_access_key_id=AWS_ACCESS_KEY,
		aws_secret_access_key=AWS_SECRET_KEY,
		region_name='ap-northeast-2'
		)

	if runMode =='Live' or runMode == 'Review' or runMode == 'Staging':
		s3_conn.Bucket(s3_LiveBucket).put_object(Body=file_content, Key=path, ACL='public-read')
	else:
		s3_conn.Bucket(s3_DevBucket).put_object(Body=file_content, Key=path, ACL='public-read')




def getTableNames(runMode):
	with dbConnector() as commander:
		rows = commander.execute("select distinct name from game_table where runMode = %s", str(runMode)) 

		return rows

def getBundleVersion(runMode):
	with dbConnector() as commander:
		rows = commander.execute("select distinct bundleVersion from game_table where runMode = %s order by bundleVersion desc;", str(runMode))

		return rows

def getTableOrderInfo(runMode, name):
	with dbConnector() as commander:
		rows = commander.execute(
			"SELECT orderNum, isSelected FROM game_table WHERE runMode = %s and name = %s LIMIT 1",
			str(runMode), str(name)
		)
				
		isSelected = 1
		orderNum = 0
		if rows:
			orderNum = rows[0]['orderNum']
			isSelected = rows[0]['isSelected']
	
		return orderNum, isSelected

def getHash(path, blocksize=65536):
    afile = open(path, 'rb')
    hasher = hashlib.md5()
    buf = afile.read(blocksize)
    while len(buf) > 0:
        hasher.update(buf)
        buf = afile.read(blocksize)
    afile.close()
    return hasher.hexdigest()
 

def updateTableAsDB(tableType, fileFormat, RunMode, comment, file_content ):
	cur_utc = str(int(time.time()))
	local_time = getLocalTime()

	fileName = tableType + '_' + cur_utc + '.' + fileFormat
	path = g_appStaticDir +'table/' + RunMode + '/'+ fileName
		
	with open(path, 'w', encoding='utf8') as tableFile:
		tableFile.write(file_content)
	
	with dbConnector(runMode=RunMode, dbname='account', auto_trans=True) as commander:
		if(tableType == '_products_all_list'):
			tr = open(path,'r', encoding='utf-8',newline='\r\n')
			trow = csv.reader(tr,delimiter='|')
			
			line_count = 0
			commander.execute("truncate table _products_all_list;")
			for rr in trow:
				if line_count != 0:
					if len(rr) == 12:
						commander.execute("INSERT INTO _products_all_list(productId, aosStoreProductId, iosStoreProductId, startDateTime, endDateTime, productTableType, storeType,  productType, countOfPurchases, materialType, cost, saleCost ) VALUES(%s, %s, %s, %s, %s, %s, %s ,%s, %s ,%s, %s, %s)",
						str(rr[0]), str(rr[1]), str(rr[2]), str(rr[3]), str(rr[4]), str(rr[5]), str(rr[6]), str(rr[7]), str(rr[8]), str(rr[9]), str(rr[10]), str(rr[11]) )

				line_count +=1
			tr.close()
			os.remove(path)

		if(tableType == '_products_text'):
			tr = open(path,'r', encoding='utf-8',newline='\r\n')
			trow = csv.reader(tr,delimiter='|')
			line_count = 0
			commander.execute("truncate table _products_text;")
			for rr in trow:
				if line_count != 0:
					if len(rr) == 3:
						commander.execute("INSERT INTO _products_text(productId, languageType, productText) VALUES(%s, %s, %s)", str(rr[0]), str(rr[1]), str(rr[2]))

				line_count +=1
			tr.close()
			os.remove(path)

		if(tableType == '_products'):
			tr = open(path,'r', encoding='utf-8',newline='\r\n')
			trow = csv.reader(tr,delimiter='|')
			line_count = 0
			commander.execute("truncate table _products;")
			for rr in trow:
				if line_count != 0:
					if len(rr) == 3:
						commander.execute("INSERT INTO _products(productId, rewardType, rewardId ) VALUES(%s, %s, %s);", str(rr[0]), str(rr[1]), str(rr[2]) )

				line_count +=1
			tr.close()
			os.remove(path)

		if(tableType == '_rewards'):
			tr = open(path,'r', encoding='utf-8',newline='\r\n')
			trow = csv.reader(tr,delimiter='|')
			line_count = 0
			commander.execute("truncate table _rewards;")
			for rr in trow:
				if line_count != 0:
					if len(rr) == 2:
						commander.execute("INSERT INTO _rewards(rewardId, itemId) VALUES(%s, %s)", str(rr[0]), str(rr[1]))

				line_count +=1
			tr.close()
			os.remove(path)

		if(tableType == '_items'):
			tr = open(path,'r', encoding='utf-8',newline='\r\n')
			trow = csv.reader(tr,delimiter='|')
			line_count = 0
			commander.execute("truncate table _items;")
			for rr in trow:
				if line_count != 0:
					if len(rr) == 6:
						commander.execute("INSERT INTO _items(itemId, itemType, id, minCount, MaxCount, probability) VALUES(%s, %s, %s, %s, %s, %s)", str(rr[0]), str(rr[1]), str(rr[2]), str(rr[3]), str(rr[4]), str(rr[5]))

				line_count +=1
			tr.close()
			os.remove(path)

		if(tableType == 'data_user_product_digital'):
			tr = open(path,'r', encoding='utf-8',newline='\r\n')
			trow = csv.reader(tr,delimiter='|')
			line_count = 0
			commander.execute("truncate table _products_user_digital;")
			for rr in trow:
				if line_count != 0:
					if len(rr) == 8:
						commander.execute("INSERT INTO _products_user_digital(productId, saleDurationHour, storeType, productType, countOfPurchases, materialType, cost, saleCost) VALUES(%s, %s, %s, %s, %s, %s, %s, %s)"
							, str(rr[0]), str(rr[1]), str(rr[2]), str(rr[3]), str(rr[4]), str(rr[5]), str(rr[6]), str(rr[7]))

				line_count +=1
			tr.close()
			os.remove(path)

		if(tableType == 'data_user_product_real'):
			tr = open(path,'r', encoding='utf-8',newline='\r\n')
			trow = csv.reader(tr,delimiter='|')
			line_count = 0
			commander.execute("truncate table _products_user_real;")
			for rr in trow:
				if line_count != 0:
					if len(rr) == 9:
						commander.execute("INSERT INTO _products_user_real(productId, platform, storeProductId, saleDurationHour, storeType, productType, countOfPurchases, cost, saleCost) VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s)"
							, str(rr[0]), 1, str(rr[1]), str(rr[3]), str(rr[4]), str(rr[5]), str(rr[6]), str(rr[7]), str(rr[8]))
						commander.execute("INSERT INTO _products_user_real(productId, platform, storeProductId, saleDurationHour, storeType, productType, countOfPurchases, cost, saleCost) VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s)"
							, str(rr[0]), 2, str(rr[2]), str(rr[3]), str(rr[4]), str(rr[5]), str(rr[6]), str(rr[7]), str(rr[8]))

				line_count +=1
			tr.close()
			os.remove(path)

		#if(tableType == '_events_list'):
		#	tr = open(path,'r', encoding='utf-8',newline='\r\n')
		#	trow = csv.reader(tr,delimiter='|')
		#	line_count = 0
		#	#commander.execute("truncate table _events;")
		#	for rr in trow:
		#		if line_count != 0:
		#			if len(rr) == 3:
		#				commander.execute("INSERT ignore INTO _events(startDateTime, endDateTime, eventType ) VALUES(%s, %s, %s)"
		#					, str(rr[0]), str(rr[1]), str(rr[2]) )
						
		#		line_count +=1
		#	tr.close()
		#	os.remove(path)

		if 'salesreport' in tableType:
			with dbConnector(runMode=RunMode, dbname='kpi', auto_trans=True) as command:
				tr = open(path,'r', encoding='utf-8',newline='\n')
				trow = csv.reader(tr,delimiter=',')
				line_count = 0
				#commander.execute("truncate table _events;")
				for rr in trow:
					if line_count != 0:
						if str(rr[3]) == 'Refund':
							command.execute("INSERT ignore INTO refundlist(transactionId, productId, storeProductId, Refunding, orderDate) VALUES(%s, %s, %s, %s, %s)"
								, str(rr[0]), str(rr[5]), str(rr[8]),1,str(rr[1]) )
						
					line_count +=1
				tr.close()
				os.remove(path)
