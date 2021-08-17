import mysql.connector, pytz
from admobCheckServer.settings import g_commonconfig, g_runMode, game_gamedbconfig

class dbConnector(object):
	def __init__(self, runMode=g_runMode, dbname='knightrun_gmtool', auto_trans=False):
		self.auto_trans = auto_trans

		
		self.conn = mysql.connector.connect(**g_commonconfig[runMode]['DBSetting'][dbname])
		
	def __enter__(self):
		self.conn_cursor = self.conn.cursor(dictionary=True)
		self.dbCommander = dbCommander(self.conn_cursor, self.conn)

		if self.auto_trans:
			self.dbCommander.start_transaction()
		
		return self.dbCommander

	def __exit__(self, type, value, traceback):
		if self.auto_trans:
			if type is not None:
				self.dbCommander.rollback()
			else:
				self.dbCommander.commit()

		self.conn_cursor.close()
		self.conn.close()

class dbCommander(object):
	def __init__(self, cursor, conn):
		self.cursor = cursor
		self.conn = conn

	def start_transaction(self):
		self.conn.start_transaction(False, 'REPEATABLE READ')

	def commit(self):
		self.conn.commit()

	def rollback(self):
		self.conn.rollback()

	def execute(self, query, *args):
		self.cursor.execute(query, args)
		if self.cursor.with_rows:
			return self.cursor.fetchall()
		else:
			return None
		
	def getResultArg(self, index):
		items = list(self.result_args.values())

		return items[index]

	def getLastRows(self):
		try:
			rows_len = len(self.stored_data)
			rows = self.stored_data[rows_len - 1]

			return rows

		except:
			return []

	def getLastRow(self):
		try:
			rows_len = len(self.stored_data)

			rows = self.stored_data[rows_len - 1]

			return rows[0]

		except:
			return {}

	def callproc(self, proc_name, args):
		self.result_args = self.cursor.callproc(proc_name, args)

		self.stored_data = []
		for it in self.cursor.stored_results():
			datalist = []
			for row in it:
				column_count = 0

				data = {}
				for column_name in it.column_names:
					data[column_name] = row[column_count]

					column_count += 1
				datalist.append(data)

			self.stored_data.append(datalist)   

		return self.result_args

class gdbConnector(object):
	def __init__(self, runMode=g_runMode, dbname='knightrun_gmtool', auto_trans=False):
		self.auto_trans = auto_trans

		
		self.conn = mysql.connector.connect(**game_gamedbconfig[runMode]['GameDBSetting']['UserDBSetting']['{0}'.format(dbname)])
		
	def __enter__(self):
		self.conn_cursor = self.conn.cursor(dictionary=True)
		self.dbCommander = dbCommander(self.conn_cursor, self.conn)

		if self.auto_trans:
			self.dbCommander.start_transaction()
		
		return self.dbCommander

	def __exit__(self, type, value, traceback):
		if self.auto_trans:
			if type is not None:
				self.dbCommander.rollback()
			else:
				self.dbCommander.commit()

		self.conn_cursor.close()
		self.conn.close()

