//var iap = require('in-app-purchase');
var iap = require('iap');


//var mysql_dbc = require('./info/dbcon')();
//var pool = mysql_dbc.init();

var key = require("./info/api-5575175483089366631-47188-772b17746c68.json")





exports.check = function (req, res, next) {
    req.accepts('application/json');
    var json = req.body;

    //console.log(json);

    

    console.log(json)
    console.log(json[0].userID)
    //iap.config({
    //    packageName : "com.AKAStudio.Prototype_B",
    //    googlePublicKeyPath: './info/',
    //    googleServiceAccount: {
    //    clientEmail: 'knightrun@api-5575175483089366631-47188.iam.gserviceaccount.com',
    //    privateKey: 'api-5575175483089366631-47188-772b17746c68.json'
    //},
    //});



    

    let platform = "google";
    let payment = {
        receipt: json[0].purchaseToken,
        productId: json[0].productID, 
        packageName: "com.AKAStudio.Prototype_B",
        keyObject: key,
        subscription: false,
    };



    // As of v1.4.0+ .validate and .validateOnce detects service automatically from the receipt

    //iap.validate(receipt, function (error, response) {
    //iap.validate(platform, json, function (error, response) {
    //iap.validate(platform, json.Payload, function (error, response) {
    iap.verifyPayment(platform, payment, function (error, response) {
        if (error) {
            // Failed to validate
            //console.log(error);
            let result = { Result: 3};
            res.json(result);
            return;
        }
        if (response) {
            //Succuessful validation
            console.log(response)

            if (response.receipt.purchaseState != 0) {
                //정상적이지 않은 상태.
                let result = {
                    Result: 5
                }
                res.json(result);
                return;
            }

            var userID = json[0].userID;
            var productId = response.productId;
            var purchaseToken = json[0].purchaseToken;
            var transactionId = response.transactionId;
            var platform = response.platform;
            
            

            pool.query("select purchaseToken from purchase_log where purchaseToken = ?;", [purchaseToken], function (err, rows, fields) {

                if (rows.length > 0) {
                    let result = {
                        Result: 1
                    };
                    res.json(result);
                    return;
                }

        
                pool.query("select productID, packageName, rewardType, rewardAmount from purchase_item where productID = ?;", [productId], function (err, rows, fields) {
                    if (err) {
                        let result = {
                            Result: 4
                        };

                        res.json(result)
                        return;
                    }
                    
                    if (rows.length < 1) {
                        let result = { Result: 4};

                        res.json(result)
                        return;
                    }
                    
                    let _productID = rows[0].productID;
                    let _packageName = rows[0].packageName;
                    let _rewardType = rows[0].rewardType;
                    let _rewardAmount = rows[0].rewardAmount;
                    
                    pool.beginTransaction();
                    //pool.query("update users set gem = gem + ifnull((select rewardAmount from  purchase_item where productID = ?),0) where userID = ?;", [productId, userID], function (err, rows, fields) {
                    pool.query("update users set gem = gem + ? where userID = ?", [_rewardAmount, userID], function (err, rows, fields) {

                        if (err) console.log(err);
                        console.log(rows)
                        if (rows.affectedRows == 1 && rows.changedRows == 1) {
                            pool.query("insert into purchase_log(UserID, productID, purchaseToken, transactionId) values(?,?,?,?);", [userID, _productID, purchaseToken, transactionId], function (err, row, fields) {
                                if (err) {
                                    console.log(err);
                                    pool.rollback();
                                }
                                pool.query("select gem from users where userID = ?;",[userID], function(err, rows, fields){
                                
                                let result = { Result:0,
                                               Gems : rows[0].gem
                                             };
                                res.json(result);
                                pool.commit();
                                return;
                                })
                            });
                        } else {
                            let result = { Result: 2 };
                            res.json(result);
                            pool.rollback();
                            return;
                        };

                    });
                });
            });
        };
    });
};



/*
post할 데이터
{
	"userID": 1,
	"purchaseToken" : "idbpbifoejhcebdlchafogah.AO-J1OykKTdMA3awtnddgXvkvwBLzMnd0bWpiBGayToKZ225oLFJ_tr3NLjgE-tju6Lesc16QGCfzNBRj1G5hY_3L_jlcTNZBSGNe5QrcndrAwdcye5gAqvy8FdFpoHh6XyJZkAHfm2H",
	"productID" : "1000won"
}
*/
