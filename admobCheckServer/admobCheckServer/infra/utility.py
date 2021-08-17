import json, socket, time, datetime
from GMTool.settings import g_runModeList

class DateTimeEncoder(json.JSONEncoder):
    def default(self, o):
        if isinstance(o, datetime.datetime):
            return o.strftime('%Y-%m-%d %H:%M:%S')

        return json.JSONEncoder.default(self, o)

def convertToBytes(str_datas):
	result = ''.join(r'\x{0:x}'.format(ord(c)) for c in str_datas)

	return result.encode('utf-8')

def sendJsonDataBySocket(connectionString, netCommandType, json_data):
	splitedConnectionString = connectionString.split(':')
	ip = splitedConnectionString[0]
	port = int(splitedConnectionString[1])

	data = {
		'MsgName': netCommandType,
		'Data': json_data
	}

	data = json.dumps(data)
	with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
		sock.connect((ip, port))
		sock.settimeout(5.0)
		head = [1]  #NetDataType(Json)
		body = convertToBytes(data)			
		bytedatas = bytearray(head) + body
		sock.sendall(bytedatas)
		res = sock.recv(512)
		result = json.loads(res)

		return result

def getTimestamp():
	ts = time.time()
	timestamp = datetime.datetime.fromtimestamp(ts).strftime('%Y-%m-%d %H:%M:%S')

def getLocalTime():
	return time.strftime('%Y-%m-%d %H:%M:%S', time.localtime())	


def getHtmlOfButtonGroup(selectedRunMode):
	html = ''

	for runMode in g_runModeList:
		html += '<button type="submit" class="btn btn-{0} btn-lg" name="runMode" value="{1}">{2}</button>\n'.format(('primary' if runMode == selectedRunMode else 'default'), runMode, runMode)

	return html