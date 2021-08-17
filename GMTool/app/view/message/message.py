from django.shortcuts import render
from django.http import HttpResponse
from app.infra.db import *
from datetime import datetime, time
import json
import redis
from GMTool.settings import g_commonconfig, g_runMode, API_KEY, game_gamedbconfig
from pyfcm import FCMNotification

push_service = FCMNotification(api_key=API_KEY)


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
		

		r = redis.StrictRedis(host=Host, port=Port, password=Auth) 
		p = r.pubsub()

		r.publish('PublicNotice', noticeMessage) 

		return HttpResponse(status=200)

def messagePush(request):
	if request.method =='GET':
		return render(
			request,
			'app/game/messagepush.html'
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
			rows = getPushKeyList(runMode)
			rowLength = len(rows)
			count =0
			for key in rows:
				ids.append(key['pushKey'])
				count = count +1
				if ((count%1000) == 0 or count == rowLength):
					result = push_service.notify_multiple_devices(registration_ids = ids, message_title =title , message_body=message)

					pushCastId = result['multicast_ids'][0]
					successCount = result['success']
					failureCount = result['failure']
					with dbConnector(runMode=g_runMode, dbname='knightrun_gmtool', auto_trans=True) as command:
						result = command.execute("INSERT INTO push_log(pushCastId, reservationNum , title, message, successCount, failure, sendDateTime) VALUES(%s, %s, %s, %s, %s, %s, now())", pushCastId, 0, title, message, successCount, failureCount)
					ids = []
			return HttpResponse(status=200)


def inGameNoticeList(request):
	if request.method == 'GET':
		return render(
			request,
			'app/game/inGameNoticeList.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		settings = response_data['settings']
		datas = {}
		if msg == 'getNoticeInfo':
			datas = getNoticeInfo(settings)
			datas = json.dumps(datas, ensure_ascii=False).encode('utf-8')

		elif msg == 'updateNoticeInfo':
			updateList = response_data['updateList']
			updateNoticeInfo(settings, updateList)

		elif msg == 'deleteNoticeInfo':
			deleteList = response_data['deleteList']
			deleteUserInfo(settings, deleteList)

		return HttpResponse(datas, content_type='application/json; charset=utf-8')


def getNoticeInfo(settings):
	
	rows = []

	with dbConnector(runMode=settings['runMode'], dbname='knightrun_gmtool') as commander:
		rows = commander.execute('SELECT seq, noticeMessage, runMode, count, DATE_FORMAT(startTime, "%Y-%m-%d %T") as startTime FROM publicNotice')
	return rows

def updateNoticeInfo(settings, datas):
	with dbConnector(runMode=settings['runMode'], dbname='knightrun_gmtool', auto_trans=True) as commander:
		for data in datas:
			commander.execute("UPDATE publicnotice SET noticeMessage = %s, runMode = %s, count = %s, startTime =%s  WHERE seq = %s", 
					 str(data['noticeMessage']), str(data['runMode']), str(data['count']), str(data['startTime']), str(data['seq']))

def deleteUserInfo(settings, datas):
	with dbConnector(runMode=settings['runMode'], dbname='knightrun_gmtool', auto_trans=True) as commander:
		for data in datas:
			commander.execute("DELETE FROM publicnotice WHERE seq = %s", str(data['seq']))


def noticeReservation(request):
	if request.method == 'GET':
		return render(
			request,
			'app/message/noticeReservation.html'
		)
	else:
		datas = {}
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)
		settings = response_data['settings']
		runMode=settings['runMode']
		message = response_data['noticeMessage']
		count = response_data['count']
		startTime = response_data['startTime']
		
		with dbConnector(runMode=g_runMode , dbname='knightrun_gmtool', auto_trans=True) as commander:
			commander.execute('INSERT INTO publicNotice(noticeMessage, runMode, count, startTime) VALUES(%s,%s,%s,%s);', message, runMode, count, startTime)
		return HttpResponse(datas, content_type='application/json; charset=utf-8')


def pushReservation(request):
	if request.method == 'GET':
		return render(
			request,
			'app/message/pushReservation.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		settings = response_data['settings']

		datas = {}
		title = response_data['pushTitle'] 
		message = response_data['pushBody']
		cond = response_data['condition']
		reservationTime = response_data['reservationTime'] 
		#nightAgreeCheck = int(temp[-5:-3])
		
		
		with dbConnector(runMode=g_runMode, dbname='knightrun_gmtool', auto_trans=True) as commander:
			query = "INSERT INTO pushReservation(title, message, cond, runMode, reservationTime, sendlog) VALUES(%s, %s, %s, %s, %s, %s)"
			commander.execute(query, title, message, cond, str(settings['runMode']), reservationTime, 0)

		datas = json.dumps(datas, ensure_ascii=False).encode('utf-8')

		return HttpResponse(datas, content_type='application/json; charset=utf-8')



def getReservationPushList(request):
	if request.method == 'GET':
		return render(
			request,
			'app/message/getReservationPushList.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		settings = response_data['settings']
		datas = {}
		if msg == 'getReservationPushList':
			datas = getReservationPushInfo(settings)
			
			datas = json.dumps(datas, ensure_ascii=False).encode('utf-8')
			

		elif msg == 'deleteReservationPushList':
			deleteList = response_data['deleteList']
			deleteReservationPushList(settings, deleteList)

		return HttpResponse(datas, content_type='application/json; charset=utf-8')

def getReservationPushInfo(settings):
	with dbConnector(runMode=settings['runMode'], dbname='knightrun_gmtool') as commander:
		rows = commander.execute('SELECT seq, title, message, cond, DATE_FORMAT(reservationTime, "%Y-%m-%d %T") as reservation, sendLog from pushreservation order by seq desc Limit 100;')

	return rows


def deleteReservationPushList(settings, datas):
	with dbConnector(runMode=settings['runMode'], dbname='knightrun_gmtool', auto_trans=True) as commander:
		for data in datas:
			commander.execute("DELETE FROM pushreservation WHERE seq = %s", str(data['seq']))


def getPushKeyList(runMode):
	now = datetime.now()
	nowTime = now.time()
	nowDate = now.date()
	
	nums = game_gamedbconfig[runMode]['GameDBSetting']['UserDBSetting']
	keys = []

	for num in nums:
		with gdbConnector(runMode=runMode, dbname=num, auto_trans=True) as commander:
			if nowTime  >= time(11,30) or nowTime <= time(23,00):
				query = 'SELECT pushKey FROM pushkeys WHERE pushAgree = 1 and nightPushAgree = 1;'
				res = commander.execute(query)
				keys.extend(res)
			else:
				query = 'SELECT pushKey FROM pushkeys WHERE pushAgree = 1;'
				res = commander.execute(query)
				keys.extend(res)

	return keys

def insertMail(request):
	if request.method == 'GET':
		return render(
			request,
			'app/message/insertPublicMail.html'
		)

	else:
		response_data = request.body.decode('utf-8')
		response_data = json.loads(response_data)

		msg = response_data['msg']
		settings = response_data['settings']
		runMode = settings['runMode']

		if  msg == 'publicMail':
			startDateTime = response_data['startDateTime']
			endDateTime = response_data['endDateTime']
			mailIcon = response_data['mailIcon']
			productId = response_data['productId']
			kr_mailTitle = response_data['kr_mailTitle']
			kr_mailText = response_data['kr_mailText']
			en_mailTitle = response_data['en_mailTitle']
			en_mailText = response_data['en_mailText']
			jp_mailTitle = response_data['jp_mailTitle']
			jp_mailText = response_data['jp_mailText']


			with dbConnector(runMode=runMode, dbname='account', auto_trans=True) as commander:
				query = "INSERT INTO _mail_public(startDateTime, endDateTime, mailIcon, productId) VALUES(%s, %s, %s, %s)"
				commander.execute(query, str(startDateTime), str(endDateTime), mailIcon, productId)
				row = commander.execute('SELECT LAST_INSERT_ID();')
				mailId = row[0]['LAST_INSERT_ID()']
				print(mailId)
				commander.execute('INSERT INTO _mail_public_text(mailId, languageType, mailTitle, mailText) VALUES(%s, %s, %s, %s)',
					  mailId, 'kr', str(kr_mailTitle), str(kr_mailText))
				commander.execute('INSERT INTO _mail_public_text(mailId, languageType, mailTitle, mailText) VALUES(%s, %s, %s, %s)',
					  mailId, 'en',str(en_mailTitle), str(en_mailText))
				commander.execute('INSERT INTO _mail_public_text(mailId, languageType, mailTitle, mailText) VALUES(%s, %s, %s, %s)',
					  mailId, 'jp', str(jp_mailTitle), str(jp_mailText))

				Host = g_commonconfig[runMode]['PUBSUB']['Redis']
				Port = g_commonconfig[runMode]['PUBSUB']['Port']
				Auth = g_commonconfig[runMode]['PUBSUB']['Auth']
		
				r = redis.StrictRedis(host=Host, port=Port, password=Auth) 
				p = r.pubsub()

				r.publish('PublicNotice', 'AllMail') 


		#datas = json.dumps(mail, ensure_ascii=False).encode('utf-8')

		if msg == 'privateMail':
			startDateTime = response_data['startDateTime']
			endDateTime = response_data['endDateTime']
			mailIcon = response_data['mailIcon']
			productId = response_data['productId']
			mailTitle = response_data['mailTitle']
			mailText = response_data['mailText']

			userIds = response_data['userId'].split(',')


			with dbConnector(runMode=runMode, dbname='knightrun', auto_trans=True) as commander:
				for userId in userIds:
					
					if userId.strip() != '':
						query = "INSERT INTO user_mail_private(userId, startDateTime, endDateTime, mailTitle, mailText, mailIcon, productId) VALUES(%s, %s, %s, %s, %s, %s, %s)"
						commander.execute(query, userId, str(startDateTime), str(endDateTime), mailTitle, mailText, mailIcon, productId)

						Host = g_commonconfig[runMode]['PUBSUB']['Redis']
						Port = g_commonconfig[runMode]['PUBSUB']['Port']
						Auth = g_commonconfig[runMode]['PUBSUB']['Auth']
		
						r = redis.StrictRedis(host=Host, port=Port, password=Auth) 
						p = r.pubsub()

						r.publish('PrivateNotice|'+userId , 'Mail') 


		if msg == 'systemMail':
			startDateTime = response_data['startDateTime']
			endDateTime = response_data['endDateTime']
			userId = response_data['userId']
			systemMailId = response_data['systemMailId']
			with dbConnector(runMode=runMode, dbname='knightrun', auto_trans=True) as commander:
				query = "INSERT INTO user_mail_system(userId, mailId, isDeleted, startDateTime, endDateTime, isRead) VALUES(%s, %s, %s, %s, %s, %s)"
				commander.execute(query, userId, systemMailId, 0, str(startDateTime), str(endDateTime), 0)

		if msg == 'noticeMail':

			userIds = response_data['userId'].split(',')

			Host = g_commonconfig[runMode]['PUBSUB']['Redis']
			Port = g_commonconfig[runMode]['PUBSUB']['Port']
			Auth = g_commonconfig[runMode]['PUBSUB']['Auth']

			r = redis.StrictRedis(host=Host, port=Port, password=Auth) 
			p = r.pubsub()

			for userId in userIds:
			
				r.publish('PrivateNotice|'+userId , 'Mail') 

		#datas = json.dumps(datas, ensure_ascii=False).encode('utf-8')
		datas = {}

		return HttpResponse(datas, content_type='application/json; charset=utf-8')

