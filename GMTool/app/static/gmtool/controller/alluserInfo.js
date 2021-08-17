krApp.controller('userInfoController', function ($scope, $http) {
    $scope.runMode = '';
    $scope.userInfoType = '';
    $scope.identifierText = '';
    $scope.header_names = [];
    $scope.datas = [];
    $scope.isAllChoiceCheck = false;

    $scope.normal_key_map = {
        'accounts': ['userId'],
        'users':['userId'],
        'units': ['id','unitName'],
        'cards': ['id','cardName'],
        'armors': ['id','Name'],
        'weapons': ['id'],
        'stage_levels': ['stageLevelId'],
        'decks': ['modeType', 'deckNum', 'slotType', 'orderNum']
    }

    $scope.updateUserInfoSettings = function () {
        $scope.runMode = $('#runMode').val();
        $scope.userInfoType = $('#userInfoType').val();
        $scope.identifierType = $('#identifierType').val();
        $scope.identifierText = $('#identifierText').val();
    }

    $scope.getUserInfoSettings = function () {
        return {
            "runMode": $scope.runMode,
            "userInfoType": $scope.userInfoType,
            "identifierType": $scope.identifierType,
            "identifierText": $scope.identifierText
        };
    }

    $scope.allUserInfo = function () {
        $scope.updateUserInfoSettings();
      
        if ($scope.userInfoType == '' ) return;

        let data_map = {
            'settings': $scope.getUserInfoSettings(),
            'updateMap': {}
        }

        if ($scope.userInfoType == 'accounts') {

        }
        else if ($scope.userInfoType == 'users') {
            let gold = $('#users_gold').val();
            //let gem = $('#users_gem').val();

            data_map['updateMap'] = {
                'gold': gold,
            //    'gem': gem
            };

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
        else if ($scope.userInfoType == 'allUnit') {
            
            let level = $('#alulevel').val();


            data_map['updateMap'] = {
                'level': level
            }
        }
        else if ($scope.userInfoType == 'allSkill') {
            let level = $('#alslevel').val();

            data_map['updateMap'] = {
                'level': level
            }
        }
        else if ($scope.userInfoType == 'allWeapons') {
            let level = $('#alwlevel').val();

            data_map['updateMap'] = {
                'level': level
            }
        }
        else if ($scope.userInfoType == 'allUnitsLevel') {
            let level = $('#aulevel').val();

            data_map['updateMap'] = {
                'level': level
            }
        }
        else if ($scope.userInfoType == 'allSkillsLevel') {
            let level = $('#aslevel').val();

            data_map['updateMap'] = {
                'level': level
            }
        }
        else if ($scope.userInfoType == 'allWeaponsLevel') {
            let level = $('#awlevel').val();

            data_map['updateMap'] = {
                'level': level
            }
        }

        $scope.post('/game/allUserModify/', data_map, function (data) {
            alert('succeed to add user infomation');
        }, function (res) {
            alert('failed to add user infomation, {0}, {1}'.format(res.status, res.statusText));
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