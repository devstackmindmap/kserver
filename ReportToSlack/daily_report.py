#상품 판매 로그 정보 데이터
#
#

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
		# oridate = datetime.datetime.strptime(ridate,"%Y-%m-%d").date()
		# lastdate = oridate + datetime.timedelta(days=-1)
		
		fs = open('c:\\project\\KServer\\ReportToSlack\\sales_report.csv', 'w', encoding='utf-8',newline='')
		wr = csv.writer(fs)
		with dbConnector( auto_trans=True) as commander:
			rows = commander.execute("select ta.date, ta.storeProductId, ta.sellCount, ta.salesTotalCost as salesTotalCostA, ta.userCount, ta.saleCost, tb.salesTotalCost as salesTotalCostB, tb.totalsellcount, tc.dau  , (tb.salesTotalCost / tc.dau) as ARPU,  tb.dailyPu, (tb.salesTotalCost /tb.dailyPu )  as ARPPU from (select date_format(a.adddatetime,'%Y-%m-%d') as date, a.storeProductId, count(a.storeProductId) as sellCount, sum(b.saleCost) as salesTotalCost , count(distinct userId) as userCount, saleCost from account.purchased as a  left join (select storeProductId , saleCost from account._products_event_real union select storeProductId , cost as saleCost from account._products_fix_real union select storeProductId , saleCost from account._products_user_real) as b on a.storeProductId = b.storeProductId group by date, a.storeProductId ) as ta left join (select date_format(a.adddatetime,'%Y-%m-%d') as date, sum(b.saleCost) as salesTotalCost , count(a.storeProductId) as totalsellcount, count(distinct userId) as dailyPU from account.purchased as a  left join (select storeProductId , saleCost from account._products_event_real union select storeProductId , cost as saleCost from account._products_fix_real union select storeProductId , saleCost from account._products_user_real) as b on a.storeProductId = b.storeProductId group by date ) as tb  on ta.date = tb.date left join (select date_format(date, '%Y-%m-%d')as datec, count(userId) as dau from kpi.login_log group by datec) as tc on tb.date = tc.datec order by date DESC;")
			wr.writerow(['date','storeProductId','상품금액','판매수','구입유저수','상품 판매금액' ])
			for row in rows:
				wr.writerow([row['date'] , row['storeProductId'] , row['saleCost'], row['sellCount'] , row['userCount'], row['salesTotalCostA']    ])

		ft = open('c:\\project\\KServer\\ReportToSlack\\dau_sales_report.csv', 'w', encoding='utf-8',newline='')
		wr = csv.writer(ft)
		with dbConnector( auto_trans=True) as commander:
			rows = commander.execute("select ta.date, tb.dau, ta.sellCount,  ta.dailyPu, ta.salesTotalCost, (ta.salesTotalCost / tb.dau) as ARPU,  (ta.salesTotalCost /ta.dailyPu )  as ARPPU   from (select date_format(addDateTime, '%Y-%m-%d') as date, count(a.storeProductId) as sellCount, sum(b.saleCost) as salesTotalCost , count(distinct a.userId) as dailyPu from account.purchased as a left join (select storeProductId, salecost from account._products_event_real union select storeProductId , cost as saleCost from account._products_fix_real union select storeProductId , saleCost from account._products_user_real) as b on a.storeProductId = b.storeProductId group by date) ta join (select date_format(date, '%Y-%m-%d')as date, count(userId) as dau from kpi.login_log group by date) as tb on ta.date = tb.date order by ta.date desc;")
			wr.writerow(['date','dau','sellCount','DailyPU','salesTotalCost','ARPU','ARPPU' ])
			for row in rows:
				wr.writerow([ row['date'] , row['dau'] , row['sellCount'],row['dailyPu'], row['salesTotalCost'] , row['ARPU'], row['ARPPU']  ])


	except Exception as e:
		print(tracebak.format_exc())
	finally:
		
		fs.close()
		print('end')

if __name__ == "__main__":
	main()
