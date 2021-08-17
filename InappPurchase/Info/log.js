var mysql_dbc = require('./dbcon')();
var logpool = mysql_dbc.log();
var logger = require('../logger');


exports.login = function (userkey, paidCrystal, freeCrystal, send, change, blackhole, rebatch) {

    logpool.query('insert into login(userkey, paidCrystal, freeCrystal, send, changeworker, blackhole, rebatch, log_date) values(?,?,?,?,?,?,?,now())', [userkey, paidCrystal, freeCrystal, send, change, blackhole, rebatch], function (err, rows, fields) {
        if (err) logger.error(err);
    })

    return;

};

exports.useitem = function (userkey, ItemCode, beforeitem, afteritem, achi_count) {

    logpool.query('insert into itemuse(userkey, ItemCode, beforeitem, afteritem, achi_count, log_date) values(?,?,?,?,?,now());', [userkey, ItemCode, beforeitem, afteritem, achi_count], function (err, rows, fields) {

        if (err) logger.error(err);
    })
    return;
};


exports.buyitem = function (userkey, itemcode, paidCrystal, aft_paidCrystal, FreeCrystal, aft_freeCrystal, useCrystal, bef_amount, amount) {
    //console.log(userkey+":"+ itemcode+":"+ paidCrystal+":"+ aft_paidCrystal+":"+ FreeCrystal+":"+ aft_freeCrystal+":"+ useCrystal+":"+ bef_amount+":"+ amount);
    logpool.query('insert into buyitem(userkey, itemcode, beforePaid, afterPaid, beforeFree, afterFree, usedCrystal, beforeItem, afterItem, log_date) values(?, ?, ?, ?, ?, ?, ?, ?, ?, now());', [userkey, itemcode, paidCrystal, aft_paidCrystal, FreeCrystal, aft_freeCrystal, useCrystal, bef_amount, amount], function (err, rows, fields) {

        if (err) logger.error(err);
    })
    return;
}


exports.setSupply = function (userkey, planetID, itemcode, status) {

    logpool.query('insert into setsupply(userkey, planetID, itemcode, status, log_date) values(?, ?, ?, ?, now());', [userkey, planetID, itemcode, status], function (err, rows, fields) {
        //if(err) console.log(err);
        if (err) logger.error(err);
    })
    return;
}


exports.clearsupply = function (userkey, lastplanetID, activeplanetID, status) {
    logpool.query('insert into clearsupply(userkey, lastPlanetID, activeplanetID, status, log_date) values(?, ?, ?, ?, now());', [userkey, lastplanetID, activeplanetID, status], function (err, rows, fields) {
        if (err) console.log(err);
        //if(err) logger.error(err);

    })
}


exports.lifesupply = function (userkey, planetID, alien, status) {
    logpool.query('insert into lifesupply(userkey, planetID, alienNum, status, log_date) values(?, ?, ?, ?, now());', [userkey, planetID, alien, status], function (err, rows, fields) {
        if (err) console.log(err);
        //if(err) logger.error(err);  
    })
};



exports.stage = function (userkey, status, planetID, stageID, exp, complete, overexp) {
    logpool.query('insert into stage(userkey, status, planetID, stageID, exp, complete, overexp, log_date) values(?, ?, ?, ?, ?, ?, ?, now());', [userkey, status, planetID, stageID, exp, complete, overexp], function (err, rows, fields) {
        if (err) console.log(err);
        //if(err) logger.error(err); 
    })
}

exports.reward = function (userkey, adkind, freecrystal, rebatch) {
    logpool.query('insert into reward(userkey, adkind, freecrystal, rebatch,log_date) values(?, ?, ?, ?, now());', [userkey, adkind, freecrystal, rebatch], function (err, rows, fields) {
        if (err) console.log(err);
        if (err) logger.error(err)
    })
};



exports.inapp = function(userkey, paymentID, productID, purchaseTime, purchaseToken, PaidCrystal, FreeCrystal, bonus_1, bonus_2, bonus_3, bonus_4) {
    logpool.query('insert into inapp_log(userkey, paymentID, productID, purchaseTime, purchaseToken, PaidCrystal, FreeCrystal, bonus_1, bonus_2, bonus_3, bonus_4, log_date) values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, NOW());',[userkey, paymentID, productID, purchaseTime, purchaseToken, PaidCrystal, FreeCrystal, bonus_1, bonus_2, bonus_3, bonus_4], function(err, rows, fields){
        if(err) console.log(err);
        //if(err) logger.error(err); 
    })
}



exports.inappCheck = function(purchase_token, callback) {
    //const foo ='';
    logpool.query('select purchaseToken from inapp_log where purchaseToken = ? limit 1',[purchase_token], function(err, rows, fields) {

        if(err) console.log(err);

        if(rows.length == 0) {
            callback('true');
        } else{
            callback('false');
        } 

    })

};


exports.inappad = function(orderid, productID, purchaseTime, PaidCrystal, FreeCrystal, bonus_1, bonus_2, bonus_3, bonus_4) {
    logpool.query('insert into inapp_adlog(orderID, productID, purchaseTime, PaidCrystal, FreeCrystal, bonus_1, bonus_2, bonus_3, bonus_4, log_date) values(?, ?, ?, ?, ?, ?, ?, ?, ?, NOW());',[orderid, productID, purchaseTime, PaidCrystal, FreeCrystal, bonus_1, bonus_2, bonus_3, bonus_4], function(err, rows, fields){
        if(err) console.log(err);
        //if(err) logger.error(err); 
    })
}



exports.inappCheckad = function(orderID, callback) {
    //const foo ='';
    logpool.query('select orderID from inapp_adlog where orderID = ? limit 1',[orderID], function(err, rows, fields) {

        if(err) console.log(err);

        if(rows.length == 0) {
            callback('true');
        } else{
            callback('false');
        } 

    })

};


