"""
Definition of urls for GMTool.
"""

from datetime import datetime
from django.conf.urls import url

import django.contrib.auth.views
import app.forms
import app.view.game.userInfo
import app.view.message.message
import app.view.game.alluserInfo
import app.view.table.file
import app.view.index
import app.view.assetBundle.file
#import app.view.log.log
import app.view.shopManage.shop
import app.view.game.clanInfo

#import app.view.getResource


# Uncomment the next lines to enable the admin:
# from django.conf.urls import include
# from django.contrib import admin
# admin.autodiscover()

urlpatterns = [
    # Examples:
	url(r'^$', app.view.index.home, name='home'),
	url(r'^assetBundle/inquiry/$', app.view.assetBundle.file.inquiryAssetBundle, name = 'inquiryAssetBundle'),
	url(r'^assetBundle/uploadMulti/$', app.view.assetBundle.file.uploadMultiAssetBundle, name = 'uploadMultiAssetBundle'),
	url(r'^assetBundle/updateVersion/$', app.view.assetBundle.file.updateAssetBundleVersion, name = 'updateAssetBundleVersion'),
	url(r'^assetBundle/transfer/$', app.view.assetBundle.file.transferAssetBundle, name = 'transferAssetBundle'),
	url(r'^assetBundle/updateOrder/$', app.view.assetBundle.file.updateAssetBundleOrder, name = 'updateAssetBundleOrder'),
	url(r'^table/inquiry/content/$', app.view.table.file.inquiryTableContent, name='inquiryTableContent'),
	url(r'^table/inquiry/history/$', app.view.table.file.inquiryTableHistory, name='inquiryTableHistory'),
	url(r'^table/uploadMulti/$', app.view.table.file.uploadMultiTable, name='uploadMultiTable'),
	#url(r'^table/updateOrderAndSelectionInfo/$', app.view.table.file.updateOrderAndSelctionInfo, name='updateOrderAndSelectionInfo'),
	url(r'^table/tableApartInfo/$', app.view.table.file.tableApartInfo, name='tableApartInfo'),
	url(r'^table/download/$', app.view.table.file.downloadFile, name='downloadFile'),
	url(r'^table/updateVersion/$', app.view.table.file.updateTableVersion, name='updateTableVersion'),
	url(r'^table/transfer/$', app.view.table.file.transferTable, name='transferTable'),
	url(r'^game/inquiryUserInfo/$', app.view.game.userInfo.inquiryUserInfo, name='inquiryUserInfo'),
	url(r'^game/addUserInfo/$', app.view.game.userInfo.addUserInfo, name='addUserInfo'),
	url(r'^game/allUserModify/$', app.view.game.alluserInfo.allUserModify, name='allUserModify'),
	url(r'^game/insertItem/$', app.view.game.userInfo.insertItem, name='insertItem'),
	url(r'^game/deleteItem/$', app.view.game.userInfo.deleteItem, name='deleteItem'),
	url(r'^game/truncateUser/$', app.view.game.userInfo.truncateUser, name='truncateUser'),
	url(r'^account/validate/$', app.view.index.validateAccount, name='validateAccount'),
	url(r'^table/uploadMultiDB/$', app.view.table.file.uploadMultiDB, name='uploadMultiDB'),
	url(r'^table/viewStoreSchedule/$', app.view.table.file.viewStoreSchedule, name='viewStoreSchedule'),
	url(r'^game/getfriendList/$',  app.view.game.userInfo.getfriendList, name='getfriendList'),
	url(r'^game/publicNotice/$',  app.view.message.message.publicNotice, name='publicNotice'),
	url(r'^game/messagePush/$',  app.view.message.message.messagePush, name='messagePush'),
	url(r'^game/serverInhouseChecker/$',  app.view.game.userInfo.serverInhouseChecker, name='serverInhouseChecker'),
	url(r'^game/inGameNoticeList/$',  app.view.message.message.inGameNoticeList, name='inGameNoticeList'),
	url(r'^message/noticeReservation/$',  app.view.message.message.noticeReservation, name='noticeReservation'),
	url(r'^message/pushReservation/$',  app.view.message.message.pushReservation, name='pushReservation'),
	url(r'^message/getReservationPushList/$',  app.view.message.message.getReservationPushList, name='getReservationPushList'),
	url(r'^message/insertMail/$',  app.view.message.message.insertMail, name='insertMail'),
#	url(r'^log/logView/$',  app.view.log.log.logView, name='logView'),
	url(r'^shop/getShopInfo/$',  app.view.shopManage.shop.getShopInfo, name='getShopInfo'),
	url(r'^shop/addShopInfo/$',  app.view.shopManage.shop.addShopInfo, name='addShopInfo'),
	url(r'^shop/refreshShopInfo/$',  app.view.shopManage.shop.refreshShopInfo, name='refreshShopInfo'),
	url(r'^game/inquiryClanInfo/$',  app.view.game.clanInfo.inquiryClanInfo, name='inquiryClanInfo'),
	url(r'^game/inquiryEventInfo/$',  app.view.game.userInfo.inquiryEventInfo, name='inquiryEventInfo'),
	url(r'^game/maintenanceTime/$',  app.view.game.userInfo.maintenanceTime, name='maintenanceTime'),
	#url(r'^getTable/$', app.view.getResource.getTable, name='getTable'),
	url(r'^account/logout/$', app.view.index.logout, name='logout'),
	url(r'^login/$',
        django.contrib.auth.views.login,
        {
            'template_name': 'app/login.html',
            'authentication_form': app.forms.BootstrapAuthenticationForm,
            'extra_context':
            {
                'title': 'Log in',
                'year': datetime.now().year,
            }
        },
        name='login')
] 

'''
url(r'^logout/$',
    django.contrib.auth.views.logout,
    {
        'next_page': '/'
    },
    name='logout'),

# Uncomment the admin/doc line below to enable admin documentation:
    url(r'^admin/doc/', include('django.contrib.admindocs.urls')),

# Uncomment the next line to enable the admin:
    url(r'^admin/', include(admin.site.urls)),
'''
