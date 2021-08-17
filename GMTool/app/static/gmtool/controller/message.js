krApp.controller('messageController', function ($scope, $http) {
    $scope.runMode = '';
    $scope.userInfoType = '';
    $scope.identifierType = 'nickName';
    $scope.identifierText = '';
    $scope.header_names = [];
    $scope.datas = [];
    $scope.isAllChoiceCheck = false;


    $scope.normal_key_map = {
        'accounts': ['userId'],
        'units': ['id','unitName'],
        'cards': ['id','cardName'],
        'armors': ['id','Name'],
        'weapons': ['id'],
        'stage_levels': ['stageLevelId'],
        'decks': ['modeType', 'deckNum', 'slotType', 'orderNum'],
        'square_object': ['objectNum'],
        'noticePublic' : ['seq']
    }

    $scope.getUserInfoSettings = function () {
        return {
            "runMode": $scope.runMode,
            "userInfoType": $scope.userInfoType,
            "identifierType": $scope.identifierType,
            "identifierText": $scope.identifierText
        };
    }

    
    $scope.onUserInfoTypeChange = function (selectedUserInfoType) {
        $scope.userInfoType = selectedUserInfoType;
    }

    
    $scope.makeElementHtmlOfUserInfo = function (datas) {
        let tbody = document.getElementById('tbodyUserInfo');
        tbody.innerHTML = '';
        
        for (let tr_count = 0; tr_count < datas.length; tr_count++) {
            let data = datas[tr_count];

            let tr = tbody.insertRow(tr_count);
            tr.id = 'userInfoElement_{0}'.format(tr_count)

            let td_count = 0;
            html = "<input type='checkbox' class='form-control' name='isChecked[]' value='{0}' />".format(tr_count);
            var cell = tr.insertCell(td_count++);
            cell.innerHTML = html;

            for (let key in data) {
                let value = data[key];

                var cell = tr.insertCell(td_count++);
                if ($scope.userInfoType in $scope.normal_key_map && $scope.normal_key_map[$scope.userInfoType].indexOf(key) >= 0) {
                    cell.innerHTML = value;
                }
                else {               
                    html = "<input type='{0}' class='form-control' value='{1}' style='float:left; margin:auto' />&nbsp;\n";    
                    if (typeof (value) == "string") {
                        html = html.format("text", value);

                    }
                    else if (typeof (value) == "number") {
                        html = html.format("number", value);
                    }
                    
                    cell.innerHTML = html;
                }
            }
        }
    }

    $scope.updateUserInfoSettings = function () {
        $scope.runMode = $('#runMode').val();
        $scope.userInfoType = $('#userInfoType').val();
        $scope.identifierType = $('#identifierType').val();
        $scope.identifierText = $('#identifierText').val();
    }

    
    $scope.checkAllChoiceButton = function () {
        $scope.isAllChoiceCheck = $('#isAllChoiceCheck')[0].checked;

        let deletionCheckboxs = $("input[type='checkbox'][name='isChecked[]']");

        for (let i = 0; i < deletionCheckboxs.length; i++) {
            let checkbox = deletionCheckboxs[i];
            checkbox.checked = $scope.isAllChoiceCheck;
        }
    }
    
    $scope.publicNotice = function() {
        $scope.updateUserInfoSettings();

        let data_map = {
            'msg': 'getUserInfo',
            'settings': $scope.getUserInfoSettings(),
            'noticeMessage': $('#noticeMessage').val()
        }

        $scope.post('/game/publicNotice/', data_map, function(data) {
            $scope.datas = data;
            $scope.makeElementHtmlOfUserInfo($scope.datas)
            alert('succedd to Notice publish');
        }, function(res) {
            alert('failed to inquiry user infomation, {0}, {1}'.format(res.status, res.statusText));
           
        });
    }

    $scope.messagePush = function () {
        $scope.updateUserInfoSettings();

        let data_map = {
            'msg': 'push',
            'settings': $scope.getUserInfoSettings(),
            'title': $('#title').val(),
            'message': $('#message').val()
        }

        $scope.post('/game/messagePush/', data_map, function (data) {
            alert('succeed to pushMessage');
        }, function (res) {
            alert('failed to pushMessage, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.getNoticeInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        let data_map = {
            'msg': 'getNoticeInfo',
            'settings': $scope.getUserInfoSettings()
        }

        $scope.post('/game/inGameNoticeList/', data_map, function (data) {
            $scope.datas = data;
            $scope.header_names = Object.keys($scope.datas[0])
            $scope.makeElementHtmlOfUserInfo($scope.datas)
        }, function (res) {
            alert('failed to inquiry user infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.deleteNoticeInfo = function () {
        $scope.updateUserInfoSettings();

        var trIndexList = $("input[type='checkbox'][name='isChecked[]']:checked").map(function () {
            return this.value;
        }).get();

        deleteList = [];
        trIndexList.forEach(function (tr_index) {
            deleteList.push($scope.datas[tr_index]);
        });

        let data_map = {
            'msg': 'deleteNoticeInfo',
            'settings': $scope.getUserInfoSettings(),
            'deleteList': deleteList
        };

        $scope.post('/game/inGameNoticeList/', data_map, function (data) {
            if ($scope.
                ChoiceCheck) {
                $scope.isAllChoiceCheck = false;
                $('#isAllChoiceCheck').prop("checked", false);
            }
            let reduceIndex = 0;
            trIndexList.forEach(function (tr_index) {
                tr_index -= reduceIndex;
                $scope.datas.splice(tr_index, 1);

                reduceIndex += 1;
            });

            $scope.makeElementHtmlOfUserInfo($scope.datas);

            alert('succedd to delete Notice user infomation');
        }, function (res) {
            alert('failed to delete Notice infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.updateNoticeInfo = function () {
        $scope.updateUserInfoSettings();

    let data_map = {
        'msg': 'updateNoticeInfo',
        'settings': $scope.getUserInfoSettings(),
        'updateList': []
    };

    for (let tr_count = 0; tr_count < $scope.datas.length; tr_count++) {
        let originData = $scope.datas[tr_count];
        let elementRef = $('#userInfoElement_{0}'.format(tr_count))[0];

        let isDifferent = false;
        let newDataMap = {}

        for (let td_count = 1; td_count < elementRef.childNodes.length; td_count++) {
            let td_child = elementRef.childNodes[td_count].firstChild;
            let key = $scope.header_names[td_count - 1];
            let value = undefined;
            let origin_value = originData[key];

            if (td_child.nodeName == '#text') {
                value = td_child.data;
            }
            else if (td_child.nodeName == 'INPUT') {
                value = td_child.value;
            }

            if (typeof (origin_value) == 'number') {
                value = Number(value);
            }
            else if (typeof (origin_value) == 'string') {
                value = String(value);
            }

            newDataMap[key] = value;

            if (!isDifferent) {
                isDifferent = value != origin_value;
            }
        }

        if (isDifferent) {
            data_map['updateList'].push(newDataMap);
        }
    }

    $scope.post('/game/inGameNoticeList/', data_map, function (data) {
        alert('succedd to update user infomation');
    }, function (res) {
        alert('failed to update user infomation, {0}, {1}'.format(res.status, res.statusText));
    });
}

    $scope.noticeReservation = function () {
        $scope.updateUserInfoSettings();

        let data_map = {
            'msg': 'noticeReservation',
            'settings': $scope.getUserInfoSettings(),
            'noticeMessage': $('#noticeMessage').val(),
            'count': $('#count').val(),
            'startTime': $('#startTime').val()
        }

        $scope.post('/message/noticeReservation/', data_map, function (data) {
            alert('succedd to Notice reservation');
        }, function (res) {
            alert('failed to Reservation infomation, {0}, {1}'.format(res.status, res.statusText));

        });
    }


    $scope.pushReservation = function () {
        $scope.updateUserInfoSettings();

        let data_map = {
            'msg': 'pushReservation',
            'settings': $scope.getUserInfoSettings(),
            'pushTitle': $('#pushTitle').val(),
            'pushBody': $('#pushBody').val(),
            'reservationTime': $('#reservationTime').val(),
            'condition': $('#condition').val(),

        }

        $scope.post('/message/pushReservation/', data_map, function (data) {
            alert('succedd to push message reservation');
        }, function (res) {
            alert('failed to Reservation infomation, {0}, {1}'.format(res.status, res.statusText));

        });
    }

    $scope.getReservationPushList = function () {
        $scope.updateUserInfoSettings();

        let data_map = {
            'msg': 'getReservationPushList',
            'settings': $scope.getUserInfoSettings()
        }

        $scope.post('/message/getReservationPushList/', data_map, function (data) {
            $scope.datas = data;
            $scope.header_names = Object.keys($scope.datas[0])
            $scope.makeElementHtmlOfUserInfo($scope.datas)
        }, function (res) {
            alert('failed to Reservation infomation, {0}, {1}'.format(res.status, res.statusText));

        });
    }

    $scope.deleteReservationPushList = function () {
        $scope.updateUserInfoSettings();

        var trIndexList = $("input[type='checkbox'][name='isChecked[]']:checked").map(function () {
            return this.value;
        }).get();

        deleteList = [];
        trIndexList.forEach(function (tr_index) {
            deleteList.push($scope.datas[tr_index]);
        });

        let data_map = {
            'msg': 'deleteReservationPushList',
            'settings': $scope.getUserInfoSettings(),
            'deleteList': deleteList
        };

        $scope.post('/message/getReservationPushList/', data_map, function (data) {
            if ($scope.
                ChoiceCheck) {
                $scope.isAllChoiceCheck = false;
                $('#isAllChoiceCheck').prop("checked", false);
            }
            let reduceIndex = 0;
            trIndexList.forEach(function (tr_index) {
                tr_index -= reduceIndex;
                $scope.datas.splice(tr_index, 1);

                reduceIndex += 1;
            });

            $scope.makeElementHtmlOfUserInfo($scope.datas);

            alert('succedd to delete PUSH reservation infomation');
        }, function (res) {
                alert('failed to delete PUSH reservation infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.insertMail = function () {
        $scope.updateUserInfoSettings();

        console.log($scope.getUserInfoSettings().userInfoType)
        if ($scope.getUserInfoSettings().userInfoType == 'publicMail') {
            data_map = {
                'msg': 'publicMail',
                'settings': $scope.getUserInfoSettings(),
                'startDateTime': $('#startDateTime').val(),
                'endDateTime': $('#endDateTime').val(),
                'mailIcon': $('#mailIcon').val(),
                'productId': $('#productId').val(),
                'kr_mailTitle': $('#kr_mailTitle').val(),
                'kr_mailText': $('#kr_mailText').val(),
                'en_mailTitle': $('#en_mailTitle').val(),
                'en_mailText': $('#en_mailText').val(),
                'jp_mailTitle': $('#jp_mailTitle').val(),
                'jp_mailText': $('#jp_mailText').val()

            }
        } else if ($scope.getUserInfoSettings().userInfoType == 'privateMail') {
            data_map = {
                'msg': 'privateMail',
                'settings': $scope.getUserInfoSettings(),
                'startDateTime': $('#startDateTime').val(),
                'endDateTime': $('#endDateTime').val(),
                'mailIcon': $('#mailIcon').val(),
                'productId': $('#productId').val(),
                'mailTitle': $('#mailTitle').val(),
                'mailText': $('#mailText').val(),
                'userId': $('#userId').val()
            }
        } else if ($scope.getUserInfoSettings().userInfoType == 'systemMail') {
             data_map = {
                'msg': 'systemMail',
                'settings': $scope.getUserInfoSettings(),
                'startDateTime': $('#startDateTime').val(),
                'endDateTime': $('#endDateTime').val(),
                'systemMailId': $('#systemMailId').val(),
                 'userId': $('#userId').val()
            }
        } else if ($scope.getUserInfoSettings().userInfoType == 'noticeMail') {
            data_map = {
                'msg': 'noticeMail',
                'settings': $scope.getUserInfoSettings(),
                'userId': $('#userId').val()
            }
        } else {
             data_map = {
                'msg' : 'none',
            }
        }
        

        $scope.post('/message/insertMail/', data_map, function () {
            alert('mail send finished');
        }, function (res) {
            alert('failed to Reservation infomation, {0}, {1}'.format(res.status, res.statusText));

        });
    }





    $scope.post = function (url, data, success_callback, fail_callback) {
        let req = {
            method: 'POST',
            url: url,
            headers: {
                'Accept': 'application/json; charset=utf-8',
                'Content-Type': 'application/json; charset=utf-8'
            },
            data: data,
            dataType: 'json'
        };

        $http(req).then(function (response) {
            success_callback(response.data);

        }, function (response) {
            fail_callback(response);
        });
    }
});