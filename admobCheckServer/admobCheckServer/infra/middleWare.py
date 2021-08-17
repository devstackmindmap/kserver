from GMTool.settings import SESSION_EXPIRY_SEC

class sessionValidator(object):
	def process_request(obj, request):
		if 'isAuthenticated' in request.session:
			request.user.isAuthenticated = request.session['isAuthenticated']
			if request.user.isAuthenticated == False:
				request.path_info = '/login/'
				del request.session['isAuthenticated']

				return None
			
		if not 'accountInfo' in request.session:
			if not request.path_info in ('/login/', '/account/validate/', '/logout/'):
				request.path_info = '/login/'

		else:
			request.user.username = request.session['accountInfo']['username']
			request.user.setRunmode = request.session['accountInfo']['setRunmode']
			request.session.set_expiry(SESSION_EXPIRY_SEC)

		return None;



