from django.shortcuts import render, redirect
from django.views.decorators.csrf import csrf_exempt
from django.http import HttpResponse
from app.infra.assetBundle import *
from app.infra.utility import *
from GMTool.settings import g_runMode
import json


def transferAssetBundle(request):
	if request.method == 'POST':
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		from_runMode = response_data['runMode']
		to_runMode = response_data['to_runMode']
		selectedVersionList = response_data['selectedVersionList']

		asset_bundle_rows = []
		with dbConnector(runMode = from_runMode) as commander:
			for version in selectedVersionList:
				rows = commander.execute("SELECT name, url, comment FROM asset_bundle WHERE version = %s and runMode = %s", str(version), str(from_runMode)) 
				asset_bundle_rows.append(rows[0])

		with dbConnector(runMode = to_runMode, auto_trans = True) as commander:
			for row in asset_bundle_rows:
				orderNum = getAssetBundleOrderInfo(to_runMode, row['name'])

				commander.execute(
					"INSERT INTO asset_bundle(name, url, runMode, orderNum, comment) VALUES(%s, %s, %s, %s, %s)", 
					str(row['name']), str(row['url']), str(to_runMode), str(orderNum), str(row['comment'])
				)

		return HttpResponse('', content_type='application/json; charset=utf-8')

	else:	
		return render(
			request,
			'app/assetBundle/transferAssetBundle.html'
		)

def uploadMultiAssetBundle(request):
	if request.method == 'POST':
		runMode = request.POST.get('runMode')
		files = request.FILES.getlist('filesToUpload[]')
		comment = request.POST.get('comment')
		bundleVersion = request.POST.get('bundleVersion')

		for file in files:
			#filename = file.name.split('.')
			#assetBundleType = filename[0]
			#file_format = filename[1]

			assetBundleType = file.name

			file_content = file.read()
			file.seek(0)
			#파일 내용 유효성 검사 필요

			updateAssetBundleAsFileMode(assetBundleType, runMode, comment, file_content, bundleVersion)

		return redirect('updateAssetBundleVersion')

	else:
		return render(
			request,
			'app/assetBundle/uploadMultiAssetBundle.html'
		)

def updateAssetBundleVersion(request):
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
			success_web_set = updateAssetBundleVersionToWeb(runMode)
		elif tableSubmitType == '초기화':
			success_web_set = initAssetBundleVersionToWeb(runMode)

	context['runMode'] = runMode
	button_group = getHtmlOfButtonGroup(runMode)
	context['button_group'] = button_group

	rows = getActiveServerListOfAssetBundle(runMode)
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
		'app/assetBundle/updateAssetBundleVersion.html',
		 context
	)

def inquiryAssetBundle(request):
	runMode = g_runMode
	if request.method == 'POST':
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		runMode = response_data['runMode']
		listpath = g_appStaticDir +'assetBundle/' + runMode + '/'
		if msg == 'getAssetBundles':
			assetBundleType = response_data['assetBundleType']

			with dbConnector(runMode = g_runMode) as commander:
				queries = "select version, name, url, createDate, comment FROM asset_bundle where name = %s and runMode = %s order by version desc limit 100;"
				rows = commander.execute(queries, str(assetBundleType), str(runMode))

				data = json.dumps(rows, cls=DateTimeEncoder).encode('utf-8')
				
				return HttpResponse(data, content_type='application/json; charset=utf-8')

		elif msg == 'getLatestAssetBundles':
			with dbConnector(runMode = g_runMode) as commander:
				args = (
					'AssetBundle',
					0,
					runMode,
				)

				commander.callproc('p_getFileLatestVersion', args)
				rows = commander.getLastRows()

				rows = sorted(rows, key=lambda row: row['version'], reverse=True)
				data = json.dumps(rows, cls=DateTimeEncoder).encode('utf-8')
				
				return HttpResponse(data, content_type='application/json; charset=utf-8')
	
		elif msg == 'deleteAssetBundles':
			deletionVersionList = response_data['deletionVersionList']

			with dbConnector(runMode = g_runMode, auto_trans=True) as commander:		
				queries = "delete from asset_bundle where version = %s and runMode = %s;"
				for version in deletionVersionList:
					commander.execute(queries, str(version), str(runMode))
				table_rows = ()
				

				arg = (
				'AssetBundle',
				0,
				runMode
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
					
			return HttpResponse({}, content_type='application/json; charset=utf-8')


	context = {
		'assetBundleNames': getAssetBundleNames(runMode)
	}

	return render(
		request,
		'app/assetBundle/inquiryAssetBundle.html',
		context
	)


def updateAssetBundleOrder(request):
	context = {}

	if request.method == 'POST':
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		runMode = response_data['runMode']

		if msg == 'getAssetBundleOrderInfo':
			with dbConnector() as commander:
				query = "SELECT name, orderNum FROM asset_bundle WHERE runMode = '{0}' GROUP BY name ORDER BY orderNum DESC".format(runMode)
				rows = commander.execute(query)

				data = json.dumps(rows).encode('utf-8')				
				return HttpResponse(data, content_type='application/json; charset=utf-8')

		elif msg == 'updateAssetBundleOrder':
			updateList = response_data['updateList']
			
			with dbConnector(auto_trans=True) as commander:
				for updateInfo in updateList:
					name = updateInfo['name']
					orderNum = updateInfo['orderNum']

					query = "UPDATE asset_bundle SET orderNum = {0} WHERE name = '{1}' and runMode = '{2}'".format(orderNum, name, runMode)
					commander.execute(query)

				return HttpResponse('', content_type='application/json; charset=utf-8')

	return render(
		request,
		'app/assetBundle/updateAssetBundleOrder.html',
		context
	)


