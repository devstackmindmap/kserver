krApp.controller('userInfoController', function ($scope, $http) {
    $scope.runMode = '';
    $scope.shopInfoType = '';
    $scope.identifierType = 'nickName';
    $scope.identifierText = '';
    $scope.header_names = [];
    $scope.datas = [];
    $scope.isAllChoiceCheck = false;

    $scope.normal_key_map = {
        //'allList': ['seq'],
        //'productsText': ['productId', 'languageType'],
        //'products': ['seq'],
        //'rewards': ['seq'],
        //'items': ['seq'],
        //'eventDigital': ['productId'],
        //'eventReal': ['productId','platform'],
        //'fixDigital': ['productId'],
        //'fixReal': ['productId', 'platform'],
        //'userDigital': ['productId'],
        //'userReal': ['productId' ]

    }

    $scope.datetime_key_map = {

        'allList': ['startDateTime', 'endDateTime'],
        'productsText': ['productId', 'languageType'],
        'products': ['seq'],
        'rewards': ['seq'],
        'items': ['seq'],
        'eventDigital': ['productId'],
        'eventReal': ['productId', 'platform'],
        'fixDigital': ['productId'],
        'fixReal': ['productId', 'platform']

    }



    $scope.getUserInfoSettings = function () {
        return {
            "runMode": $scope.runMode,
            "identifierType": $scope.identifierType,
            "identifierText": $scope.identifierText,
            "shopInfoType": $scope.shopInfoType
        };
    }

    $scope.addShopInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.shopInfoType == '' ) return;

        let data_map = {
            'settings': $scope.getUserInfoSettings(),
            'updateMap': {}
        }

        if ($scope.shopInfoType == 'accounts') {

        }
        else if ($scope.shopInfoType == 'allList') {
            let productId = $('#productId').val();
            let aosStoreProductId = $('#aosStoreProductId').val();
            let iosStoreProductId = $('#iosStoreProductId').val();
            let startDateTime = $('#startDateTime').val();
            let endDateTime = $('#endDateTime').val();
            let productTableType = $('#productTableType').val();
            let storeType = $('#storeType').val();
            let productType = $('#productType').val();
            let countOfPurchases = $('#countOfPurchases').val();
            let materialType = $('#materialType').val();
            let cost = $('#cost').val();
            let saleCost = $('#saleCost').val();

            data_map['updateMap'] = {
                'productId': productId,
                'aosStoreProductId': aosStoreProductId,
                'iosStoreProductId': iosStoreProductId,
                'startDateTime': startDateTime,
                'endDateTime': endDateTime,
                'productTableType': productTableType,
                'storeType': storeType,
                'productType': productType,
                'countOfPurchases': countOfPurchases,
                'materialType': materialType,
                'cost': cost,
                'saleCost': saleCost
            };

        }
        else if ($scope.shopInfoType == 'productsText') {
            let productId = $('#productId').val();
            let languageType = $('#languageType').val();
            let productText = $('#productText').val();

            data_map['updateMap'] = {
                'productId': productId,
                'languageType': languageType,
                'productText': productText
            };
        }
        else if ($scope.shopInfoType == 'products') {
            let productId = $('#productId').val();
            let rewardType = $('#rewardType').val();
            let rewardId = $('#rewardId').val();

            data_map['updateMap'] = {
                'productId': productId,
                'rewardType': rewardType,
                'rewardId': rewardId
            };
        }
        else if ($scope.shopInfoType == 'rewards') {
            let rewardId = $('#rewardId').val();
            let itemId = $('#itemId').val();

            data_map['updateMap'] = {
                'rewardId': rewardId,
                'itemId': itemId
            };
        }
        else if ($scope.shopInfoType == 'items') {
            let itemId = $('#itemId').val();
            let itemType = $('#itemType').val();
            let id = $('#id').val();
            let minCount = $('#minCount').val();
            let MaxCount = $('#MaxCount').val();
            let probability = $('#probability').val();

            data_map['updateMap'] = {
                'itemId': itemId,
                'itemType': itemType,
                'id': id,
                'minCount': minCount,
                'MaxCount': MaxCount,
                'probability': probability
            };
        }
        else if ($scope.shopInfoType == 'eventDigital') {
            let productId = $('#productId').val();
            let startDateTime = $('#startDateTime').val();
            let endDateTime = $('#endDateTime').val();
            let storeType = $('#storeType').val();
            let productType = $('#productType').val();
            let materialType = $('#materialType').val();
            let saleCost = $('#saleCost').val();
            let cost = $('#cost').val();
            let countOfPurchases = $('#countOfPurchases').val();

            data_map['updateMap'] = {
                'productId': productId,
                'startDateTime': startDateTime,
                'endDateTime': endDateTime,
                'storeType': storeType,
                'productType': productType,
                'materialType': materialType,
                'saleCost': saleCost,
                'cost': cost,
                'countOfPurchases': countOfPurchases
            };
        }
        else if ($scope.shopInfoType == 'eventReal') {
            let productId = $('#productId').val();
            let startDateTime = $('#startDateTime').val();
            let endDateTime = $('#endDateTime').val();
            let platform = $('#platform').val();
            let storeProductId = $('#storeProductId').val();
            let storeType = $('#storeType').val();
            let productType = $('#productType').val();
            let saleCost = $('#saleCost').val();
            let cost = $('#cost').val();
            let countOfPurchases = $('#countOfPurchases').val();

            data_map['updateMap'] = {
                'productId': productId,
                'startDateTime': startDateTime,
                'endDateTime': endDateTime,
                'platform': platform,
                'storeProductId': storeProductId,
                'storeType': storeType,
                'productType': productType,
                'saleCost': saleCost,
                'cost': cost,
                'countOfPurchases': countOfPurchases
            };
        }
        else if ($scope.shopInfoType == 'fixDigital') {
            let productId = $('#productId').val();
            let storeType = $('#storeType').val();
            let productType = $('#productType').val();
            let materialType = $('#materialType').val();
            let cost = $('#cost').val();

            data_map['updateMap'] = {
                'productId': productId,
                'storeType': storeType,
                'productType': productType,
                'materialType': materialType,
                'cost': cost
            };
        }
        else if ($scope.shopInfoType == 'fixDigital') {
            let productId = $('#productId').val();
            let platform = $('#platform').val();
            let storeProductId = $('#storeProductId').val();
            let storeType = $('#storeType').val();
            let productType = $('#productType').val();
            let cost = $('#cost').val();

            data_map['updateMap'] = {
                'productId': productId,
                'platform': platform,
                'storeProductId': storeProductId,
                'storeType': storeType,
                'productType': productType,
                'cost': cost
            };
        }
        else if ($scope.shopInfoType == 'userDigital') {
            let productId = $('#productId').val();
            let saleDurationHour = $('#saleDurationHour').val();
            let storeType = $('#storeType').val();
            let productType = $('#productType').val();
            let materialType = $('#materialType').val();
            let saleCost = $('#saleCost').val();
            let cost = $('#cost').val();
            let countOfPurchases = $('#countOfPurchases').val();
            

            data_map['updateMap'] = {
                'productId': productId,
                'saleDurationHour': saleDurationHour,
                'storeType': storeType,
                'productType': productType,
                'materialType': materialType,
                'saleCost': saleCost,
                'cost': cost,
                'countOfPurchases': countOfPurchases
            };
        }
        else if ($scope.shopInfoType == 'userReal') {
            let productId = $('#productId').val();
            let platform = $('#platform').val();
            let storeProductId = $('#storeProductId').val();
            let saleDurationHour = $('#saleDurationHour').val();
            let storeType = $('#storeType').val();
            let productType = $('#productType').val();
            let saleCost = $('#saleCost').val();
            let cost = $('#cost').val();
            let countOfPurchases = $('#countOfPurchases').val();


            data_map['updateMap'] = {
                'productId': productId,
                'platform': platform,
                'storeProductId': storeProductId,
                'saleDurationHour': saleDurationHour,
                'storeType': storeType,
                'productType': productType,
                'saleCost': saleCost,
                'cost': cost,
                'countOfPurchases': countOfPurchases
            };
        }

        $scope.post('/shop/addShopInfo/', data_map, function (data) {
            alert('succeed to add Shop infomation');
        }, function (res) {
            alert('failed to add Shop infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }
    
    
    $scope.onshopInfoTypeChange = function (selectedshopInfoType) {
        $scope.shopInfoType = selectedshopInfoType;
    }

    $scope.getShopInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.shopInfoType == '' ) return;

        let data_map = {
            'msg': 'getShopInfo',
            'settings': $scope.getUserInfoSettings()
        }

        $scope.post('/shop/getShopInfo/', data_map, function (data) {
            $scope.datas = data;
            $scope.header_names = Object.keys($scope.datas[0])
            $scope.makeElementHtmlOfUserInfo($scope.datas)
        }, function (res) {
            alert('failed to get Shop infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }


    $scope.updateShopInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.updateShopInfo == '') return;
        
        let data_map = {
            'msg': 'updateShop',
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

        $scope.post('/shop/getShopInfo/', data_map, function (data) {
            alert('succedd to update Shop infomation');
        }, function (res) {
            alert('failed to update Shop infomation, {0}, {1}'.format(res.status, res.statusText));
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
                if ($scope.shopInfoType in $scope.normal_key_map && $scope.normal_key_map[$scope.shopInfoType].indexOf(key) >= 0) {
                    html = "<span style='font-size:11px; margin:auto;'> {0} </span>"
                    html = html.format(value);
                    cell.innerHTML = html;
                }
                //else if ($scope.shopInfoType in $scope.datetime_key_map && $scope.datetime_key_map[$scope.shopInfoType].indexOf(key) >= 0) {

                //    html = "<input type='{0}' class='form-control' value='{1}' style='float:left; margin:auto; font-size:9pt' />&nbsp;\n";

                //    if (typeof (value) == "string") {
                //        var date = new Date(value).toISOString()
                //        var datelocal = date.slice(0, 16)
                //        html = html.format("datetime-local", datelocal);

                //    }
                //    else if (typeof (value) == "number") {
                //        html = html.format("number", value);
                //    }

                //    cell.innerHTML = html;
                //}
                else {               
                    html = "<input type='{0}' class='form-control' value='{1}' style='float:left; margin:auto; font-size:10px;' />";    
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

    $scope.onShopInfoTypeChange = function (selectedShopInfoType) {
        $scope.shopInfoType = selectedShopInfoType;
    }

    $scope.updateUserInfoSettings = function () {
        $scope.runMode = $('#runMode').val();
        $scope.shopInfoType = $('#shopInfoType').val();
        //$scope.identifierType = $('#identifierType').val();
        //$scope.identifierText = $('#identifierText').val();
    }

    $scope.deleteShopInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.shopInfoType == '') return;

        var trIndexList = $("input[type='checkbox'][name='isChecked[]']:checked").map(function () {
            return this.value;
        }).get();

        deleteList = [];
        trIndexList.forEach(function (tr_index) {
            deleteList.push($scope.datas[tr_index]);
        });

        let data_map = {
            'msg': 'deleteShopInfo',
            'settings': $scope.getUserInfoSettings(),
            'deleteList': deleteList
        };

        $scope.post('/shop/getShopInfo/', data_map, function (data) {
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

            alert('succedd to delete Shop infomation');
        }, function (res) {
            alert('failed to delete Shop infomation, {0}, {1}'.format(res.status, res.statusText));
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

        if ($scope.shopInfoType == '' || $scope.identifierText == '') return;

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


    $scope.refreshShopInfo = function () {
        $scope.updateUserInfoSettings();

        if ($scope.updateShopInfo == '') return;

        let data_map = {
            'msg': 'refreshShop',
            'settings': $scope.getUserInfoSettings(),
            'updateList': []
        };


        $scope.post('/shop/refreshShopInfo/', data_map, function (data) {
            alert('succedd to refresh Shop infomation');
        }, function (res) {
            alert('failed to refresh Shop infomation, {0}, {1}'.format(res.status, res.statusText));
        });
    }





});