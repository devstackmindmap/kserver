"""
Definition of views.
"""

from django.shortcuts import render, redirect
from django.http import HttpRequest
from django.template import RequestContext
from datetime import datetime
from GMTool.settings import SESSION_EXPIRY_SEC, g_runMode
from app.infra.db import *
from django import forms
import hashlib

def home(request):
	if request.user.username != '':
		return render(
			request,
			'app/index.html',
			{
				'year':datetime.now().year
			}
		)
	
	else:
		return redirect('login')

def validateAccount(request):
	username = request.POST['username']
	password = request.POST['password']
	
	with dbConnector() as commander:
		query = "select PASSWORD('{0}') as inputPW".format(password)

		row = commander.execute(query)
		passwd = row[0]['inputPW']

	isAuthenticated = False
	with dbConnector() as commander:
		query = "select password , setRunmode from account where username = '{0}'".format(username)

		rows = commander.execute(query)
		if rows:
			db_password = rows[0]['password']
			setRunmode = rows[0]['setRunmode']			
			if passwd == db_password:
				isAuthenticated = True
				
	if isAuthenticated:
		request.session['accountInfo'] = {
			'username': username,
			'setRunmode' : setRunmode
		}

	request.session['isAuthenticated'] = isAuthenticated
	request.session.set_expiry(SESSION_EXPIRY_SEC)

	return redirect('/')

def logout(request):
	request.user.username = ''
	request.user.isAuthenticated = False

	if 'accountInfo' in request.session:
		del request.session['accountInfo']

	if 'isAuthenticated' in request.session:
		del request.session['isAuthenticated']

	return redirect('/')


