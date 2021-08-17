krApp.controller('assetBundleController', function ($scope, $http, $window) {
    $scope.runMode = '';
    $scope.assetBundleType = '';
    $scope.assetBundleRows = [];
    $scope.isAllChoiceCheck = false;
    $scope.changeOrderNumList = [];
    
    $scope.getAssetBundles = function () {
        $scope.updateAssetBundleSettings();

        let data_map = {
            'msg': 'getAssetBundles',
            'runMode': $scope.runMode,
            'assetBundleType': $scope.assetBundleType
        }

        $scope.post('/assetBundle/inquiry/', data_map, function (data) {
           $scope.assetBundleRows = data;
        }, function (res) {
            alert('failed to get asset bundles, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.getLatestAssetBundles = function () {
        $scope.updateAssetBundleSettings();

        let data_map = {
            'msg': 'getLatestAssetBundles',
            'runMode': $scope.runMode
        }

        $scope.post('/assetBundle/inquiry/', data_map, function (data) {
            $scope.assetBundleRows = data;
        }, function (res) {
            alert('failed to get asset bundles, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.getAssetBundleOrderInfo = function () {
        $scope.updateAssetBundleSettings();

        let data_map = {
            'msg': 'getAssetBundleOrderInfo',
            'runMode': $scope.runMode
        }

        $scope.post('/assetBundle/updateOrder/', data_map, function (data) {
            $scope.assetBundleRows = data;
        }, function (res) {
            alert('failed to get asset bundles, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.updateAssetBundleOrder = function () {
        let changedAssetBundleInfoList = [];
        $scope.assetBundleRows.forEach(function (assetBundleInfo) {
            let currentOrderNum = $("input[type='number'][name='{0}']".format(assetBundleInfo.name)).val();

            if (currentOrderNum != assetBundleInfo.orderNum) {
                changedAssetBundleInfoList.push({
                    'name': assetBundleInfo.name,
                    'orderNum': currentOrderNum
                });
            }
        });

        if (changedAssetBundleInfoList.length == 0) {
            return;
        }

        $scope.updateAssetBundleSettings();

        let data_map = {
            'msg': 'updateAssetBundleOrder',
            'runMode': $scope.runMode,
            'updateList': changedAssetBundleInfoList
        };

        $scope.post('/assetBundle/updateOrder/', data_map, function (data) {
            changedAssetBundleInfoList.forEach(function (assetBundleInfo) {
                var data = $scope.assetBundleRows.find((data) => data.name == assetBundleInfo.name);
                data.orderNum = parseInt(assetBundleInfo.orderNum);
            });

            $scope.assetBundleRows.sort(function (data1, data2) {
                return data2.orderNum - data1.orderNum;
            });

            alert('succeed to arrange order of asset bundle');
        }, function (res) {
            alert('failed to get asset bundles, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.checkAllChoiceButton = function () {
        $scope.isAllChoiceCheck = $('#isAllChoiceCheck')[0].checked;

        let choiceCheckBoxes = $('input[name="isChecked[]"]');

        for (let i = 0; i < choiceCheckBoxes.length; i++) {
            let checkbox = choiceCheckBoxes[i];
            checkbox.checked = $scope.isAllChoiceCheck;
        }
    }

    $scope.updateAssetBundleSettings = function () {
        $scope.runMode = $('#runMode').val();
        $scope.assetBundleType = $('#assetBundleType').val();
    }

    $scope.transferAssetBundles = function () {
        let selectedVersionList = $scope.getCheckedAssetBundleVersions();
        if (selectedVersionList.length == 0) {
            return;
        }

        $scope.updateAssetBundleSettings()
        let to_runMode = $('#to_runMode').val();

        let data_map = {
            'msg': 'transferAssetBundles',
            'runMode': $scope.runMode,
            'to_runMode': to_runMode,
            'selectedVersionList': selectedVersionList
        }

        $scope.post('/assetBundle/transfer/', data_map, function (data) {
            $window.location.href = '/assetBundle/updateVersion/'

        }, function (res) {
            alert('failed to transfer asset bundles, {0}, {1}'.format(res.status, res.statusText));
        });
    }

    $scope.getCheckedAssetBundleVersions = function () {
        let selectedVersions = $('input[name="isChecked[]"]:checked').map(function () {
            return this.value;
        }).get();

        return selectedVersions;
    }

    $scope.deleteSelectedAssetBundles = function () {
        if ($scope.assetBundleRows.length == 0) {
            return ;
        }

        let selectedVersions = $scope.getCheckedAssetBundleVersions();
        if (selectedVersions.length == 0) {
            return ;
        }

        $scope.updateAssetBundleSettings();

        let data_map = {
            'msg': 'deleteAssetBundles',
            'runMode': $scope.runMode,
            'deletionVersionList': []
        }

        selectedVersions.forEach(function (version) {
            data_map['deletionVersionList'].push(version);
        });

        $scope.post('/assetBundle/inquiry/', data_map, function (data) {
            if ($scope.isAllChoiceCheck) {
                $('#isAllChoiceCheck')[0].checked = false;
                $scope.checkAllChoiceButton();
            }

            selectedVersions.forEach(function (version) {
                for (let i = $scope.assetBundleRows.length -1; i > -1; i--) {
                    if ($scope.assetBundleRows[i]['version'] == version) {
                        $scope.assetBundleRows.splice(i, 1);     
                        break;
                    }
                }
            });
        }, function (res) {
            alert('failed to delete asset bundles, {0}, {1}'.format(res.status, res.statusText));
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