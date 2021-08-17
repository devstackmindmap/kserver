from django.shortcuts import render
from django.http import HttpResponse
from app.infra.db import *
from datetime import datetime
import json

def inquiryUserInfo(request):
	if request.method == 'GET':
		return render(
			request,
			'app/game/inquiryUserInfo.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		settings = response_data['settings']
		datas = {}
		if msg == 'getUserInfo':
			datas = getUserInfo(settings)
			datas = json.dumps(datas, ensure_ascii=False).encode('utf-8')

		elif msg == 'updateUserInfo':
			updateList = response_data['updateList']
			updateUserInfo(settings, updateList)

		elif msg == 'deleteUserInfo':
			deleteList = response_data['deleteList']
			deleteUserInfo(settings, deleteList)

		return HttpResponse(datas, content_type='application/json; charset=utf-8')


def getUserInfo(settings):
	userInfoType = settings['userInfoType']
	rows = {}

	with dbConnector(runMode=settings['runMode'], dbname='knightrun') as commander:
		userId = getUserId(commander, settings)

		if userInfoType == 'accounts':
			rows = commander.execute("SELECT userId, nickName FROM accounts WHERE userId = %s", str(userId))

		elif userInfoType == 'users':
			rows = commander.execute("SELECT gold , gem FROM users WHERE userId = %s", str(userId))

		elif userInfoType == 'units':
			rows = commander.execute("SELECT a.id, b.unitName, a.level, a.count, a.rankPoint, a.rankLevel FROM units AS a JOIN _common_unitName AS b ON a.id = b.unitId WHERE userId = %s", str(userId))

		elif userInfoType == 'cards':
			rows = commander.execute("SELECT a.id, b.cardName, a.level, a.count FROM cards AS a JOIN _common_cardName AS b ON a.id = b.cardId WHERE userId = %s", str(userId))

		elif userInfoType == 'stage_levels':
			rows = commander.execute("SELECT stageLevelId, clearCount FROM stage_levels WHERE userId = %s", str(userId))

		elif userInfoType == 'armors':
			rows = commander.execute("SELECT a.id, b.Name, a.level, a.count, a.unitId FROM armors AS a JOIN _common_equipName AS b ON a.id = b.Id WHERE userId =%s", str(userId))

		elif userInfoType == 'weapons':
			rows = commander.execute("SELECT a.id, b.Name, a.level, a.count, a.unitId FROM weapons AS a JOIN _common_equipName AS b ON a.id = b.Id WHERE userId =%s", str(userId))

		elif userInfoType == 'decks':
			rows = commander.execute("SELECT modeType, deckNum, orderNum, slotType, classId, deckName FROM decks WHERE userId = %s", str(userId))

		elif userInfoType == 'energy':
			rows = commander.execute("SELECT type, id, boxEnergy, userEnergy, userBonusEnergy FROM infusion_boxes  WHERE userID = %s", str(userId))

		return rows

def updateUserInfo(settings, datas):
	userInfoType = settings['userInfoType']

	with dbConnector(runMode=settings['runMode'], dbname='knightrun', auto_trans=True) as commander:
		userId = getUserId(commander, settings)

		for data in datas:
			if userInfoType == 'accounts':
				commander.execute("UPDATE accounts SET nickName = %s WHERE userId = %s", str(data['nickName']), str(userId))

			elif userInfoType == 'users':
				commander.execute("UPDATE users SET gold = %s , gem = %s WHERE userId = %s", str(data['gold']), str(data['gem']), str(userId))

			elif userInfoType == 'units':
				commander.execute("UPDATE units SET level = %s, count = %s, rankPoint = %s, rankLevel = %s WHERE userId = %s AND id = %s", str(data['level']), str(data['count']), str(data['rankPoint']), str(data['rankLevel']), str(userId), str(data['id']))

			elif userInfoType == 'cards':
				commander.execute("UPDATE cards SET level = %s, count = %s WHERE userId = %s AND id = %s", str(data['level']), str(data['count']), str(userId), str(data['id']))

			elif userInfoType == 'armors':
				commander.execute("UPDATE armors SET level = %s, count = %s, unitId = %s WHERE userId = %s AND id = %s", str(data['level']), str(data['count']), str(data['unitId']), str(userId), str(data['id']))

			elif userInfoType == 'weapons':
				commander.execute("UPDATE weapons SET level = %s, count = %s, unitId = %s WHERE userId = %s AND id = %s", str(data['level']), str(data['count']), str(data['unitId']), str(userId), str(data['id']))

			elif userInfoType == 'stage_levels':
				commander.execute("UPDATE stage_levels SET clearCount = %s WHERE userId = %s AND stageLevelId = %s", str(data['clearCount']), str(userId), str(data['stageLevelId']))

			elif userInfoType == 'decks':
				query = "UPDATE decks SET classId = {0}, deckName = '{1}' WHERE userId = {2} AND modeType = {3} AND deckNum = {4} AND slotType = {5} AND orderNum = {6}"\
							.format(data['classId'], data['deckName'], userId, data['modeType'], data['deckNum'], data['slotType'], data['orderNum'])

				commander.execute(query)

			elif userInfoType == 'energy':
				 commander.execute("UPDATE infusion_boxes SET id = %s, boxEnergy = %s, userEnergy =%s, userBonusEnergy =%s WHERE userId = %s AND type =%s" , str(data['id']) , str(data['boxEnergy']) , str(data['userEnergy']), str(data['userBonusEnergy']),  str(userId), str(data['type']) )

def deleteUserInfo(settings, datas):
	userInfoType = settings['userInfoType']

	with dbConnector(runMode=settings['runMode'], dbname='knightrun', auto_trans=True) as commander:
		userId = getUserId(commander, settings)

		for data in datas:
			if userInfoType == 'accounts':
				args = (
					userId,
				)
				commander.callproc('p_deleteUserDatas', args)

			elif userInfoType == 'users':
				commander.execute("DELETE FROM users WHERE userId = %s", str(userId))

			elif userInfoType == 'units':
				commander.execute("DELETE FROM units WHERE userId = %s AND id = %s", str(userId), str(data['id']))

			elif userInfoType == 'cards':
				commander.execute("DELETE FROM cards WHERE userId = %s AND id = %s", str(userId), str(data['id']))

			elif userInfoType == 'armors':
				commander.execute("DELETE FROM armors WHERE userId = %s AND id = %s", str(userId), str(data['id']))

			elif userInfoType == 'weapons':
				commander.execute("DELETE FROM weapons WHERE userId = %s AND id = %s", str(userId), str(data['id']))

			elif userInfoType == 'stage_levels':
				commander.execute("DELETE FROM stage_levels WHERE userId = %s AND stageLevelId = %s", str(userId), str(data['stageLevelId']))

			elif userInfoType == 'decks':
				commander.execute("DELETE FROM decks WHERE userId = %s AND modeType = %s AND deckNum = %s AND orderNum = %s AND slotType = %s AND classId = %s",
					str(userId), str(data['modeType']), str(data['deckNum']), str(data['orderNum']), str(data['slotType']), str(data['classId'])
				)
			elif userInfoType == 'energy':
				commander.execute("DELETE FROM infusion_boxes WHERE userId = %s AND type = %s AND id = %s", str(userId), str(data['type']), str(data['id']))

def getUserId(commander, settings):
	
	#identifierText = {"userID":0}
	identifierText = 0

	return identifierText

def allUserModify(request):
	if request.method == 'GET':
		return render(
			request,
			'app/game/allUserModify.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		data = response_data['updateMap']
		settings = response_data['settings']
		userInfoType = settings['userInfoType']

		with dbConnector(runMode=settings['runMode'], dbname='knightrun', auto_trans=True) as commander:
			userId = getUserId(commander, settings)
			
			if userInfoType == 'accounts':
				pass

			elif userInfoType == 'users':
				#commander.execute("UPDATE users SET gold = %s, gem = %s WHERE userId = 224", str(data['gold']), str(data['gem']))
				commander.execute("UPDATE users SET gold = %s ", str(data['gold']))

			elif userInfoType == 'units':
				#commander.execute("INSERT INTO units(userId, id, level, count, rankPoint, rankLevel) VALUES(%s, %s, %s, %s, %s, %s)", str(userId), str(data['id']), str(data['level']), str(data['count']), str(data['rankPoint']), str(data['rankLevel']))
				commander.execute("UPDATE units SET level =%s , count=%s , rankPoint= %s , rankLevel=%s WHERE id = %s ",  str(data['level']), str(data['count']), str(data['rankPoint']), str(data['rankLevel']), str(data['id']))

			elif userInfoType == 'cards':
				#commander.execute("INSERT INTO cards(userId, id, level, count) VALUES(%s, %s, %s, %s)", str(userId), str(data['id']), str(data['level']), str(data['count']))
				commander.execute("UPDATE cards SET level = %s, count=%s WHERE id = %s", str(data['level']), str(data['count']), str(data['id']))

			elif userInfoType == 'armors':
				#commander.execute("INSERT INTO armors(userId, id, level, count, unitId) VALUES(%s, %s, %s, %s, %s)", str(userId), str(data['id']), str(data['level']), str(data['count']), str(data['unitId']))
				commander.execute("UPDATE armors SET level = %s, count=%s WHERE id = %s", str(data['level']), str(data['count']), str(data['id']))

			elif userInfoType == 'weapons':
				#commander.execute("INSERT INTO weapons(userId, id, level, count, unitId) VALUES(%s, %s, %s, %s, %s)", str(userId), str(data['id']), str(data['level']), str(data['count']), str(data['unitId']))
				commander.execute("UPDATE weapons SET level =%s, count=%s WHERE id = %s",  str(data['level']), str(data['count']), str(data['id']))
			elif userInfoType == 'allUnit':
				units = [1001,1002,1003,1004,1005,1006,1007,1008,1009,1010,1011,1012]
				userIds = commander.execute("select distinct(userid) from units;")
				for userId in userIds:
					for unit in units:
						commander.execute("Insert IGNORE  into units(userId, id, level ) values(%s, %s, %s)", userId['userid'], unit, str(data['level']))
			elif userInfoType == 'allSkill':
				skills = [101,102,103,104,105,201,202,203,204,205,301,302,303,304,305,401,402,403,404,405,501,502,503,504,505,601,602,603,604,605,701,702,703,704,705,801,802,803,804,805,901,902,903,904,905,1001,1002,1003,1004,1005,1101,1102,1103,1104,1105,1201,1202,1203,1204,1205]
				userIds = commander.execute("select distinct(userid) from cards;")
				for userId in userIds:
					for skill in skills:
						commander.execute("Insert IGNORE  into cards(userId, id,level ) values(%s, %s, %s)", userId['userid'], skill, str(data['level']))
			elif userInfoType == 'allWeapons':
				weapons = [1001,1002,1003,1004,1005,1006,1007,1008,1009,1010,1011]
				userIds = commander.execute("select distinct(userid) from cards;")
				for userId in userIds:
					for skill in weapons:
						commander.execute("Insert IGNORE  into weapons(userId, id, level ) values(%s, %s, %s)", userId['userid'], skill, str(data['level']))

			elif userInfoType == 'allUnitsLevel':
				commander.execute("update units set level = %s ;",str(data['level']) )
			
			elif userInfoType == 'allSkillsLevel':
				commander.execute("update cards set level = %s ;",str(data['level']) )
			
			elif userInfoType == 'allWeaponsLevel':
				commander.execute("update weapons set level = %s ;",str(data['level']) )
				





		return HttpResponse({}, content_type='application/json; charset=utf-8')
