krApp.controller('userInfoController', function ($scope, $http) {
    $scope.runMode = '';
    $scope.userInfoType = '';
    $scope.identifierType = 'nickName';
    $scope.identifierText = '';
    $scope.header_names = [];
    $scope.datas = [];
    $scope.isAllChoiceCheck = false;

    $scope.normal_key_map = {
        'accounts': ['userId'],
        //'units': ['id'],
        //'cards': [,'cardName'],
        //'armors': ['id','Name'],
        //'weapons': ['id'],
        //'stage_levels': ['stageLevelId'],
        //'decks': ['modeType', 'deckNum', 'slotType', 'orderNum'],
        //'square_object': ['objectNum'],
        //'noticePublic': ['seq'],
        //'EventInfo': ['eventId'],
        //'user_info': ['userId']
        'payment_pending': ['userId', 'productId', 'productTableType', 'storeProductId', 'purchasedToken', 'platformType', 'payedTime', 'updatedTime'],
        'payment_pending_issue': ['userId', 'productId', 'transactionId', 'is_pending', 'productTableType', 'storeProductId', 'purchasedToken', 'platformType']
        
    }
    $scope.datetime_key_map = {
        
        'accounts': ['limitLoginDate']

    }

    $scope.getUserInfoSettings = function () {
        return {
            "runMode": $scope.runMode,
            "userInfoType": $scope.userInfoType,
            "identifierType": $scope.identifierType,
            "identifierText": $scope.identifierText
        };
    }

    $scope.addUserInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        let data_map = {
            'settings': $scope.getUserInfoSettings(),
            'updateMap': {}
        }

        if ($scope.userInfoType == 'accounts') {

        }
        else if ($scope.userInfoType == 'users') {

        }
        else if ($scope.userInfoType == 'cards') {
            let id = $('#cards_cardId').val();
            let level = $('#cards_level').val();
            let count = $('#cards_count').val();

            data_map['updateMap'] = {
                'id': id,
                'level': level,
                'count': count
            };
        }
        else if ($scope.userInfoType == 'units') {
            let id = $('#units_unitId').val();
            let level = $('#units_level').val();
            let count = $('#units_count').val();
            let rankPoint = $('#units_rankPoint').val();
            let rankLevel = $('#units_rankLevel').val();

            data_map['updateMap'] = {
                'id': id,
                'level': level,
                'count': count,
                'rankPoint': rankPoint,
                'rankLevel': rankLevel
            };
        }
        else if ($scope.userInfoType == 'armors') {
            let id = $('#armors_id').val();
            let level = $('#armors_level').val();
            let count = $('#armors_count').val();
            let unitId = $('#armors_unitId').val();

            data_map['updateMap'] = {
                'id': id,
                'level': level,
                'count': count,
                'unitId': unitId
            };
        }
        else if ($scope.userInfoType == 'weapons') {
            let id = $('#weapons_id').val();
            let level = $('#weapons_level').val();
            let count = $('#weapons_count').val();
            let unitId = $('#weapons_unitId').val();

            data_map['updateMap'] = {
                'id': id,
                'level': level,
                'count': count,
                'unitId': unitId
            };
        }
        else if ($scope.userInfoType == 'stage_levels') {
            let stageLevelId = $('#stage_levels_stageLevelId').val();
            let clearCount = $('#stage_levels_clearCount').val();

            data_map['updateMap'] = {
                'stageLevelId': stageLevelId,
                'clearCount': clearCount
            };
        }
        else if ($scope.userInfoType == 'decks') {
            let modeType = $('#decks_modeType').val();
            let deckNum = $('#decks_deckNum').val();
            let orderNum = $('#decks_orderNum').val();
            let slotType = $('#decks_slotType').val();
            let classId = $('#decks_classId').val();
            let deckName = $('#decks_deckName').val();

            data_map['updateMap'] = {
                'modeType': modeType,
                'deckNum': deckNum,
                'orderNum': orderNum,
                'slotType': slotType,
                'classId': classId,
                'deckName': deckName
            }
        }
        else if ($scope.userInfoType == 'give_skin') {
            let skinId = $('#skinID').val();

            data_map['updateMap'] = {
                'skinId': skinId
            }
        }
        else if ($scope.userInfoType == 'store_schedule') {
            let scheduleType = $('#scheduleType').val();
            let startDateTime = $('#startDateTime').val();
            let endDateTime = $('#endDateTime').val();

            data_map['updateMap'] = {
                'scheduleType': scheduleType,
                'startDateTime': startDateTime,
                'endDateTime': endDateTime
            }
        }

        $scope.post('/game/addUserInfo/', data_map, function (data) {
            alert('succeed to add user infomation');
        }, function (res) {
            alert('failed to add user infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }
    
    $scope.insertItem = function () {
        $scope.updateUserInfoSettings();

        if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        let data_map = {
            'settings': $scope.getUserInfoSettings(),
            'updateMap': {}
        }

        if ($scope.userInfoType == 'cards') {
            let id = $('#cards_cardId').val();
            let level = $('#cards_level').val();
            let count = $('#cards_count').val();

            data_map['updateMap'] = {
                'id': id,
                'level': level,
                'count': count
            };
        }
        else if ($scope.userInfoType == 'units') {
            let id = $('#units_unitId').val();
            let level = $('#units_level').val();
            let count = $('#units_count').val();
            let rankPoint = $('#units_rankPoint').val();
            let rankLevel = $('#units_rankLevel').val();

            data_map['updateMap'] = {
                'id': id,
                'level': level,
                'count': count,
                'rankPoint': rankPoint,
                'rankLevel': rankLevel
            };
        }
        else if ($scope.userInfoType == 'armors') {
            let id = $('#armors_id').val();
            let level = $('#armors_level').val();
            let count = $('#armors_count').val();

            data_map['updateMap'] = {
                'id': id,
                'level': level,
                'count': count,
            };
        }
        else if ($scope.userInfoType == 'weapons') {
            let id = $('#weapons_id').val();
            let level = $('#weapons_level').val();
            let count = $('#weapons_count').val();

            data_map['updateMap'] = {
                'id': id,
                'level': level,
                'count': count,
            };
        }

        $scope.post('/game/insertItem/', data_map, function (data) {
            alert('succeed to add user infomation');
        }, function (res) {
            alert('failed to add user infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.deleteItem = function () {
        $scope.updateUserInfoSettings();

        if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        
        let data_map = {
            'settings': $scope.getUserInfoSettings(),
            'updateMap': {}
        }

        if ($scope.userInfoType == 'cards') {
            let id = $('#cards_cardId').val();

            data_map['updateMap'] = {
                'id': id
            };
        }
        else if ($scope.userInfoType == 'units') {
            let id = $('#units_unitId').val();

            data_map['updateMap'] = {
                'id': id
            };
        }
        else if ($scope.userInfoType == 'armors') {
            let id = $('#armors_id').val();

            data_map['updateMap'] = {
                'id': id
            };
        }
        else if ($scope.userInfoType == 'weapons') {
            let id = $('#weapons_id').val();

            data_map['updateMap'] = {
                'id': id
            };
        }
        else if ($scope.userInfoType == 'skin') {
            let id = $('#skin_id').val();

            data_map['updateMap'] = {
                'id': id
            };
        }

        $scope.post('/game/deleteItem/', data_map, function (data) {
            alert('succeed to Delete user infomation');
        }, function (res) {
            alert('failed to Delete user infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    
    $scope.onUserInfoTypeChange = function (selectedUserInfoType) {
        $scope.userInfoType = selectedUserInfoType;
    }

    $scope.getUserInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        let data_map = {
            'msg': 'getUserInfo',
            'settings': $scope.getUserInfoSettings()
        }

        $scope.post('/game/inquiryUserInfo/', data_map, function (data) {
            $scope.datas = data;
            $scope.header_names = Object.keys($scope.datas[0])
            $scope.makeElementHtmlOfUserInfo($scope.datas)
        }, function (res) {
            alert('failed to inquiry user infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }


    $scope.truncateuser = function () {
        $scope.updateUserInfoSettings();
        if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        let data_map = {
            'msg': 'truncateUser',
            'settings': $scope.getUserInfoSettings(),
        };

        $scope.post('/game/truncateUser/', data_map, function (data) {
            alert('succedd to update user infomation');
        }, function (res) {
            alert('failed to update user infomation, {0}, {1}'.format(res.status, res.statusText));
        });

    }


    $scope.updateUserInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        let data_map = {
            'msg': 'updateUserInfo',
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

                if (typeof(origin_value) == 'number') {
                    value = Number(value);
                }
                else if (typeof(origin_value) == 'string') {
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
        console.log(data_map)

        $scope.post('/game/inquiryUserInfo/', data_map, function (data) {
            alert('succedd to update user infomation');
        }, function (res) {
            alert('failed to update user infomation, {0}, {1}'.format(res.status, res.statusText));
        });
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
                    html = "<span style='font-size:11px; margin:auto;'> {0} </span>"
                    html = html.format(value);
                    cell.innerHTML = html;
                    
                }
                //else if ($scope.userInfoType in $scope.datetime_key_map && $scope.datetime_key_map[$scope.userInfoType].indexOf(key) >= 0) {
                    
                //    html = "<input type='{0}' class='form-control' value='{1}' style='float:left; margin:auto; font-size:9pt' />&nbsp;\n";

                //    if (typeof (value) == "string") {
                //        var date = new Date(value).toISOString()
                //        var datelocal = date.slice(0,16)
                //        html = html.format("datetime-local", datelocal);
                        
                //    }
                //    else if (typeof (value) == "number") {
                //        html = html.format("number", value);
                //    }

                //    cell.innerHTML = html;
                //}
                else {               
                    
                    html = "<input type='{0}' class='form-control' value='{1}' style='float:left; margin:auto; font-size:9pt' />";    
                    
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

    $scope.deleteUserInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        var trIndexList = $("input[type='checkbox'][name='isChecked[]']:checked").map(function () {
            return this.value;
        }).get();

        deleteList = [];
        trIndexList.forEach(function (tr_index) {
            deleteList.push($scope.datas[tr_index]);
        });

        let data_map = {
            'msg': 'deleteUserInfo',
            'settings': $scope.getUserInfoSettings(),
            'deleteList': deleteList
        };

        $scope.post('/game/inquiryUserInfo/', data_map, function (data) {
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

            alert('succedd to delete user infomation');
        }, function (res) {
            alert('failed to delete user infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.checkAllChoiceButton = function () {
        $scope.isAllChoiceCheck = $('#isAllChoiceCheck')[0].checked;

        let deletionCheckboxs = $("input[type='checkbox'][name='isChecked[]']");

        for (let i = 0; i < deletionCheckboxs.length; i++) {
            let checkbox = deletionCheckboxs[i];
            checkbox.checked = $scope.isAllChoiceCheck;
        }
    }

    $scope.getfriendList = function () {
        $scope.updateUserInfoSettings();

        if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        let data_map = {
            'msg': 'getUserInfo',
            'settings': $scope.getUserInfoSettings()
        }

        $scope.post('/game/getfriendList/', data_map, function (data) {
            $scope.datas = data;
            $scope.header_names = Object.keys($scope.datas[0])
            $scope.makeElementHtmlOfUserInfo($scope.datas)
        }, function (res) {
            alert('failed to inquiry user infomation, {0}, {1}'.format(res.status, res.statusText));
        });
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

    $scope.setInhouseChecker = function () {
        $scope.updateUserInfoSettings();

        let data_map = {
            'msg': 'setInhouse',
            'settings': $scope.getUserInfoSettings(),
        }

        $scope.post('/game/serverInhouseChecker/', data_map, function (data) {
            //$scope.datas = data;
            
            //$scope.makeElementHtmlOfUserInfo($scope.datas)
            alert('succedd to SetInhouse {0} Check publish'.format(data) );
        }, function (res) {
                alert('failed to inhouse checker infomation, {0}, {1}'.format(res.status, res.statusText));

        });
    }

    $scope.releaseInhouseChecker = function () {
        $scope.updateUserInfoSettings();

        let data_map = {
            'msg': 'releaseInhouse',
            'settings': $scope.getUserInfoSettings(),
        }

        $scope.post('/game/serverInhouseChecker/', data_map, function (data) {
            //$scope.datas = data;
            
            alert('succedd to Release Inhouse {0} Check publish'.format(data));
        }, function (res) {
            alert('failed to inhouse checker infomation, {0}, {1}'.format(res.status, res.statusText));

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


    $scope.getEventInfo = function () {
        
        $scope.updateUserInfoSettings();
        
        $scope.userInfoType = 'EventInfo';
        let data_map = {
            'msg': 'getEventInfo',
            'settings': $scope.getUserInfoSettings()
        }

        $scope.post('/game/inquiryEventInfo/', data_map, function (data) {
            
            $scope.datas = data;
            $scope.header_names = Object.keys($scope.datas[0])
            $scope.UTCTime = new Date(Date.now()).toUTCString()
            $scope.makeElementHtmlOfUserInfo($scope.datas)
            
        }, function (res) {
            alert('failed to inquiry Event infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.deleteEventInfo = function () {
        $scope.updateUserInfoSettings();


        var trIndexList = $("input[type='checkbox'][name='isChecked[]']:checked").map(function () {
            return this.value;
        }).get();

        deleteList = [];
        trIndexList.forEach(function (tr_index) {
            deleteList.push($scope.datas[tr_index]);
        });

        let data_map = {
            'msg': 'deleteEventInfo',
            'settings': $scope.getUserInfoSettings(),
            'deleteList': deleteList
        };

        $scope.post('/game/inquiryEventInfo/', data_map, function (data) {
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

            alert('succedd to delete Event infomation');
        }, function (res) {
            alert('failed to delete Event infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.updateEventInfo = function () {
        $scope.updateUserInfoSettings();

        let data_map = {
            'msg': 'updateEventInfo',
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

        $scope.post('/game/inquiryEventInfo/', data_map, function (data) {
            alert('succedd to update Event infomation');
        }, function (res) {
            alert('failed to update Event infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.maintenanceTimeAdd = function () {
        $scope.updateUserInfoSettings();

        //if ($scope.userInfoType == '' || $scope.identifierText == '') return;
        
        let data_map = {
            'msg': 'maintenanceAdd',
            'settings': $scope.getUserInfoSettings(),
            'updateMap': {}
        }

        let startDateTime = $('#startTime').val();
        let endDateTime = $('#endTime').val();
        
        data_map['updateMap'] = {
            'startDateTime': startDateTime,
            'endDateTime': endDateTime,
        };
    
        $scope.post('/game/maintenanceTime/', data_map, function (data) {
            alert('succeed to Maintenance Date Time Insert');
        }, function (res) {
            alert('failed to add user infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }
    $scope.getMaintenanceTime = function () {
        $scope.updateUserInfoSettings();

        //if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        let data_map = {
            'msg' : 'maintenanceList',
            'settings': $scope.getUserInfoSettings(),
            'updateMap': {}
        }

        $scope.post('/game/maintenanceTime/', data_map, function (data) {
            $scope.datas = data;
            $scope.header_names = Object.keys($scope.datas[0])
            $scope.UTCTime = new Date(Date.now()).toUTCString()
            $scope.makeElementHtmlOfUserInfo($scope.datas)
        }, function (res) {
            alert('failed to add user infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }


    $scope.addEventInfo = function () {

        $scope.updateUserInfoSettings();

        $scope.userInfoType = 'EventInfo';

        let data_map = {
            'msg': 'addEventInfo',
            'settings': $scope.getUserInfoSettings()
        }

        let startDateTime = $('#startTime').val();
        let endDateTime = $('#endTime').val();
        let eventType = $('#eventType').val();

        data_map['updateMap'] = {
            'startDateTime': startDateTime,
            'endDateTime': endDateTime,
            'eventType' : eventType,
        };

        $scope.post('/game/inquiryEventInfo/', data_map, function (data) {

            $scope.datas = data;
            $scope.header_names = Object.keys($scope.datas[0])
            $scope.UTCTime = new Date(Date.now()).toUTCString()
            $scope.makeElementHtmlOfUserInfo($scope.datas)

        }, function (res) {
            alert('failed to inquiry Event infomation, {0}, {1}'.format(res.status, res.statusText));
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