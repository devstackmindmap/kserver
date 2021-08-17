krApp.controller('clanInfoController', function ($scope, $http) {
    $scope.runMode = '';
    $scope.userInfoType = '';
    $scope.identifierType = 'nickName';
    $scope.identifierText = '';
    $scope.header_names = [];
    $scope.datas = [];
    $scope.isAllChoiceCheck = false;

    $scope.normal_key_map = {
        'clan': ['clanId'],
        'clanMembers': ['seq'],
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

    $scope.getClanInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        let data_map = {
            'msg': 'getClanInfo',
            'settings': $scope.getUserInfoSettings()
        }

        $scope.post('/game/inquiryClanInfo/', data_map, function (data) {
            $scope.datas = data;
            $scope.header_names = Object.keys($scope.datas[0])
            $scope.makeElementHtmlOfUserInfo($scope.datas)
        }, function (res) {
            alert('failed to inquiry Clan infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }


    $scope.updateClanInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        let data_map = {
            'msg': 'updateClanInfo',
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

        $scope.post('/game/inquiryClanInfo/', data_map, function (data) {
            alert('succedd to update user infomation');
        }, function (res) {
            alert('failed to update Clan infomation, {0}, {1}'.format(res.status, res.statusText));
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

    $scope.updateUserInfoSettings = function () {
        $scope.runMode = $('#runMode').val();
        $scope.userInfoType = $('#userInfoType').val();
        $scope.identifierType = $('#identifierType').val();
        $scope.identifierText = $('#identifierText').val();
    }

    $scope.deleteClanInfo = function () {
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
            'msg': 'deleteClanInfo',
            'settings': $scope.getUserInfoSettings(),
            'deleteList': deleteList
        };

        $scope.post('/game/inquiryClanInfo/', data_map, function (data) {
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

            alert('succedd to delete Clan infomation');
        }, function (res) {
            alert('failed to delete clan infomation, {0}, {1}'.format(res.status, res.statusText));
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