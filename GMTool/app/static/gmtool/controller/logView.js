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


    $scope.logView = function () {
        
        $scope.updateUserInfoSettings();


        let dbCollection = $('#dbCollection').val();
        let messageType = $('#messageType').val();
        let fromTime = $('#fromTime').val();
        let toTime = $('#toTime').val();

        let data_map = {
            'msg': 'get',
            'settings': $scope.getUserInfoSettings(),
            'dbCollection': dbCollection,
            'messageType': messageType,
            'fromTime': fromTime,
            'toTime': toTime

        }

        $scope.post('/log/logView/', data_map, function (data) {
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