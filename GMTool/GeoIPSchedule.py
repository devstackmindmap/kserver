from app.infra.db import *
from datetime import datetime, time
import json
from GMTool.settings import g_commonconfig, g_runMode, API_KEY
import wget, os, sys
import zipfile
import csv
from sqlalchemy import create_engine, types
import pandas as pd
import argparse



print(g_commonconfig['Milkman'])

parser = argparse.ArgumentParser(description ='downloadurl')
parser.add_argument('--url', required=True, help='URL = https://download.ip2location.com/lite/ filename')
parser.add_argument('--filename', required=True, help='unzip filename')
parser.add_argument('--table', required=True, help='tableName ex) _ip2location')
parser.add_argument('--runMode', required=True, help='config RunMode ex) Dev1')

args = parser.parse_args()


groupCode = {"KR" : 1, "CN" : 1, "JP":1 }

URL = 'https://download.ip2location.com/lite/'
ipv4 = 'IP2LOCATION-LITE-DB1.CSV.ZIP'
ipv6 = 'IP2LOCATION-LITE-DB1.IPV6.CSV.ZIP'
ipv4name = 'IP2LOCATION-LITE-DB1.CSV'
ipv6name = 'IP2LOCATION-LITE-DB1.IPV6.CSV'


def fileDownload(url, unzipfilename):
	filename = wget.download(url)
	zip = zipfile.ZipFile(filename)
	zip.extract(unzipfilename)
	rows = fileRead(unzipfilename)
	zip.close()
	os.remove(filename)
	return rows


def fileRead (filename):
	path = os.getcwd()
	if os.path.isfile(path+"/"+filename):
		f = open(path+"/"+filename, 'r' , encoding = 'utf-8')
		line = csv.reader(f)
		#f.close()
		return line

def fileClean(name):
	if os.path.isfile(name):
		os.remove(name)


def addGroupCode(rows):
	data = []
	column =["ip_from", "ip_to", "country_code", "country_name", "group_code"]
	data.append(column)
	for row in rows:
		if row[2] == "KR" or row[2] == "CN" or row[2] == "JP":
			row.append(groupCode[row[2]])
			data.append(row)
		else:
			row.append(1)
			data.append(row)
		
	return data

def csvWrite(data):
	
	with open(os.getcwd()+'/rowdata.csv', 'w', newline='') as file:
		writer = csv.writer(file,quoting=csv.QUOTE_NONNUMERIC )
		writer.writerows(data)
		file.close()
	return 


def dataInsert(tablename, runMode, dbUser, dbPassword, dbHost, dbPort):
	
	query = 'TRUNCATE TABLE %s;' % tablename
	with dbConnector(runMode = g_runMode,  dbname='account', auto_trans = True) as commander:
		commander.execute(query)
	
	uri = 'mysql://%s:%s@%s:%s/account' % (dbUser, dbPassword, dbHost, dbPort)
	engine = create_engine(uri)
	df = pd.read_csv(os.getcwd()+'/rowdata.csv',sep=',',encoding='utf8',low_memory=False )
	df.to_sql(tablename,uri,if_exists='append', index=False)

runMode = args.runMode	
dbUser = g_commonconfig[runMode]['DBSetting']['account']['user']
dbPassword = g_commonconfig[runMode]['DBSetting']['account']['password']
dbHost = g_commonconfig[runMode]['DBSetting']['account']['host']
dbPort = g_commonconfig[runMode]['DBSetting']['account']['port']

rows = fileDownload(args.url, args.filename)
data = addGroupCode(rows)
csvWrite(data)
dataInsert(args.table, runMode, dbUser, dbPassword, dbHost, dbPort)

