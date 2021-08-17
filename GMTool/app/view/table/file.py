from django.shortcuts import render, redirect
from django.http import HttpResponse
from GMTool.settings import g_appStaticDir, g_tableUploadMode, g_commonconfig, g_runMode
from app.infra.db import *
from app.infra.table import *
from app.infra.utility import *
import json, datetime, time, os, sys, requests
from time import time
import boto3

awsCloudFrontDev = 'EW28QOIUS9NSX'
awsCloudFrontLive = 'E3U4VPGXSENWQJ'


def transferTable(request):
	if request.method == 'POST':
		from_runMode = request.POST['from_runMode']
		to_runMode = request.POST['to_runMode']

		table_rows = ()
		with dbConnector(runMode = from_runMode) as commander:
			arg = (
				'Table',
				0,
				from_runMode
			)

			commander.callproc('p_getFileLatestVersion', arg)
			table_rows = commander.getLastRows()

		with dbConnector(runMode = to_runMode, auto_trans = True) as commander:
			for row in table_rows:
				tableName = row['name']
				if tableName == 'server_info':
					continue

				url = row['url']
				orderNum = 0
				isSelected = 1
				datas = commander.execute("SELECT orderNum, isSelected FROM game_table WHERE name = %s AND runMode = %s LIMIT 1", str(tableName), str(to_runMode))
				if datas:
					orderNum = datas[0]['orderNum']
					isSelected = datas[0]['isSelected']
		
				query = "INSERT INTO game_table(name, url, runMode, orderNum, isSelected) VALUES('{0}', '{1}', '{2}', {3}, {4})"\
						.format(tableName, url, to_runMode, orderNum, isSelected)
				commander.execute(query)

		return redirect('updateTableVersion')

	else:
		return render(
			request,
			'app/table/transferTable.html'
		)

def inquiryTableHistory(request):
	runMode = g_runMode
	if request.method == 'POST':
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		runMode = response_data['runMode']

		listpath = g_appStaticDir +'table/' + runMode + '/'
		if msg == 'getTables':
			tableType = response_data['tableType']

			with dbConnector() as commander:
				queries = "select version, name, bundleVersion, url, createDate, comment FROM game_table where name = %s and runMode = %s order by version desc limit 100;"
				rows = commander.execute(queries, str(tableType), str(runMode))

				data = json.dumps(rows, cls=DateTimeEncoder).encode('utf-8')
				
				return HttpResponse(data, content_type='application/json; charset=utf-8')
				

		elif msg == 'deleteTables':
			deletionVersionList = response_data['deletionVersionList']
			bundleVersion = response_data['bundleVersion']	
			bundlePath = g_appStaticDir +'table/' + runMode + '/' +bundleVersion+ '/'

			with dbConnector(auto_trans=True) as commander:		
				queries = "delete from game_table where version = %s and runMode = %s;"
				for version in deletionVersionList:
					commander.execute(queries, str(version), str(runMode))
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
	
				return HttpResponse({}, content_type='application/json; charset=utf-8')


	context = {
		'tableNames': getTableNames(runMode),
		'bundleVersions' : getBundleVersion(runMode)
	}

	return render(
		request,
		'app/table/inquiryTableHistory.html',
		context
	)

def inquiryTableContent(request):
	context = {}

	runMode = g_runMode
	if request.method == 'POST':
		runMode = request.POST.get('runMode')
		tableType = request.POST.get('tableType')

		with dbConnector() as commander:
			queries = "SELECT version, name, url, createDate, comment FROM game_table WHERE name = %s and runMode = %s ORDER BY version DESC LIMIT 1;"
			table_info = commander.execute(queries, str(tableType), str(runMode))
			if table_info:
				table_info = table_info[0]
				url = table_info['url']
				if url == '' or url is None:
					return;

				result = requests.get(url)
				content_rows = result.content.split(b'\r\n')
				head_rows = content_rows[0].split(b'|')
				del content_rows[0]

				body_rows = []
				for row in content_rows:
					if row:
						split_rows = row.split(b'|')

						if len(head_rows) == len(split_rows):
							body_rows.append(split_rows)

				context['table_info'] = table_info
				context['content_rows'] = body_rows
				context['head_rows'] = head_rows
	
	context['tableTypeNames'] = getTableNames(runMode)

	return render(
        request,
        'app/table/inquiryTableContent.html',
        context
    )

def downloadFile(request):
	if request.method != 'GET':
		raise Exception("invalid method type")

	fileName = request.GET['fileName']
	path = g_appStaticDir + 'table/' + fileName

	with open(path, 'r') as tableFile:
		file_content = tableFile.readlines()
		file_content = ''.join(file_content)
		file_content = file_content.replace('\n\n', '\r\n').encode('utf-8')

	response = HttpResponse(file_content, content_type='text/csv; charset={0}'.format('utf-8'))
	response['Content-Disposition'] = 'attachment; filename=' + fileName

	return response

def updateTableVersion(request):
	context = {
		'runMode': g_runMode,
		'server_list': []
	}

	success_web_set = None

	runMode = context['runMode']
	if request.method == 'GET' and 'runMode' in request.GET:
		runMode = request.GET['runMode']		

	elif request.method == 'POST' and 'runMode' in request.POST:
		runMode = request.POST['runMode']
		tableSubmitType = request.POST['TableSubmitType']

		if tableSubmitType == '업데이트':
			success_web_set = updateGameTableVersion(runMode)
		elif tableSubmitType == '초기화':
			success_web_set = initTableVersionToWeb(runMode)

	context['runMode'] = runMode
	button_group = getHtmlOfButtonGroup(runMode)
	context['button_group'] = button_group

	rows = getActiveServerListOfTable(runMode)
	if rows:
		context['server_list'] = rows
		if success_web_set is not None:
			for row in rows:
				if row['ip'] in success_web_set:
					row['isSuccess'] = True

				else:
					row['isSuccess'] = False

	return render(
		request,
		'app/table/updateTableVersion.html',
		 context
	)

def updateOrderAndSelctionInfo(request):
	context = {}

	if request.method == 'POST':
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		runMode = response_data['runMode']

		if msg == 'get':
			with dbConnector() as commander:
				query = "SELECT name, orderNum, isSelected FROM game_table WHERE runMode = '{0}' GROUP BY name ORDER BY orderNum DESC".format(runMode)
				rows = commander.execute(query)

				data = json.dumps(rows).encode('utf-8')				
				return HttpResponse(data, content_type='application/json; charset=utf-8')

		elif msg == 'update':
			updateList = response_data['updateList']
			
			with dbConnector(auto_trans=True) as commander:
				for updateInfo in updateList:
					name = updateInfo['name']
					orderNum = updateInfo['orderNum']
					isSelected = int(updateInfo['isSelected'])

					query = "UPDATE game_table SET orderNum = {0}, isSelected = {1} WHERE name = '{2}' and runMode = '{3}'".format(orderNum, isSelected, name, runMode)
					commander.execute(query)

				return HttpResponse('', content_type='application/json; charset=utf-8')

	return render(
		request,
		'app/table/updateTableOrderAndSelectionInfo.html',
		context
	)

def tableApartInfo(request):
	context = {}

	if request.method == 'POST':
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		

		if msg == 'get':
			with dbConnector() as commander:
				query = "SELECT name, used, ifNull(languageType,'NULL') as languageType FROM _table_target_list ORDER BY name ASC;"
				rows = commander.execute(query)

				data = json.dumps(rows).encode('utf-8')				
				return HttpResponse(data, content_type='application/json; charset=utf-8')

		elif msg == 'update':
			updateList = response_data['updateList']
			
			with dbConnector(auto_trans=True) as commander:
				for updateInfo in updateList:
					name = updateInfo['name']
					used = updateInfo['used']
					languageType = updateInfo['languageType']
					if languageType == 'NULL': 
						query = "UPDATE _table_target_list SET name = '{0}', used = {1} , languageType = NULL WHERE name = '{2}'; ".format(name, used,  name)
					else:
						query = "UPDATE _table_target_list SET name = '{0}', used = {1}, languageType = '{2}' WHERE name = '{3}'; ".format(name, used, languageType, name)

					commander.execute(query)

				return HttpResponse('', content_type='application/json; charset=utf-8')

	return render(
		request,
		'app/table/tableApartInfo.html',
		context
	)



def uploadMultiTable(request):
	if request.method == 'POST':
		runMode = request.POST.get('runMode')
		files = request.FILES.getlist('filesToUpload[]')
		comment = request.POST.get('comment')
		bundleVersion = request.POST.get('bundleVersion')

		path = '/table/' + runMode + '/*'

		for file in files:
			filename = file.name.split('.')
			file_type = filename[0]
			file_format = filename[1]

			file_content = file.read()
			file_content = file_content.decode('utf-8-sig')
			file.seek(0)
			#파일 내용 유효성 검사 필요

			if g_tableUploadMode == 'File':
				updateTableAsFileMode(file_type, file_format, runMode, comment, file_content, bundleVersion )

			elif g_fileUploadMode == 'S3':
				pass

		awsCloudFront(runMode, path)
		
		return redirect('updateTableVersion')

	else:
		return render(
			request,
			'app/table/uploadMultiTable.html',
			{}
		)

def uploadMultiDB(request):
	if request.method == 'POST':
		RunMode = request.POST.get('runMode')
		files = request.FILES.getlist('filesToUpload[]')
		comment = request.POST.get('comment')
		

		for file in files:
			filename = file.name.split('.')
			file_type = filename[0]
			file_format = filename[1]

			file_content = file.read()
			file_content = file_content.decode('utf-8-sig')
			file.seek(0)
			#파일 내용 유효성 검사 필요

			if g_tableUploadMode == 'File':
				updateTableAsDB(file_type, file_format, RunMode, comment, file_content)

			elif g_fileUploadMode == 'S3':
				pass

		return redirect('uploadMultiDB')

	else:
		return render(
			request,
			'app/table/uploadMultiDB.html',
			{}
		)

def viewStoreSchedule(request):
	if request.method == 'POST':
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)
		RunMode = response_data['runMode']
		tableName = response_data['tableName']
		msg = response_data['msg']
		if msg == 'viewStoreSchedule':
			if tableName == 'schedules':
				with dbConnector(runMode=RunMode, dbname='account', auto_trans=True) as commander:
					query = "SELECT scheduleId, scheduleType, DATE_FORMAT(startDateTime,'%Y-%m-%d %H:%i:%S') as startDateTime, DATE_FORMAT(endDateTime,'%Y-%m-%d %H:%i:%S') as endDateTime  FROM _schedules"
					rows = commander.execute(query)
					datas = rows;
					data = json.dumps(rows).encode('utf-8')
			
					return HttpResponse(data, content_type='application/json; charset=utf-8')
			if tableName == 'item':
				with dbConnector(runMode=RunMode, dbname='account', auto_trans=True) as commander:
					query = "SELECT itemId, itemType, id, value FROM _items"
					rows = commander.execute(query)
					datas = rows;
					data = json.dumps(rows).encode('utf-8')
			
					return HttpResponse(data, content_type='application/json; charset=utf-8')
			if tableName == 'store':
				with dbConnector(runMode=RunMode, dbname='account', auto_trans=True) as commander:
					query = "SELECT storeId, storeType, goodsImage, text, purchaseMaterialType, cost, itemId, scheduleId from _store"
					rows = commander.execute(query)
					datas = rows;
					data = json.dumps(rows).encode('utf-8')
			
					return HttpResponse(data, content_type='application/json; charset=utf-8')				
		if msg == 'updateStoreInfo':
			if tableName == 'schedules':
				with dbConnector(runMode=RunMode, dbname='account', auto_trans=True) as commander:
					for data in response_data['updateList']:
						#sdate = datetime.datetime.strptime(data['startDateTime'],"%Y-%m-%d %H:%M:%S")
						#edate = datetime.datetime.strptime(data['endDateTime'],"%Y-%m-%d %H:%M:%S")

						commander.execute("update _schedules set scheduleType =  %s, startDateTime =  %s, endDateTime =  %s WHERE scheduleId =  %s;",str(data['scheduleType']), data['startDateTime'], data['endDateTime'], str(data['scheduleId']))
			
					return HttpResponse({}, content_type='application/json; charset=utf-8')		
				
			if tableName == 'item':
				with dbConnector(runMode=RunMode, dbname='account', auto_trans=True) as commander:
					for data in response_data['updateList']:
						commander.execute("UPDATE _items SET itemType = %s, id = %s, value = %s WHERE itemId = %s",str(data['itemType']), str(data['id']), str(data['value']), str(data['itemId']))
					
					return HttpResponse({}, content_type='application/json; charset=utf-8')				

			if tableName == 'store':
				with dbConnector(runMode=RunMode, dbname='account', auto_trans=True) as commander:
					for data in response_data['updateList']:
						commander.execute("UPDATE _store SET storeType = %s, goodsImage =%s, text = %s, purchaseMaterialType =%s , cost =%s , itemId = %s, scheduleId = %s WHERE storeID = %s",str(data['storeType']),str(data['goodsImage']),str(data['text']),str(data['purchaseMaterialType']),str(data['cost']),str(data['itemId']),str(data['scheduleId']),str(data['storeId']))
			
				return HttpResponse({}, content_type='application/json; charset=utf-8')				
	else:
		return render(
			request,
			'app/table/viewStoreSchedule.html',
			{}
		)


def awsCloudFront(runMode, path):
	boto3Client = boto3.client('cloudfront')
	
	DestId = awsCloudFrontDev
	if runMode == 'Live' or runMode == 'Review' or runMode == 'Staging' :
		DestId = awsCloudFrontLive

	response = boto3Client.create_invalidation(
	DistributionId=DestId,
	InvalidationBatch ={
		'Paths' : {
			'Quantity' : 1,
			'Items' : [
				path
				]
			},
		'CallerReference' : str(time()).replace(".","")
		}
	)
	