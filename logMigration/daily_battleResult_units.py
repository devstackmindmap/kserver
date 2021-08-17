#당일 상점 구매 정보 로그
#login_log (userId, cnt, date)
#crontab 에 스케쥴로 돌리기.

from pymongo import MongoClient
from bson.son import SON
import re
import mysql.connector, pytz
from  db import *
import datetime as datetime
import json



def main():
	try: 
		with open('/KServer/logMigration/config.json') as json_file:
		#with open('./config_local.json') as json_file:
			jsondata = json.load(json_file)

		
		client = MongoClient(
			host =jsondata['host'],
			port = jsondata['port'],
			#host = '47.75.102.249',
			#port = 27017,
			# replica=replica set
            # username=user
            # password=password
            # authSource=auth database
			)

		db = client[jsondata['database']]
				
		print('MongoDB connected')


		date = datetime.date.today() - datetime.timedelta(days=1) 

				
		
		pipeline = { 'message': 'BattleEndResult','timestamp':  re.compile(r'^'+str(date)) }
		
		results = db.BattleStatus.find(pipeline)
		
		unitsplit(results, date)				
		

	except Exception as e:
		print(tracebak.format_exc())
	finally:
		client.close()
		print('MongoDB Closed')


def unitsplit(units, date):
	
	winUnitLevels =list()
	wincards = list()
	
	loseUnitLevels = list()
	losecards = list()

	winDeckUnits = list()
	winDeckCards = list()

	loseDeckUnits = list()
	loseDeckCards = list()

	for unit in units:
		winner = unit['fields']['Winner']
		BattleType = unit['fields']['BattleType']
		Player1Units = unit['fields']['Player1Units']
		Player2Units = unit['fields']['Player2Units']
		Player1Cards = unit['fields']['Player1Cards']

		if 'Player2Cards' in unit['fields']:
			Player2Cards = unit['fields']['Player2Cards']
		else:
			Player2Cards = 'none'

		Player1UnitLevels = unit['fields']['Player1UnitLevels']

		if 'Player2UnitLevels' in unit['fields']:
			Player2UnitLevels = unit['fields']['Player2UnitLevels']
		else:
			Player2UnitLevels = 'none'


		winnerUnits = list()
		winnerUnitLevels =list()
		winnerCards = list()
		
		loseUnits = list()
		loseunitLevels =list()
		loseCards = list()

		windeckunit = list()
		windeckcard = list()
		losedeckunit = list()
		losedeckcard = list()
		
		if BattleType == '1':
			if winner == 'Player1':
				winnerUnits = Player1Units.split(',')
				winnerUnitLevels = Player1UnitLevels.split(',')
				winnerCards = Player1Cards.split(',')
				
				loseUnits = Player2Units.split(',')	
				loseunitLevels = Player2UnitLevels.split(',')
				loseCards = Player2Cards.split(',')

				windeckunit = Player1Units
				winnerCards.sort()
				windeckcard = ','.join(winnerCards)

				losedeckunit = Player2Units
				loseCards.sort()
				losedeckcard = ','.join(loseCards)


				
				windeck = {windeckunit : windeckcard}
				winDeckUnits.append(windeck)

				losedeck = {losedeckunit : losedeckcard}
				loseDeckUnits.append(losedeck)

				i =  0
				for wunit in winnerUnits:
					wu = { wunit : winnerUnitLevels[i]}
					winUnitLevels.append(wu)
					i= i+1

				for card in winnerCards:
					wincards.append(card)

				i =  0
				for lunit in loseUnits:
					lu = { lunit : loseunitLevels[i]}
					loseUnitLevels.append(lu)
					i= i+1
				for card in loseCards:
					losecards.append(card)


			elif winner == 'Player2':
				
				if Player2UnitLevels != 'none':
					winnerUnits = Player2Units.split(',')
					winnerUnitLevels = Player2UnitLevels.split(',')
					winnerCards = Player2Cards.split(',')

					loseUnits = Player1Units.split(',')	
					loseunitLevels = Player1UnitLevels.split(',')
					loseCards = Player1Cards.split(',')

					windeckunit = Player1Units
					if winnerCards == 'None':
						windeckcard = winnerCards
					else:
						winnerCards.sort()
						windeckcard = ",".join(winnerCards)
					
					losedeckunit = Player2Units
					if winnerCards == 'None':
						losedeckcard = loseCards
					else:
						loseCards.sort()
						losedeckcard = ",".join(loseCards)

					windeck = {windeckunit : windeckcard}
					winDeckUnits.append(windeck)

					losedeck = {losedeckunit : losedeckcard}
					loseDeckUnits.append(losedeck)

					i =  0
					for wunit in winnerUnits:
						wu = {wunit : winnerUnitLevels[i] }
						winUnitLevels.append(wu)
						i= i+1
					for card in winnerCards:
						wincards.append(card)
					
					i =  0
					for lunit in loseUnits:
						lu = { lunit : loseunitLevels[i]}
						loseUnitLevels.append(lu)
						i= i+1
					for card in loseCards:
						losecards.append(card)

	
	with dbConnector(dbname='KPI', auto_trans=True) as commander:
		for winunit in winUnitLevels:
			pk = list(winunit.keys())
			pv = list(winunit.values())
			
			#commander.execute('insert into win_unit_count(unitid, level, count, Date) values(%s, %s, %s, %s)  on duplicate key update count = count + 1;',int(pk[0]), int(pv[0]), 1, '2020-04-01')
			commander.execute('insert into winlose_unit_count(unitid, level, wincount, Date) values(%s, %s, %s, %s)  on duplicate key update wincount = wincount + 1;',int(pk[0]), int(pv[0]), 1, date)
		for wincard in wincards:
			#commander.execute('insert into win_card_count(cardId, count, Date) values(%s, %s, %s) on duplicate key update count = count +1;',wincard, 1, '2020-04-01' )
			commander.execute('insert into winlose_card_count(cardId, wincount, Date) values(%s, %s, %s) on duplicate key update wincount = wincount +1;',wincard, 1, date )
		
		for loseunit in loseUnitLevels:
			pk = list(loseunit.keys())
			pv = list(loseunit.values())
			
			#commander.execute('insert into lose_unit_count(unitid, level, count, Date) values(%s, %s, %s, %s)  on duplicate key update count = count + 1;',int(pk[0]), int(pv[0]), 1, '2020-04-01')
			commander.execute('insert into winlose_unit_count(unitid, level, losecount, Date) values(%s, %s, %s, %s)  on duplicate key update losecount = losecount + 1;',int(pk[0]), int(pv[0]), 1, date)
		for losecard in losecards:
			#commander.execute('insert into lose_card_count(cardId, count, Date) values(%s, %s, %s) on duplicate key update count = count +1;',losecard, 1, '2020-04-01' )
			commander.execute('insert into winlose_card_count(cardId, losecount, Date) values(%s, %s, %s) on duplicate key update losecount = losecount +1;',losecard, 1, date )

		for winDeck in winDeckUnits:
			for key, card in winDeck.items():
				commander.execute('insert into deckset(units, cards, wincount, date) values(%s, %s, %s, %s) on duplicate key update wincount = wincount +1;',key, card, 1, date )

		for loseDeck in loseDeckUnits:
			for key, card in loseDeck.items():
				commander.execute('insert into deckset(units, cards, losecount, date) values(%s, %s, %s, %s) on duplicate key update losecount = losecount +1;',key, card, 1, date )




if __name__ == "__main__":
	main()


