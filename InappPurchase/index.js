//var iap = require('in-app-purchase');
var iap = require('iap');
var fs = require('fs');

const crypto = require('crypto');

var mysql_dbc = require('./Info/dbcon')();
var pool = mysql_dbc.init();

var key = require("./Info/api-5575175483089366631-47188-772b17746c68.json")

var PUBKEY = "-----BEGIN PUBLIC KEY-----\n" +
    "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAw+nDAnWmAghc6LXmVXON\n" +
    "GvYF4dwaenQM85dc0pqeMOw6bgN3sg4NU4JB25ymSf+4nbSBnQVjtMivyNvyEAHD\n" +
    "W90UJtaUKxcv2HdzD5YsYy5T4olsR6WD85c0JydXuuofBxxTddftIAYifRfC+Zsz\n" +
    "HhOdE63QiFJ2HikENaGsZxOCRriVeyAe6VKj8Mq3lvD5AiTjbqIRir/HA8zdd8sB\n" +
    "+ec5rapy74S5bshu6aruDPcYBpgjxSAMspo1f6yC1rhqGvKOGrappVxsjxPQstWK\n" +
    "c5wJeTS7IySNcIg5DRUe6SaWLTqclffPo3Yl9ubyNoT/aZgysKg3c14aW4nV4I1j\n" +
    "1QIDAQAB\n" +
    "-----END PUBLIC KEY-----"

var pubkey = require("./Info/qookey.json")
var admobkeys = require("./Info/admobkeys.json")
var adkey = admobkeys.keys
console.log(adkey[0].keyId)

pubK = fs.readFileSync('./Info/pub.key').toString();


function privENC_pubDEC(originMSG) {

    msg = crypto.publicDecrypt(PUBKEY, Buffer.from(originMSG, 'base64'));

    console.log(msg.toString());
}




exports.admob = function (req, res, next) {
    req.accepts('application/json');
    var json = req.body;

    var qjson = req.query

    ad_network = qjson.ad_network;
    ad_unit = qjson.ad_unit;
    reward_amount = qjson.reward_amount;
    reward_item = qjson.reward_item;
    timestamp = qjson.timestamp;
    transaction_id = qjson.transaction_id;
    user_id = qjson.user_id;
    signature = qjson.signature;
    key_id = qjson.key_id;




    //{"keys":[{"keyId":3335741209,"pem":"-----BEGIN PUBLIC KEY-----\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE+nzvoGqvDeB9+SzE6igTl7TyK4JB\nbglwir9oTcQta8NuG26ZpZFxt+F2NDk7asTE6/2Yc8i1ATcGIqtuS5hv0Q==\n-----END PUBLIC KEY-----","base64":"MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE+nzvoGqvDeB9+SzE6igTl7TyK4JBbglwir9oTcQta8NuG26ZpZFxt+F2NDk7asTE6/2Yc8i1ATcGIqtuS5hv0Q=="}]}

    pool.beginTransaction();
    pool.query("INSERT INTO account.ad_view_log(ad_network, ad_unit, reward_amount, reward_item, timestamp, transaction_id, user_id, signature, key_id) values(?,?,?,?,?,?,?,?,?);", [ad_network, ad_unit, reward_amount, reward_item, timestamp, transaction_id, user_id, signature, key_id], function (err, rows, fields) {
        if (err) {
            console.log(err)
            pool.rollback();
            let result = {
                "Result": 0,
            };
            res.json(result)
            return;

        }
        adkey.forEach(function (AkeyId) {
            
            if(AkeyId.keyId == key_id) {
                let result = {
                    "Result": 1,
                    "keys": AkeyId.keyId
                };
                
                msg = crypto.publicDecrypt(AkeyId.pem, Buffer.from(signature, 'base64'));
                
                console.log(msg)
                res.json(result)
                pool.commit();
                return;
            }
        })
        

    })


}





/*
exports.qooapp = function (req, res, next) {
    req.accepts('application/json');
    var json = req.body;
    //console.log(json)
    
    payload = json.data;
    signat = base64decode(json.signature);

    msg = crypto.publicDecrypt(pubK, Buffer.from(signat, 'base64'));
    
    
    
    res.json("ok")
}
*/

exports.check = function (req, res, next) {
    req.accepts('application/json');
    var json = req.body;
    console.log(json)

    if (json.platform == 1) { //호출된 플랫폼 값이 1 은 google 2는 apple
        let platform = "google";
        let payment = {
            receipt: json.purchaseToken,
            productId: json.productID,
            packageName: "com.AKAStudio.KnightRun",
            keyObject: key,
            subscription: false,
        };



        // As of v1.4.0+ .validate and .validateOnce detects service automatically from the receipt

        iap.verifyPayment(platform, payment, function (error, response) {
            if (error) {
                // Failed to validate
                console.log(error)
                let result = {
                    "Result": 3,
                    "transactionId": null,
                    "purchaseState": null,
                    "purchaseDate": null,
                    "expirationDate": null
                };
                let rs = {
                    receipt: [result]
                }
                res.json(rs)

                return;
            }
            if (response) {
                //Succuessful validation
                //console.log(response)

                if (response.receipt.purchaseState != 0) {
                    //정상적이지 않은 상태.
                    let result = {
                        "Result": response.receipt.purchaseState,
                        "transactionId": null,
                        "purchaseState": null,
                        "purchaseDate": null,
                        "expirationDate": null
                    }
                    let rs = {
                        receipt: [result]
                    }
                    res.json(rs)
                    return;
                }


                var productId = response.productId;
                var purchaseToken = json.purchaseToken;
                var transactionId = response.transactionId;
                var platform = response.platform;
                var purchaseState = response.receipt.purchaseState;
                var purchaseDate = response.purchaseDate;
                var expirationDate = response.expirationDate;

                if (transactionId != json.transaction_id) {
                    let result = {
                        "Result": 5,
                        "transactionId": null,
                        "purchaseState": null,
                        "purchaseDate": null,
                        "expirationDate": null
                    }
                    let rs = {
                        receipt: [result]
                    }
                    res.json(rs)
                    return;
                }

                let result = {
                    "Result": 0,
                    "transactionId": transactionId,
                    "productId": productId,
                    "purchaseState": purchaseState,
                    "purchaseDate": purchaseDate,
                    "expirationDate": expirationDate
                };
                let rs = {
                    receipt: [result]
                }
                res.json(rs)

                return;



                /*
                                pool.query("select purchaseToken from purchase_log where purchaseToken = ?;", [purchaseToken], function (err, rows, fields) {

                                    if (rows.length > 0) {
                                        let result = {
                                            "Result": 2,
                                            "transactionId": transactionId,
                                            "productId": productId,
                                            "purchaseState": purchaseState,
                                            "purchaseDate": purchaseDate,
                                            "expirationDate": expirationDate
                                        };
                                        let rs = {
                                            receipt: [result]
                                        }
                                        res.json(result)
                                        return;
                                    }


                                    pool.beginTransaction();
                                    pool.query("INSERT INTO purchase_log(UserID, productId, purchaseToken, transactionId, platform) values(?,?,?,?,?);", [json.userId, productId, purchaseToken, transactionId, platform], function (err, rows, fields) {
                                        if (err) {
                                            console.log(err)
                                            pool.rollback();
                                            let result = {
                                                "Result": 4,
                                                "transactionId": transactionId,
                                                "productId": productId,
                                                "purchaseState": purchaseState,
                                                "purchaseDate": purchaseDate,
                                                "expirationDate": expirationDate
                                            };
                                            let rs = {
                                                receipt: [result]
                                            }
                                            res.json(result)
                                            return;

                                        }
                                        let result = {
                                            "Result": 0,
                                            "transactionId": transactionId,
                                            "productId": productId,
                                            "purchaseState": purchaseState,
                                            "purchaseDate": purchaseDate,
                                            "expirationDate": expirationDate
                                        };
                                        let rs = {
                                            receipt: [result]
                                        }
                                        res.json(result)
                                        pool.commit();
                                        return;
                                    })
                                })
                                */
            };
        });
    } else if (json.platform == 2) {
        //        console.log(json)
        let platform = "apple";
        let payment = {
            receipt: json.purchaseToken,
            productId: json.productID,
            packageName: "com.akastudiocorp.knightrun",
            excludeOldTransactions: false
        };

        //      console.log(payment)
        iap.verifyPayment(platform, payment, function (error, response) {

            if (error) {

                console.log(error);
                if (!error.status) error.status = 5
                let result = {
                    "Result": error.status,
                    "transactionId": null,
                    "purchaseState": null,
                    "purchaseDate": null,
                    "expirationDate": null
                };
                let rs = {
                    receipt: [result]
                }
                res.json(rs)
                return;
            }


            if (response) {
                //Succuessful validation


                if (response.receipt.in_app == null) {
                    //정상적이지 않은 상태.
                    let result = {
                        "Result": 21100,
                        "transactionId": null,
                        "purchaseState": null,
                        "purchaseDate": null,
                        "expirationDate": null
                    }
                    let rs = {
                        receipt: [result]
                    }
                    res.json(rs)
                    return;
                }


                in_app = response.receipt.in_app;

                addReturn(in_app, json, platform, res);
                /*
                                var notVaildate = IOSvaildate(in_app, json, platform, res);

                                if(notVaildate == 1) {
                                    let result = {
                                        "Result": 3,
                                        "transactionId": null,
                                        "purchaseState": null,
                                        "purchaseDate": null,
                                        "expirationDate": null
                                    }

                                    res.json(result)
                                }


                            } else {

                                let result = {
                                    "Result": 3,
                                    "transactionId": null,
                                    "purchaseState": null,
                                    "purchaseDate": null,
                                    "expirationDate": null
                                };
                                
                                res.json(result)
                */
            }

        });
    };
};




function base64decode(base64text) {
    return Buffer.from(base64text, 'base64').toString('utf-8');
}


async function addReturn(in_app, json, platform, res) {
    var result = []
    for (var i = 0; i < in_app.length; i++) {

        var result1 = await DataAdd(in_app[i], json, platform)
        result.push(result1)
        if (result.length == in_app.length) {
            if (in_app.length == 1) {
                let rs = {
                    receipt: result
                }
                res.json(rs)
            } else {
                let rs = {
                    receipt: result
                }

                res.json(rs)
            }
        }
    }

}


async function IOSvaildate(in_app, json, platform, res) {

    var result = []
    var i = await transationIdChecker(in_app, json);

    if (i != 65535) {
        result = await DataAdd(in_app[i], json, platform)
    } else {
        result = {
            "Result": 5,
            "transactionId": null,
            "purchaseState": null,
            "purchaseDate": null,
            "expirationDate": null
        }
    }

    res.json(result)

}


function transationIdChecker(in_app, json) {
    return new Promise(function (resolve, reject) {
        let ret = 65535;

        for (var i = 0; i < in_app.length; i++) {

            if (in_app[i].transaction_id == json.transaction_id) {
                ret = i;
                resolve(ret)
            } else {
                console.log(i + ' ' + length)
                if (i == (in_app.length - 1)) {
                    resolve(ret)
                }
            }
        }

    })
}

function base64encode(plaintext) {
    return Buffer.from(plaintext, "utf8").toString('base64');
}



function DataAdd(inapp, json, platform) {
    return new Promise(function (resolve, reject) {

        var result = []
        var productId = inapp.product_id;
        var transactionId = inapp.transaction_id;
        var purchaseDate = inapp.purchase_date;
        var expirationDate = inapp.purchase_date_ms;
        //var purchaseToken = json.purchaseToken;
        var purchaseToken = "apple";
        var purchaseState = 0;



        let temp = {
            "Result": 0,
            "transactionId": transactionId,
            "productId": productId,
            "purchaseState": purchaseState,
            "purchaseDate": purchaseDate,
            "expirationDate": expirationDate
        };
        resolve(temp)

        /*
        pool.query("select transactionId from purchase_log where transactionId = ?;", [transactionId], function (err, rows, fields) {

            if (rows.length > 0) {
                let temp = {
                    "Result": 2,
                    "transactionId": transactionId,
                    "productId": productId,
                    "purchaseState": purchaseState,
                    "purchaseDate": purchaseDate,
                    "expirationDate": expirationDate
                };
                //result.push(temp)
                //res.json(temp);
                result.push(temp)
                resolve(temp)


            } else {


                pool.beginTransaction();
                pool.query("INSERT INTO purchase_log(UserID, productId, purchaseToken, transactionId, platform) values(?,?,?,?,?);", [json.userId, productId, purchaseToken, transactionId, platform], function (err, rows, fields) {
                    if (err) {
                        console.log(err)
                        pool.rollback();
                        let temp = {
                            "Result": 4,
                            "transactionId": transactionId,
                            "productId": productId,
                            "purchaseState": purchaseState,
                            "purchaseDate": purchaseDate,
                            "expirationDate": expirationDate
                        };

                        //res.json(temp);
                        result.push(temp);
                        resolve(result)


                    }
                    let temp = {
                        "Result": 0,
                        "transactionId": transactionId,
                        "productId": productId,
                        "purchaseState": purchaseState,
                        "purchaseDate": purchaseDate,
                        "expirationDate": expirationDate
                    };
                    
                    
                    result.push(temp)
                    resolve(temp)
                    pool.commit();

                })
            }
        })
*/
    })
}









/*
post할 데이터
{
	"userID": 1,
	"purchaseToken" : "idbpbifoejhcebdlchafogah.AO-J1OykKTdMA3awtnddgXvkvwBLzMnd0bWpiBGayToKZ225oLFJ_tr3NLjgE-tju6Lesc16QGCfzNBRj1G5hY_3L_jlcTNZBSGNe5QrcndrAwdcye5gAqvy8FdFpoHh6XyJZkAHfm2H",
	"productID" : "1000won"
}
*/
