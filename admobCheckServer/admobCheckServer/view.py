from django.http import HttpResponse

from admobCheckServer.infra.db import *
import json
from admobCheckServer.settings import g_commonconfig, g_runMode
import hashlib
import base64
import ecdsa
from ecdsa import SigningKey, SECP256k1
import socket

from django.dispatch import receiver

from django_admob_ssv.signals import valid_admob_ssv

@receiver(valid_admob_ssv)
def reward_user(sender, query, **kwargs):
	ad_network = query.get('ad_network')
	ad_unit = query.get('ad_unit')
	custom_data = query.get('custom_data')
	reward_amount = query.get('reward_amount')
	reward_item = query.get('reward_item')
	timestamp = query.get('timestamp')
	transaction_id = query.get('transaction_id')
	user_id = query.get('user_id')
	signature = query.get('signature')
	key_id = query.get('key_id')

	if reward_amount == None:
		reward_amount = 0
	if user_id == None:
		user_id = 0
	if reward_item == None:
		reward_item =''


	with dbConnector(runMode='Milkman', dbname='account', auto_trans=True) as commander:
		commander.execute("INSERT INTO ad_view_log(ad_network, ad_unit, reward_amount, reward_item, timestamp, transaction_id, user_id, signature, key_id) values(%s,%s,%s,%s,%s,%s,%s,%s,%s);", 
				str(ad_network), str(ad_unit), str(reward_amount), str(reward_item), str(timestamp), str(transaction_id), str(user_id), str(signature), str(key_id) )





def index(request):
	return HttpResponse("여긴 아니지")

def checkHost(ip):
	hostname = socket.gethostbyaddr(ip)
	hostname = hostname[0]
	hostname = hostname.split('.')
	
	if hostname[2] == 'com' and hostname[1] == 'google':
		return 'google'
	elif hostname[2] == 'com' and hostname[1] == 'qooapp':
		return 'qooapp'
	else:
		return 'unknow'


def admob(request):
	#try:
		if request.method == 'GET':
			
			
			ad_network = request.GET.get('ad_network')
			ad_unit = request.GET.get('ad_unit')
			reward_amount = request.GET.get('reward_amount')
			reward_item = request.GET.get('reward_item')
			timestamp = request.GET.get('timestamp')
			transaction_id = request.GET.get('transaction_id')
			user_id = request.GET.get('user_id')
			signature = request.GET.get('signature')
			key_id = request.GET.get('key_id')
			
			if reward_amount == None:
				reward_amount = 0
			if user_id == None:
				user_id = 0
			if reward_item == None:
				reward_item =''

			resdata = {
				"ad_network" : ad_network,
				"ad_unit" : ad_unit,
				"reward_amount" : reward_amount,
				"reward_item" : reward_item,
				"timestamp" : timestamp,
				"transaction_id" : transaction_id,
				"user_id" : user_id,
				"signature" : signature,
				"key_id" : key_id
			}
			


			#보내 온곳이 구글인지 확인 하기 위한 지점.
			remote_ip = request.META.get('HTTP_X_FORWARDED_FOR', request.META.get('REMOTE_ADDR', '')).split(',')[0].strip()
			if checkHost(remote_ip) != 'google':
				return HttpResponse('sorry!', content_type='application/json; charset=utf-8')

			with open('./admobkeys.json') as json_keys:
				pubkeys = json.load(json_keys)
		
			pem = 0
		
			for pubkey in pubkeys['keys']:
			
				if str(resdata['key_id']) == str(pubkey['keyId']):
					pem = pubkey['pem']
					key64 = pubkey['base64']

			if pem != 0:
				datas = json.dumps(resdata, ensure_ascii=False).encode('utf-8')
				with dbConnector(runMode='Milkman', dbname='account', auto_trans=True) as commander:
					commander.execute("INSERT INTO ad_view_log(ad_network, ad_unit, reward_amount, reward_item, timestamp, transaction_id, user_id, signature, key_id) values(%s,%s,%s,%s,%s,%s,%s,%s,%s);", 
							str(ad_network), str(ad_unit), str(reward_amount), str(reward_item), str(timestamp), str(transaction_id), str(user_id), str(signature), str(key_id) )
				return HttpResponse(datas, content_type='application/json; charset=utf-8')
			else:

				datas = json.dumps(resdata, ensure_ascii=False).encode('utf-8')
				return HttpResponse(datas, content_type='application/json; charset=utf-8')
		else:
			response_data = request.body.decode('utf-8')
			response_data = json.loads(response_data)

			return HttpResponse('', content_type='application/json; charset=utf-8')
	#except :
	#	return  HttpResponse('', content_type='application/json; charset=utf-8')


def qooappcall(request):
	
	response_data = request.body.decode('utf-8')
	
	response_data = json.loads(response_data)

	
	print(request.META)
	print(response_data)
	payload = response_data['payload']
	
	purchase_id = payload['purchase_id']
	token = payload['token']
	user_id = payload['user_id']
	product_id = payload['product_id']
	product_type = payload['product_type']
	amount = payload['amount']
	currency = payload['currency']
	cpOderId = payload['cpOderId']
	developerPayload = payload['developerPayload']

	# qooapp의 경우는 callback host에 대해서 정상적으로 오는지 확인이 필요
	remote_ip = request.META.get('HTTP_X_FORWARDED_FOR', request.META.get('REMOTE_ADDR', '')).split(',')[0].strip()
	#if checkHost(remote_ip) != 'qooapp':
	#	print('실서비스 확인')
	#	return HttpResponse('faker!', content_type='application/json; charset=utf-8')

	
	with dbConnector(runMode='Milkman', dbname='account', auto_trans=True) as commander:
		commander.execute("INSERT INTO pay_qooapp(purchase_id,	token, user_id, product_id, product_type, amount, currency, cpOderId, developerPayload) values(%s,%s,%s,%s,%s,%s,%s,%s,%s);", 
				str(purchase_id), str(token), str(user_id), str(product_id), str(product_type), str(amount), str(currency), str(cpOderId), str(developerPayload) )


	pay = json.dumps(payload)
	signature = json.dumps(response_data['signature'])
	
	h = hashlib.sha1()
	h.update(pay.encode())
	print(h.hexdigest())
	#print(h.digest())
	

	pay2 = pay.encode("UTF-8")
	encoded = base64.b64encode(pay2)
	print(encoded.decode("UTF-8"))

	decoded = base64.b64decode(signature)
	

	return HttpResponse(encoded, content_type='application/json; charset=utf-8')


