#리텐션 정보
#
#

from bson.son import SON
import re
import mysql.connector, pytz
from  db import *
import datetime as datetime
import json
import csv
import traceback

def main():
	try: 
		rows = list()
		ridate = '2020-03-31'
		oridate = datetime.datetime.strptime(ridate,"%Y-%m-%d").date()

		td = datetime.date.today() - datetime.datetime.strptime(ridate,"%Y-%m-%d").date()
		rangedate = int(td.days)
		dates = list()
		fs = open('c:\\project\\KServer\\ReportToSlack\\retension_count.csv', 'w', encoding='utf-8',newline='')
		wrt = csv.writer(fs)
		with dbConnector( auto_trans=True) as commander:
			for i in range(rangedate-1,-1,-1):
				ordate = oridate + datetime.timedelta(days=i)
				dates.append(ordate.strftime("%Y-%m-%d"))
				# print(dates)
				daterow = list()
				percent = list()
				per = list()
				for date in dates:
					if ordate.strftime("%Y-%m-%d") <= date:
						row = commander.execute('select count(userId) as count from account.accounts where joinDateTime like %s and userId in (select userId from kpi.login_log where date = %s);', ordate.strftime("%Y-%m-%d")+'%',date)
						tmp = row[0]['count']
						# tmpp = {ordate.strftime("%Y-%m-%d") : tmp}
						daterow.append(tmp)
				daterow.reverse()
				dte  = [date, daterow]
				rows.append(dte)
				# print(dte)
			for row in rows:
				firstDay = row[1][0]
				percent = list()
				day = row[0]
				count = list()
				for ro in row[1]:
					count.append(str(ro))
				st = ','.join(count)
				rrow = (row[0] ,  st)
				wrt.writerow(rrow)

				for ro in row[1]:
					if ro != 0:
						calc = (ro / firstDay) * 100
						percent.append("{0:.2f}%".format(calc))
					else:
						percent.append("0%")
				perrow = [row[0],percent]
				per.append(perrow)

			f = open('c:\\project\\KServer\\ReportToSlack\\retension.csv', 'w', encoding='utf-8',newline='')
			wr = csv.writer(f)
			for row in per:
				ros = list()
				for ro in row[1]:
					ros.append(ro)
				st = ','.join(ros)
				rrow = (row[0] ,  st)
				wr.writerow(rrow)
			
	

	except Exception as e:
		print(tracebak.format_exc())
	finally:
		f.close()
		fs.close()
		print('end')

if __name__ == "__main__":
	main()
