krApp.controller('tableController', function ($scope, $http) {
    $scope.runMode = '';
    $scope.tableType = '';
    $scope.tableRows = [];
    $scope.isAllChoiceCheck = false;
    $scope.tableName = '';

    $scope.getTables = function () {
        $scope.updateTableSettings();

        let data_map = {
            'msg': 'getTables',
            'runMode': $scope.runMode,
            'tableType': $scope.tableType
        }

        $scope.post('/table/inquiry/history/', data_map, function (data) {
            $scope.tableRows = data;
            
        }, function (res) {

            alert('failed to get tables, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.getTableOrderAndSelectionInfo = function () {
        $scope.updateTableSettings();

        let data_map = {
            'msg': 'get',
            'runMode': $scope.runMode
        }

        $scope.post('/table/updateOrderAndSelectionInfo/', data_map, function (data) {
            $scope.tableRows = data;
        }, function (res) {
            alert('failed to get asset bundles, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.getTableApartInfo = function () {
        $scope.updateTableSettings();

        let data_map = {
            'msg': 'get',
            'runMode': $scope.runMode
        }

        $scope.post('/table/tableApartInfo/', data_map, function (data) {
            $scope.datas = data;
            $scope.header_names = Object.keys(data[0])
            $scope.makeElementHtmlOfUserInfo($scope.datas)

        }, function (res) {
            alert('failed to get table, {0}, {1}'.format(res.status, res.statusText));
        });
    }
    

    $scope.updateTableOrderAndSelectionInfo = function () {
        let changedTableInfoList = [];
        $scope.tableRows.forEach(function (tableInfo) {
            let currentOrderNum = parseInt($("input[type='number'][name='{0}']".format(tableInfo.name)).val());
            let currentSelection = 0;
            if ($("input[type='checkbox'][name='{0}']".format(tableInfo.name)).is(":checked")) {
                currentSelection = 1;
            }

            if (currentOrderNum != tableInfo.orderNum || currentSelection != tableInfo.isSelected) {
                changedTableInfoList.push({
                    'name': tableInfo.name,
                    'orderNum': currentOrderNum,
                    'isSelected': currentSelection
                });
            }
        });

        if (changedTableInfoList.length == 0) {
            return;
        }

        $scope.updateTableSettings();

        let data_map = {
            'msg': 'update',
            'runMode': $scope.runMode,
            'updateList': changedTableInfoList
        };

        $scope.post('/table/updateOrderAndSelectionInfo/', data_map, function (data) {
            changedTableInfoList.forEach(function (changedTableInfo) {
                var tableInfo = $scope.tableRows.find((data) => data.name == changedTableInfo.name);
                tableInfo.orderNum = changedTableInfo.orderNum;
                tableInfo.isSelected = changedTableInfo.isSelected;
            });

            $scope.tableRows.sort(function (data1, data2) {
                return data2.orderNum - data1.orderNum;
            });

            alert('succeed to arrange order and selection of table');
        }, function (res) {
            alert('failed to get table info, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.updateTableApartInfo = function () {
        

        let data_map = {
            'msg': 'update',
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

        $scope.post('/table/tableApartInfo/', data_map, function (data) {
            alert('succedd to update user infomation');
        }, function (res) {
            alert('failed to update user infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }





    $scope.checkAllChoiceButton = function () {
        $scope.isAllChoiceCheck = $('#isAllChoiceCheck')[0].checked;

        let deletionCheckboxs = $('input[name="isChecked[]"]');

        for (let i = 0; i < deletionCheckboxs.length; i++) {
            let checkbox = deletionCheckboxs[i];
            checkbox.checked = $scope.isAllChoiceCheck;
        }
    }

    $scope.updateTableSettings = function () {
        $scope.runMode = $('#runMode').val();
        $scope.tableType = $('#tableType').val();
        $scope.tableName = $('#tableName').val();
        $scope.bundleVersion = $('#bundleVersion').val();
    }

    $scope.deleteSelectedTables = function () {
        if ($scope.tableRows.length == 0) {
            return;
        }

        let selectedVersions = $('input[name="isChecked[]"]:checked').map(function () {
            return this.value;
        }).get();

        if (selectedVersions.length == 0) {
            return;
        }

        $scope.updateTableSettings();

        let data_map = {
            'msg': 'deleteTables',
            'runMode': $scope.runMode,
            'deletionVersionList': [],
            'bundleVersion': $('#bundleVersion').val()
        }

        selectedVersions.forEach(function (version) {
            data_map['deletionVersionList'].push(version);
        });

        $scope.post('/table/inquiry/history/', data_map, function (data) {
            if ($scope.isAllChoiceCheck) {
                $('#isAllChoiceCheck')[0].checked = false;
                $scope.checkAllChoiceButton();
            }

            selectedVersions.forEach(function (version) {
                for (let i = $scope.tableRows.length - 1; i > -1; i--) {
                    if ($scope.tableRows[i]['version'] == version) {
                        $scope.tableRows.splice(i, 1);
                        break;
                    }
                }
            });
        }, function (res) {
            alert('failed to delete tables, {0}, {1}'.format(res.status, res.statusText));
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

    $scope.viewStoreSchedule = function () {
        $scope.updateTableSettings();

        let data_map = {
            'msg': 'viewStoreSchedule',
            'runMode': $scope.runMode,
            'tableName' : $scope.tableName
            
        };

        $scope.post('/table/viewStoreSchedule/', data_map, function (data) {
            $scope.datas = data;
            $scope.header_names = Object.keys(data[0])
            $scope.makeElementHtmlOfSchedule($scope.datas)
            
        }, function (res) {
            alert('failed');
            });

        return 
    };

    $scope.makeElementHtmlOfSchedule = function (datas) {
        let tbody = document.getElementById('tbodyStoreInfo');
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
                
                //if ($scope.userInfoType in $scope.normal_key_map && $scope.normal_key_map[$scope.userInfoType].indexOf(key) >= 0) {
                //    cell.innerHTML = value;
                //}
                //else {
                    html = "<input type='{0}' class='form-control' value='{1}' style='float:left; margin:auto' />&nbsp;\n";
                    if (typeof (value) == "string") {
                        html = html.format("text", value);

                    }
                    else if (typeof (value) == "number") {
                        html = html.format("number", value);
                    }

                    cell.innerHTML = html;
                //}
            }
            
        }
    }

    $scope.updateStoreInfo = function () {
        $scope.updateTableSettings();

        //if ($scope.userInfoType == '' || $scope.identifierText == '') return;

        let data_map = {
            'msg': 'updateStoreInfo',
            'runMode': $scope.runMode,
            'tableName': $scope.tableName,
            'settings': $scope.updateTableSettings(),
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

        $scope.post('/table/viewStoreSchedule/', data_map, function (data) {
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
            html = "<input type='checkbox' class='form-control' name='isChecked[]' value='{0}'  />".format(tr_count);
            var cell = tr.insertCell(td_count++);
            cell.innerHTML = html;

            for (let key in data) {
                let value = data[key];

                var cell = tr.insertCell(td_count++);
    
                    html = "<input type='{0}' class='form-control' value='{1}' style='float:left; margin:auto; font-size:10px;' />&nbsp;\n";
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

    


});