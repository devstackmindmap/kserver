from django.shortcuts import render
from django.http import HttpResponse
from app.infra.db import *
from datetime import datetime
import json
import redis
from GMTool.settings import g_commonconfig, g_runMode

def inquiryClanInfo(request):
	if request.method == 'GET':
		return render(
			request,
			'app/game/inquiryClanInfo.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		settings = response_data['settings']
		datas = {}
		if msg == 'getClanInfo':
			datas = getClanInfo(settings)
			datas = json.dumps(datas, ensure_ascii=False).encode('utf-8')

		elif msg == 'updateClanInfo':
			updateList = response_data['updateList']
			updateClanInfo(settings, updateList)

		elif msg == 'deleteClanInfo':
			deleteList = response_data['deleteList']
			deleteClanInfo(settings, deleteList)

		return HttpResponse(datas, content_type='application/json; charset=utf-8')

def getClanInfo(settings):
	userInfoType = settings['userInfoType']
	rows = []

	with dbConnector(runMode=settings['runMode'], dbname='account') as commander:
		clanId = getClanId(commander, settings)

		if userInfoType == 'clan':
			rows = commander.execute("SELECT clanId, clanName, countryCode, clanPublicType, joinConditionRankPoint, clanMasteruserId, clanSymbolId, rankPoint, memberCount, inviteCode, clanExplain FROM clans WHERE clanId = %s",
							str(clanId))

		elif userInfoType == 'clanMembers':
			rows = commander.execute("SELECT seq, clanId, userId, memberGrade FROM clan_members WHERE clanId = %s;", str(clanId))
		
		
		return rows

def updateClanInfo(settings, datas):
	userInfoType = settings['userInfoType']

	with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True) as commander:
		clanId = getClanId(commander, settings)

		for data in datas:
			if userInfoType == 'clan':
				commander.execute("UPDATE clans SET clanName = %s, countryCode = %s, clanPublicType = %s, joinConditionRankPoint = %s, clanMasteruserId = %s, clanSymbolId = %s, rankPoint = %s, memberCount = %s, inviteCode = %s, clanExplain = %s WHERE clanId = %s;",
					 str(data['clanName']), str(data['countryCode']), str(data['clanPublicType']), str(data['joinConditionRankPoint']), str(data['clanMasteruserId']), str(data['clanSymbolId']), str(data['rankPoint']), str(data['memberCount']), str(data['inviteCode']), str(data['clanExplain']), str(clanId))

			elif userInfoType == 'clanMembers':
				commander.execute("UPDATE clan_members SET  clanId = %s, userId = %s, memberGrade = %s WHERE seq = %s;", 
					  str(data['clanId']), str(data['userId']), str(data['memberGrade']),  str(data['seq']))



def deleteClanInfo(settings, datas):
	userInfoType = settings['userInfoType']

	with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True) as commander:
		clanId = getClanId(commander, settings)

		for data in datas:
			if userInfoType == 'clan':
				commander.execute("DELETE FROM clans WHERE clanId = %s", str(clanId))
					

			elif userInfoType == 'clanMember':
				commander.execute("DELETE FROM clan_members WHERE seq = %s", str(data['seq']))



def getClanId(commander, settings):
	identifierText = settings['identifierText']
	userInfoType = settings['userInfoType']
	with dbConnector(runMode=settings['runMode'], dbname='account') as command:
		if settings['identifierType'] == 'clanName':
			query = "SELECT clanId FROM clans WHERE clanName = %s;"

			rows = command.execute(query, str(identifierText))
			identifierText = rows[0]['clanId']
		

	return identifierText

def getClanName(commander, settings):
	identifierText = settings['identifierText']
	userInfoType = settings['userInfoType']
	with dbConnector(runMode=settings['runMode'], dbname='account') as command:
		if settings['identifierType'] == 'clanId':
			query = "SELECT clanName FROM clans WHERE clanId = %s;"

			rows = command.execute(query, str(identifierText))
			identifierText = rows[0]['clanName']
		

	return identifierText


