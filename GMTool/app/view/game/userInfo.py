from django.shortcuts import render
from django.http import HttpResponse
from app.infra.db import *
from datetime import datetime
import json
import redis
from GMTool.settings import g_commonconfig, g_runMode, API_KEY
from pyfcm import FCMNotification

push_service = FCMNotification(api_key=API_KEY)


    
def default(self, obj):
	if isinstance(obj, bson.objectid.ObjectId):
		return str(obj)
	elif isinstance(obj, datetime.datetime):
		return obj.isoformat()
	return json.JSONEncoder.default(self, obj)

def myconverter(o):
    if isinstance(o, datetime):
        return o.__str__()



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
			datas = json.dumps(datas, ensure_ascii=False ,default=myconverter).encode('utf-8')

		elif msg == 'updateUserInfo':
			updateList = response_data['updateList']
			updateUserInfo(settings, updateList)

		elif msg == 'deleteUserInfo':
			deleteList = response_data['deleteList']
			deleteUserInfo(settings, deleteList)

		

		return HttpResponse(datas, content_type='application/json; charset=utf-8')

def addUserInfo(request):
	if request.method == 'GET':
		return render(
			request,
			'app/game/addUserInfo.html'
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
				pass

			elif userInfoType == 'units':
				commander.execute("INSERT INTO units(userId, id, level, count, maxRankLevel currentRankLevel, currentSeasonRankPoint ,nextSeasonRankPoint, skinId ) VALUES(%s, %s, %s, %s, %s, %s, %s ,%s)", str(userId), str(data['id']), str(data['level']), str(data['count']), str(data['maxRankLevel']), str(data['currentSeasonRankLevel']), str(data['currentSeasonRankPoint']), str(data['nextSeasonRankPoint']), str(data['skinId']))

			elif userInfoType == 'cards':
				commander.execute("INSERT INTO cards(userId, id, level, count) VALUES(%s, %s, %s, %s)", str(userId), str(data['id']), str(data['level']), str(data['count']))

			elif userInfoType == 'stage_levels':
				commander.execute("INSERT INTO stage_levels(userId, stageLevelId, clearCount) VALUES(%s, %s, %s)", str(userId), str(data['stageLevelId']), str(data['clearCount']))

			elif userInfoType == 'armors':
				commander.execute("INSERT INTO armors(userId, id, level, count, unitId) VALUES(%s, %s, %s, %s, %s)", str(userId), str(data['id']), str(data['level']), str(data['count']), str(data['unitId']))

			elif userInfoType == 'weapons':
				commander.execute("INSERT INTO weapons(userId, id, level, count, unitId) VALUES(%s, %s, %s, %s, %s)", str(userId), str(data['id']), str(data['level']), str(data['count']), str(data['unitId']))
			
			elif userInfoType == 'decks':
				commander.execute("INSERT INTO decks(userId, modeType, deckNum, orderNum, slotType, classId, deckName) VALUES(%s, %s, %s, %s, %s, %s, %s)",
					str(userId), str(data['modeType']), str(data['deckNum']), str(data['orderNum']), str(data['slotType']), str(data['classId']), str(data['deckName'])
				)

			elif userInfoType == 'energy':
			     commander.execute("INSERT INTO infusion_boxes(userId, type, id, BoxEnergy, userEnergy, userbonusEnergy, userEnergyRecentUpdateDatetime) VALUES(%s, %s, %s, %s, %s, %s, %s)", str(userId), 1, 1001, 100, 0, 100, datetime.now()  )

			elif userInfoType == 'give_skin':
				commander.execute("INSERT IGNORE INTO skins(userId, skinId) values(%s, %s)", str(userId), str(data['skinId']) )

			elif userInfoType == 'store_schedule':
				commander.execute("INSERT INTO _schedules(scheduleType,startDateTime,endDateTime) VALUES(%s, %s, %s)",str(data['scheduleType']), str(data['startDateTime']), str(data['endDateTime']))

		return HttpResponse({}, content_type='application/json; charset=utf-8')

def getUserInfo(settings):
	userInfoType = settings['userInfoType']
	rows = []

	with dbConnector(runMode=settings['runMode'], dbname='knightrun') as commander:
		userId = getUserId(commander, settings)

		if userInfoType == 'accounts':
			with dbConnector(runMode=settings['runMode'], dbname='account') as commander:
				rows = commander.execute('SELECT userId, nickName, currentSeason, maxRankLevel, currentSeasonRankPoint, maxRankPoint, DATE_FORMAT(loginDateTime, "%Y-%m-%d %T") as loginDateTime,  DATE_FORMAT(limitLoginDate, "%Y-%m-%d %T") as limitLoginDate, limitLoginReason  FROM accounts WHERE userId = %s', str(userId))

		elif userInfoType == 'users':
			rows = commander.execute("SELECT gold , gem, gemPaid, starCoin, soStartTicket,  level, exp FROM users WHERE userId = %s", str(userId))

		elif userInfoType == 'units':
			#rows = commander.execute("SELECT a.id, b.unitName, a.level, a.count, a.rankPoint, a.rankLevel FROM units AS a JOIN _common_unitName AS b ON a.id = b.unitId WHERE userId = %s", str(userId))
			rows = commander.execute("SELECT id , level, count, maxRankLevel, currentRankLevel, currentSeasonRankPoint ,nextSeasonRankPoint, currentVirtualRankLevel, currentVirtualRankPoint, skinId FROM units WHERE userId = %s", str(userId))

		elif userInfoType == 'cards':
			#rows = commander.execute("SELECT a.id, b.cardName, a.level, a.count FROM cards AS a JOIN _common_cardName AS b ON a.id = b.cardId WHERE userId = %s order by a.id asc", str(userId))
			rows = commander.execute("SELECT id, level, count FROM cards WHERE userId = %s order by id asc", str(userId))

		elif userInfoType == 'stage_levels':
			rows = commander.execute("SELECT stageLevelId, clearCount FROM stage_levels WHERE userId = %s", str(userId))

		elif userInfoType == 'armors':
			#rows = commander.execute("SELECT a.id, b.Name, a.level, a.count, a.unitId FROM armors AS a JOIN _common_equipName AS b ON a.id = b.Id WHERE userId =%s", str(userId))
			rows = commander.execute("SELECT id,  level, count, unitId FROM armors WHERE userId =%s", str(userId))

		elif userInfoType == 'weapons':
			#rows = commander.execute("SELECT a.id, b.Name, a.level, a.count, a.unitId FROM weapons AS a JOIN _common_equipName AS b ON a.id = b.Id WHERE userId =%s", str(userId))
			rows = commander.execute("SELECT id, level, count FROM weapons WHERE userId =%s", str(userId))

		elif userInfoType == 'decks':
			rows = commander.execute("SELECT modeType, deckNum, orderNum, slotType, classId FROM decks WHERE userId = %s order by deckNum, slotType asc", str(userId))

		elif userInfoType == 'energy':
			rows = commander.execute("SELECT type, id, boxEnergy, userEnergy, userBonusEnergy FROM infusion_boxes  WHERE userID = %s", str(userId))

		elif userInfoType == 'give_skin':
			rows = commander.execute("SELECT skinId FROM skins WHERE userID = %s", str(userId))

		elif userInfoType == 'square_object':
			rows = commander.execute("SELECT isActivated, activatedTime, nextInvasionTime, nextInvasionMonsterId, nextInvasionLevel, powerRefreshTime, objectPower,  activeObjectLevel, objectShield, planetBoxExp, planetBoxLevel, coreEnergy, energyRefreshTime, objectExp, objectLevel, enableContents, coreExp, coreLevel, agencyExp, agencyLevel FROM square_object_schedule where userId = %s;", str(userId))

		elif userInfoType == 'friendList':
			rows = commander.execute("SELECT userId, nickName from account.accounts where userId in (select friendId from friends where userId =%s);", str(userId))

		elif userInfoType == 'user_info':
			rows = commander.execute("SELECT userId, isAlreadyFreeNicknameChange, recentDateTimeCountryChange, dailyRankVictoryGoldRewardCount, dailyRankVictoryGoldRewardDateTime, rewardedRankSeason, unlockContents,  maxVirtualRankLevel, currentVirtualRankPoint,maxVirtualRankPoint from user_info where userId = %s;", str(userId))
			#rows = commander.execute("select friendId from friends where userId =%s;", str(userId))
			#with dbConnector(runMode=settings['runMode'], dbname='account') as commander:
			#	for id in ids:
			#		temp.append(str(id['friendId']))
			#	rp = ', '.join(map(str,temp))
				#rows = commander.execute("SELECT userId, nickName from accounts where userId in (select friendId from knightrun.friends where userId =%s);", str(userId))

		elif userInfoType == 'payment_info':
			with dbConnector(runMode=settings['runMode'], dbname='account') as commander:
				rows = commander.execute('SELECT seq, userId, transactionId, storeProductId, DATE_FORMAT(addDateTime, "%Y-%m-%d %T") as addDateTime FROM purchased WHERE userId = %s', str(userId))

		elif userInfoType == 'payment_pending':
			with dbConnector(runMode=settings['runMode'], dbname='account') as commander:
				rows = commander.execute('SELECT seq, userId, transactionId, is_pending, productId, productTableType, storeProductId, purchasedToken, platformType, DATE_FORMAT(payedTime, "%Y-%m-%d %T") as payedTime FROM pay_pending WHERE userId = %s', str(userId))

		elif userInfoType == 'payment_pending_issue':
			with dbConnector(runMode=settings['runMode'], dbname='account') as commander:
				rows = commander.execute('SELECT  userId, transactionId, is_pending, productId, productTableType, storeProductId, purchasedToken, platformType  FROM pay_pending_issue WHERE userId = %s', str(userId))
		
		elif userInfoType == 'privateMail':
			rows = commander.execute('SELECT mailId, userId, isDeleted, DATE_FORMAT(startDateTime, "%Y-%m-%d %T") as startDateTime,  DATE_FORMAT(endDateTime, "%Y-%m-%d %T") as endDateTime, mailTitle, mailText, isRead, mailIcon, productId FROM user_mail_private where userId = %s;', str(userId))
			
		return rows

def updateUserInfo(settings, datas):
	userInfoType = settings['userInfoType']

	with dbConnector(runMode=settings['runMode'], dbname='knightrun', auto_trans=True) as commander:
		userId = getUserId(commander, settings)

		for data in datas:
			if userInfoType == 'accounts':
				with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True) as commander:
					commander.execute("UPDATE accounts SET nickName = %s, limitLoginDate = %s, limitLoginReason =%s WHERE userId = %s;", str(data['nickName']), str(data['limitLoginDate']), str(data['limitLoginReason']), str(userId))

			elif userInfoType == 'users':
				commander.execute("UPDATE users SET gold = %s , gem = %s , gemPaid = %s, starCoin = %s, soStartTicket = %s, level = %s, exp = %s WHERE userId = %s",
					 str(data['gold']), str(data['gem']), str(data['gemPaid']), str(data['starCoin']), str(data['soStartTicket']), str(data['level']), str(data['exp']), str(userId))

			elif userInfoType == 'units':
				commander.execute("UPDATE units SET level = %s, count = %s, maxRankLevel =%s, currentRankLevel = %s, currentSeasonRankPoint = %s ,nextSeasonRankPoint = %s, currentVirtualRankLevel = %s, currentVirtualRankPoint = %s, skinId =%s WHERE userId = %s AND id = %s", 
					  str(data['level']), str(data['count']), str(data['maxRankLevel']), str(data['currentRankLevel']), str(data['currentSeasonRankPoint']), str(data['nextSeasonRankPoint']), str(data['currentVirtualRankLevel']), str(data['currentVirtualRankPoint']), str(data['skinId']), str(userId), str(data['id']))

			elif userInfoType == 'cards':
				commander.execute("UPDATE cards SET level = %s, count = %s WHERE userId = %s AND id = %s", str(data['level']), str(data['count']), str(userId), str(data['id']))

			elif userInfoType == 'armors':
				commander.execute("UPDATE armors SET level = %s, count = %s, unitId = %s WHERE userId = %s AND id = %s", str(data['level']), str(data['count']), str(data['unitId']), str(userId), str(data['id']))

			elif userInfoType == 'weapons':
				commander.execute("UPDATE weapons SET level = %s, count = %s WHERE userId = %s AND id = %s", str(data['level']), str(data['count']), str(userId), str(data['id']))

			elif userInfoType == 'stage_levels':
				commander.execute("UPDATE stage_levels SET clearCount = %s WHERE userId = %s AND stageLevelId = %s", str(data['clearCount']), str(userId), str(data['stageLevelId']))

			elif userInfoType == 'decks':
				query = "UPDATE decks SET classId = {0}, WHERE userId = {2} AND modeType = {3} AND deckNum = {4} AND slotType = {5} AND orderNum = {6}"\
							.format(data['classId'], userId, data['modeType'], data['deckNum'], data['slotType'], data['orderNum'])

				commander.execute(query)

			elif userInfoType == 'energy':
				 commander.execute("UPDATE infusion_boxes SET id = %s, boxEnergy = %s, userEnergy =%s, userBonusEnergy =%s WHERE userId = %s AND type =%s" , str(data['id']) , str(data['boxEnergy']) , str(data['userEnergy']), str(data['userBonusEnergy']),  str(userId), str(data['type']) )

			elif userInfoType =='square_object':
				commander.execute("UPDATE square_object_schedule SET isActivated = %s, activatedTime = %s, nextInvasionTime = %s, nextInvasionMonsterId = %s, nextInvasionLevel = %s, powerRefreshTime = %s, objectPower = %s, activeObjectLevel = %s, objectShield = %s, planetBoxExp = %s, planetBoxLevel = %s, coreEnergy = %s, energyRefreshTime = %s, objectExp = %s, objectLevel = %s, enableContents = %s, coreExp = %s, coreLevel = %s, agencyExp = %s, agencyLevel = %s WHERE userId = %s",
					 str(data['isActivated']), str(data['activatedTime']), str(data['nextInvasionTime']), str(data['nextInvasionMonsterId']), str(data['nextInvasionLevel']), str(data['powerRefreshTime']), str(data['objectPower']), str(data['activeObjectLevel']), str(data['objectShield']), str(data['planetBoxExp']), str(data['planetBoxLevel']), str(data['coreEnergy']), str(data['energyRefreshTime']), str(data['objectExp']), str(data['objectLevel']), str(data['enableContents']), str(data['coreExp']), str(data['coreLevel']), str(data['agencyExp']), str(data['agencyLevel']), str(userId))

			elif userInfoType == 'user_info':
				commander.execute("UPDATE user_info set isAlreadyFreeNicknameChange = %s, recentDateTimeCountryChange = %s, dailyRankVictoryGoldRewardCount = %s, dailyRankVictoryGoldRewardDateTime = %s, rewardedRankSeason = %s, unlockContents = %s,  maxVirtualRankLevel = %s, currentVirtualRankPoint = %s, maxVirtualRankPoint = %s WHERE userId = %s;",
					 str(data['isAlreadyFreeNicknameChange']), str(data['recentDateTimeCountryChange']), str(data['dailyRankVictoryGoldRewardCount']), str(data['dailyRankVictoryGoldRewardDateTime']), str(data['rewardedRankSeason']), str(data['unlockContents']), str(data['maxVirtualRankLevel']), str(data['currentVirtualRankPoint']), str(data['maxVirtualRankPoint']), str(userId))
			elif userInfoType == 'payment_info':
				pass

			elif userInfoType == 'payment_pending':
				with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True) as commander:
					rows = commander.execute('UPDATE pay_pending SET is_pending = %s WHERE userId = %s AND seq = %s AND transactionId = %s', str(data['is_pending']), str(userId), str(data['seq']), str(data['transactionId']) )

			elif userInfoType == 'privateMail':
				commander.execute('UPDATE user_mail_private SET isDeleted = %s, startDateTime = %s, endDateTime = %s, mailTitle = %s, mailText = %s, isRead = %s, mailIcon = %s, productId = %s where mailId = %s and userId = %s;',
					  str(data['isDeleted']), str(data['startDateTime']), str(data['endDateTime']), str(data['mailTitle']), str(data['mailText']), str(data['isRead']), str(data['mailIcon']), str(data['productId']),  str(data['mailId']), str(userId)  )



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
				with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True)  as command:
					command.execute("DELETE FROM accounts WHERE userId = %s", str(userId))
					command.execute("DELETE FROM clan_members WHERE userId = %s", str(userId))
					

			elif userInfoType == 'users':
				commander.execute("DELETE FROM users WHERE userId = %s;", str(userId))
				commander.execute("DELETE FROM units WHERE userId = %s;", str(userId))
				commander.execute("DELETE FROM cards WHERE userId = %s;", str(userId))
				commander.execute("DELETE FROM armors WHERE userId = %s;", str(userId))
				commander.execute("DELETE FROM weapons WHERE userId = %s;", str(userId))
				commander.execute("DELETE FROM stage_levels WHERE userId = %s;", str(userId))
				commander.execute("DELETE FROM decks WHERE userId = %s;", str(userId))
				commander.execute("DELETE FROM infusion_boxes WHERE userId = %s;", str(userId))
				commander.execute("DELETE FROM skins WHERE userId = %s;", str(userId))
				commander.execute("DELETE FROM square_object_schedule WHERE userId = %s", str(userId))


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

			elif userInfoType == 'give_skin':
				commander.execute("DELETE FROM skins WHERE userId = %s AND skinId = %s ", str(userId), str(data['skinId']))
			
			elif userInfoType == 'privateMail':
				commander.execute("DELETE FROM user_mail_private WHERE userId =%s AND mailId =%s;",
					 str(userId), str(data['mailId'])  )


def getUserId(commander, settings):
	identifierText = settings['identifierText']
	userInfoType = settings['userInfoType']
	with dbConnector(runMode=settings['runMode'], dbname='account') as command:
		if settings['identifierType'] == 'nickName':
			query = "SELECT userId FROM accounts WHERE nickName = %s;"

			rows = command.execute(query, str(identifierText))
			identifierText = rows[0]['userId']
		

	return identifierText

def getNickname(commander, settings):
	identifierText = settings['identifierText']
	userInfoType = settings['userInfoType']
	with dbConnector(runMode=settings['runMode'], dbname='account') as command:
		if settings['identifierType'] == 'userId':
			query = "SELECT nickname FROM accounts WHERE userId = %s;"

			rows = command.execute(query, str(identifierText))
			identifierText = rows[0]['userId']
		

	return identifierText

def insertItem(request):
	if request.method == 'GET':
		return render(
			request,
			'app/game/insertItem.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		data = response_data['updateMap']
		settings = response_data['settings']
		userInfoType = settings['userInfoType']

		with dbConnector(runMode=settings['runMode'], dbname='knightrun', auto_trans=True) as commander:
		##	userId = getUserId(commander, settings)
			
			if userInfoType == 'units':
				commander.execute("Insert into units(userId, id, level, count, currentSeasonRankPoint, maxRankLevel) (select userId , %s, %s, %s, %s, %s from accounts where userid not in (select userID from units where id = %s));", str(data['id']), str(data['level']), str(data['count']), str(data['rankPoint']), str(data['rankLevel']), str(data['id']) )

			elif userInfoType == 'cards':
				commander.execute("insert into cards ( userID, id, Level,count)  (select userID, %s, %s, %s from accounts  where userid not in (select userID from cards where id = %s));",  str(data['id']), str(data['level']), str(data['count']),  str(data['id']) )

			elif userInfoType == 'armors':
				commander.execute("insert into armors(userId, id, level, count) (select userID, %s, %s, %s from accounts  where userid not in (select userID from armors where id = %s));", str(data['id']), str(data['level']), str(data['count']),  str(data['id']))

			elif userInfoType == 'weapons':
				commander.execute("INSERT INTO weapons(userId, id, level, count) (select userID, %s, %s, %s from accounts  where userid not in (select userID from weapons where id = %s));", str(data['id']), str(data['level']), str(data['count']), str(data['id']))
						
		return HttpResponse({}, content_type='application/json; charset=utf-8')


def deleteItem(request):
	return render(
			request,
			'app/game/deleteItem.html'
		)
	if request.method == 'GET':
		return render(
			request,
			'app/game/deleteItem.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		data = response_data['updateMap']
		settings = response_data['settings']
		userInfoType = settings['userInfoType']

		with dbConnector(runMode=settings['runMode'], dbname='knightrun', auto_trans=True) as commander:
		##	userId = getUserId(commander, settings)
			
			if userInfoType == 'units':
				commander.execute("delete from units where id = %s;", str(data['id']))

			elif userInfoType == 'cards':
				commander.execute("delete from cards where id = %s;",  str(data['id']) )

			elif userInfoType == 'armors':
				commander.execute("delete from armors where id = %s;", str(data['id']))

			elif userInfoType == 'weapons':
				commander.execute("delete from weapons where id = %s;", str(data['id']))

			elif userInfoType == 'skin':
				commander.execute("delete from skins where skinId = %s;", str(data['id']))
						
		return HttpResponse({}, content_type='application/json; charset=utf-8')

def  truncateUser(request):
	return 
	if request.method == 'GET':
		return render(
			request,
			'app/game/truncateUser.html'
		)
	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		settings = response_data['settings']
		userInfoType = settings['userInfoType']
		args = (
					userInfoType,
				)

		with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True) as commander:
			commander.execute("truncate accounts;")
		with dbConnector(runMode=settings['runMode'], dbname="knightrun", auto_trans=True) as commander:
			commander.callproc('p_truncateAllUsers', args)

		return HttpResponse({}, content_type='application/json; charset=utf-8')	

def getfriendList(request):
	if request.method == 'GET':
		return render(
			request,
			'app/game/friendList.html'
		)
	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)
		settings = response_data['settings']

		rows = {}


		with dbConnector(runMode=settings['runMode'], dbname='account') as commander:
			if settings['identifierType'] == 'nickName':

				query = "SELECT userId FROM accounts WHERE nickName = %s;"
				rows = commander.execute(query, str(settings['identifierText']))
				userId = rows[0]['userId']
				with dbConnector(runMode=settings['runMode'], dbname='knightrun') as command:
					rows = command.execute("select friendId from friends where userId =%s;", str(userId))
			else:
				with dbConnector(runMode=settings['runMode'], dbname='knightrun') as command:
					rows = command.execute("select friendId from friends where userId =%s;", str(settings['identifierText']))

		return rows


def publicNotice(request):
	if request.method == 'GET':
		return render(
			request,
			'app/game/publicNotice.html'
		)
	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)
		settings = response_data['settings']
		runMode=settings['runMode']
		noticeMessage = response_data['noticeMessage']
		Host = g_commonconfig[runMode]['PUBSUB']['Redis']
		Port = g_commonconfig[runMode]['PUBSUB']['Port']
		Auth = g_commonconfig[runMode]['PUBSUB']['Auth']
		datas ={}

		r = redis.StrictRedis(host=Host, port=Port, password=Auth) 
		p = r.pubsub()

		r.publish('PublicNotice', noticeMessage) 

		return HttpResponse(datas, content_type='application/json; charset=utf-8')	

def messagePush(request):
	if request.method =='GET':
		return render(
			request,
			'app/game/messagePush.html'
			)
	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)
		settings = response_data['settings']
		title = response_data['title']
		message = response_data['message']
		runMode=settings['runMode']

		ids=[]


		msg = response_data['msg']
		if(msg =='push'):
			with dbConnector(runMode=settings['runMode'], dbname='account') as commander:
				query = "SELECT pushKeys from user_pushkeys"
				rows = commander.execute(query)
				rowLength = len(rows)
				count = 0
				for key in rows:
					ids.append(key['pushKeys'])
					count = count +1
					if ((count%1000) == 0 or count == rowLength):
						result = push_service.notify_multiple_devices(registration_ids = ids, message_title =title , message_body=message)

						pushCastId = result['multicast_ids'][0]
						successCount = result['success']
						failureCount = result['failure']
						with dbConnector(runMode=g_runMode, dbname='knightrun_gmtool', auto_trans=True) as command:
							result = command.execute("INSERT INTO push_log(pushCastId, successCount, failure, sendDateTime) VALUES(%s, %s, %s, now())", pushCastId, successCount, failureCount)
						ids = []
				return HttpResponse(status=200)


def inquiryEventInfo(request):
	if request.method == 'GET':
		return render(
			request,
			'app/game/inquiryEventInfo.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		settings = response_data['settings']
		
		datas = {}
		if msg == 'getEventInfo':
			datas = getEventInfo(settings)
			datas = json.dumps(datas, ensure_ascii=False, default=myconverter ).encode('utf-8')

		elif msg == 'updateEventInfo':
			updateList = response_data['updateList']
			updateEventInfo(settings, updateList)

		elif msg == 'deleteEventInfo':
			deleteList = response_data['deleteList']
			deleteEventInfo(settings, deleteList)

		elif msg == 'addEventInfo':
			data = response_data['updateMap']
			with dbConnector(runMode =settings['runMode'], dbname='account', auto_trans=True) as commander:
				commander.execute("INSERT INTO account._events(startDateTime, endDateTime, eventType) VALUES(%s, %s, %s);", str(data['startDateTime']), str(data['endDateTime']), str(data['eventType']))
				datas = getEventInfo(settings)
				datas = json.dumps(datas, ensure_ascii=False, default=myconverter ).encode('utf-8')

		return HttpResponse(datas, content_type='application/json; charset=utf-8')


def getEventInfo(settings):
	
	rows = []
	with dbConnector(runMode=settings['runMode'], dbname='account') as commander:
		#rows = commander.execute('SELECT eventId, startDateTime, endDateTime, eventType FROM _events WHERE endDateTime > now() ORDER BY eventId desc;')
		rows = commander.execute('SELECT eventId, startDateTime, endDateTime, eventType FROM _events  ORDER BY eventId asc;')
		
		return rows


def updateEventInfo(settings, datas):
	with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True) as commander:
		for data in datas:
			commander.execute("UPDATE _events set startDateTime = %s, endDateTime = %s, eventType = %s WHERE eventId = %s",
				str(data['startDateTime']), str(data['endDateTime']), str(data['eventType']),str(data['eventId']) )


def deleteEventInfo(settings, datas):
	with dbConnector(runMode =settings['runMode'], dbname='account', auto_trans=True) as commander:
		for data in datas:
			commander.execute("DELETE FROM _events WHERE eventId = %s", str(data['eventId']))



def serverInhouseChecker(request):
	if request.method == 'GET':
		return render(
			request,
			'app/game/serverInhouseChecker.html'
		)
	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)
		settings = response_data['settings']
		runMode=settings['runMode']
		msg = response_data['msg']
		settings = response_data['settings']

		Host = g_commonconfig[runMode]['PUBSUB']['Redis']
		Port = g_commonconfig[runMode]['PUBSUB']['Port']
		Auth = g_commonconfig[runMode]['PUBSUB']['Auth']
		redisClient = redis.StrictRedis(host=Host, port=Port, password=Auth) 
		if msg == 'setInhouse':
			redisClient.set('SIsServerDown', 1)
			redisClient.set('SDeveloperList', '["175.231.12.29","211.104.146.238"]')
			is_server = redisClient.get('SIsServerDown')
			iplist = redisClient.get('SDeveloperList')
			datas = [{'SIsServerDown' : json.dumps(is_server.decode()).replace("'", '"')[1:-1],
					'List' : json.dumps(iplist.decode()).replace("'", '"')[1:-1]}]
		elif msg == 'releaseInhouse':
			redisClient.set('SIsServerDown',0)
			redisClient.delete('SDeveloperList')
			is_server = redisClient.get('SIsServerDown')
			datas = [{'SIsServerDown' : json.dumps(is_server.decode()).replace("'", '"')[1:-1]}]

		return HttpResponse(datas, status=200)

def maintenanceTime(request):
	if request.method == 'GET':
		return render(
			request,
			'app/game/maintenanceTimeInsert.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		settings = response_data['settings']
		data = response_data['updateMap']
		datas = {}

		if msg == 'maintenanceAdd':
			with dbConnector(runMode =settings['runMode'], dbname='account', auto_trans=True) as commander:
				commander.execute("INSERT INTO _maintenance_time(startTime, endTime) VALUES(%s, %s);", str(data['startDateTime']), str(data['endDateTime']))
				return HttpResponse(data, status=200)
		elif msg == 'maintenanceList':
			with dbConnector(runMode=settings['runMode'], dbname='account', auto_trans=True) as commander:
				rows = commander.execute("SELECT seq, startTime, endTime FROM _maintenance_time")
				datas = json.dumps(rows, ensure_ascii=False ,default=myconverter).encode('utf-8')

		return HttpResponse(datas, content_type='application/json; charset=utf-8')

