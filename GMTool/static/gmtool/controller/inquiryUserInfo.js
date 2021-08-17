krApp.controller('InquiryUserInfoController', function ($scope, $http) {
    $scope.runMode = 'Dev2'
    $scope.userInfoType = 'accounts'
    $scope.identifierType = 'nickName'
    $scope.identifierText = ''
    $scope.header_names = []
    $scope.datas = []

    $scope.normal_key_map = {
        'accounts': ['userId'],
        'units': ['unitId'],
        'cards': ['cardId'],
        'equipments': ['equipId']
    }

    $scope.getUserInfoSettings = function () {
        return {
            "runMode": $scope.runMode,
            "userInfoType": $scope.userInfoType,
            "identifierType": $scope.identifierType,
            "identifierText": $scope.identifierText
        };
    }
    
    $scope.updateUserInfo = function () {
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
            
            for (let td_count = 0; td_count < elementRef.childNodes.length; td_count++) {
                let td_child = elementRef.childNodes[td_count].firstChild;
                let key = $scope.header_names[td_count];
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

        $scope.post('/game/inquiryUserInfo/', data_map, function (data) {
            alert('succedd to update user infomation');
        }, function (res) {
            alert('failed to update user infomation, {0}, {1}'.format(res.status, res.statusText));
        });

    }

    $scope.makeElementHtmlOfUserInfo = function (datas, normalFormatKeys) {
        let tbody = document.getElementById('tbodyUserInfo');
        tbody.innerHTML = '';
        
        for (let tr_count = 0; tr_count < datas.length; tr_count++) {
            let data = datas[tr_count];

            let tr = tbody.insertRow(tr_count);
            tr.id = 'userInfoElement_{0}'.format(tr_count)

            let td_count = 0;
            for (let key in data) {
                let value = data[key];

                if ($scope.userInfoType in $scope.normal_key_map && $scope.normal_key_map[$scope.userInfoType].indexOf(key) == -1) {
                    html = "<input type='{0}' class='form-control' value='{1}' style='float:left; margin:auto' />&nbsp;\n";
                    if (typeof (value) == "string") {
                        value = html.format("text", value);
                    }
                    else if (typeof (value) == "number") {
                        value = html.format("number", value);
                    }
                }

                var cell = tr.insertCell(td_count);
                cell.innerHTML = value;

                ++td_count;
            }
        }
    }

    $scope.getUserInfo = function () {
        $scope.runMode = $('#runMode').val();
        $scope.userInfoType = $('#userInfoType').val();
        $scope.identifierType = $('#identifierType').val();
        $scope.identifierText = $('#identifierText').val();

        let data_map = {
            'msg': 'getUserInfo',
            'settings': $scope.getUserInfoSettings()
        }
        
        $scope.post('/game/inquiryUserInfo/', data_map, function (data) {
            $scope.datas = data;
            $scope.header_names = Object.keys($scope.datas[0])


            $scope.makeElementHtmlOfUserInfo($scope.datas, $scope.normal_key_map)
        }, function (res) {
            alert('failed to inquiry user infomation, {0}, {1}'.format(res.status, res.statusText));
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