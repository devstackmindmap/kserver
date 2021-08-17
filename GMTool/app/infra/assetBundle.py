from GMTool.settings import g_appStaticDir, g_tableUploadMode, g_commonconfig, g_runMode
from app.infra.db import *
from app.infra.utility import *
import json, datetime, time, os, sys, requests, socket
import hashlib
import zipfile


def updateAssetBundleAsFileMode(assetBundleType, runMode, comment, file_content, bundleVersion):
	cur_utc = str(int(time.time()))
	local_time = getLocalTime()

	#fileName = assetBundleType + '_' + cur_utc + '.zip'
	#fileName = assetBundleType + '.zip'
	fileName = assetBundleType
	path = g_appStaticDir +'assetBundle/' + runMode + '/'+ bundleVersion + '/'+ fileName
	listpath = 	g_appStaticDir +'assetBundle/' + runMode + '/'+bundleVersion+ '/'
	checkpath= 	g_appStaticDir +'assetBundle/' + runMode + '/'+ assetBundleType
	
	bundlePath = g_appStaticDir +'assetBundle/' + runMode + '/' +bundleVersion+ '/'

	if not os.path.isdir(bundlePath):
		os.mkdir(bundlePath)


	with open(path, 'wb') as tableFile:
		tableFile.write(file_content)

	url = g_commonconfig[g_runMode]['ServerSetting']['TableCdnUrl'] + 'static/assetBundle/' + runMode + '/' +bundleVersion+ '/' +fileName

	
	
	#assetzip = zipfile.ZipFile(path)
	#assetzip.extract(assetBundleType,listpath)

	#fileHash = getHash(checkpath)
	fileSize = os.path.getsize(path)
	fileExtension = os.path.splitext(path)
	fileExtensionType = fileExtension[1].replace(".","")
	#os.remove(checkpath)
	
	with dbConnector(auto_trans=True) as commander:
		orderNum = getAssetBundleOrderInfo(runMode, assetBundleType)

		commander.execute(
			"INSERT INTO asset_bundle(name, url, runMode, bundleVersion, orderNum, comment, createDate, mdChecksum, Bytes, fileExtensionType) VALUES(%s, %s, %s, %s, %s, %s, %s, %s, %s, %s)", 
			str(assetBundleType), str(url), str(runMode), str(bundleVersion), str(orderNum), str(comment), str(local_time), "임시", str(fileSize), str(fileExtensionType)
			#str(assetBundleType), str(url), str(runMode), str(orderNum), str(comment), str(local_time), str(fileHash), str(fileSize), str(fileExtensionType)
		)
	table_rows = ()
	with dbConnector() as commander:
		arg = (
			'AssetBundle',
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
		with open(listpath+'assetBundle.json', 'w', encoding="utf-8") as make_file:
			json.dump(table_add, make_file, ensure_ascii=False, indent="\t")

def updateAssetBundleVersionToWeb(runMode):
	headers = {'Content-Type': 'application/json; charset=utf-8'}
	data = {
		'Command': 'UpdateVersion'
	}

	success_web_set = set()

	rows = getActiveServerListOfAssetBundle(runMode)

	for row in rows:
		url = row['ip']
		serverName = row['serverName']

		try:
			if serverName == 'Web':
				res = requests.post(url + 'UpdateAssetBundle', headers=headers, data=json.dumps(data), timeout=3)
				if res.status_code == 200:
					success_web_set.add(url)

		except Exception as e:
			print(e)

	return success_web_set

def initAssetBundleVersionToWeb(runMode):
	headers = {'Content-Type': 'application/json; charset=utf-8'}
	data = {
		'Command': 'InitVersion'
	}

	success_web_set = set()

	rows = getActiveServerListOfAssetBundle(runMode)

	with dbConnector(auto_trans=True) as commander:
		commander.execute("UPDATE server_list SET assetBundleVersion = 0 WHERE runMode = %s", str(runMode))
	
		for row in rows:
			url = row['ip']
			serverName = row['serverName']

			try:
				if serverName == 'Web':
					res = requests.post(url + 'UpdateAssetBundle', headers=headers, data=json.dumps(data), timeout=3)
					if res.status_code == 200:
						success_web_set.add(url)

			except Exception as e:
				print(e)

	return success_web_set

def getActiveServerListOfAssetBundle(runMode):
	with dbConnector() as commander:
		rows = commander.execute("SELECT hostName, serverName, ip, runMode, assetBundleVersion, assetBundleUpdateTime, isRunning FROM server_list WHERE runMode = %s and serverName = 'Web' AND isRunning = 1", runMode)
		if rows:
			return rows

	return []

def getAssetBundleNames(runMode):
	with dbConnector() as commander:
		rows = commander.execute("select distinct name from asset_bundle where runMode = %s", str(runMode)) 

		return rows


def getAssetBundleOrderInfo(runMode, name):
	with dbConnector() as commander:
		rows = commander.execute("SELECT orderNum FROM asset_bundle WHERE runMode = %s and name = %s LIMIT 1", str(runMode), str(name))

		orderNum = 0
		if rows:
			orderNum = rows[0]['orderNum']

		return orderNum

def getHash(path, blocksize=65536):
    afile = open(path, 'rb')
    hasher = hashlib.md5()
    buf = afile.read(blocksize)
    while len(buf) > 0:
        hasher.update(buf)
        buf = afile.read(blocksize)
    afile.close()
    return hasher.hexdigest()