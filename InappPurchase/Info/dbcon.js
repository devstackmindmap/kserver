var mysql = require('mysql');
var akaconfig = require('./db_info').local;
const args = process.argv
    .slice(2)
    .map((val, i) => {
        let object = {};
        let [regexForProp, regexForVal] = (() => [new RegExp('^(.+?)='), new RegExp('\=(.*)')])();
        let [prop, value] = (() => [regexForProp.exec(val), regexForVal.exec(val)])();
        if (!prop) {
            object[val] = true;
            return object;
        } else {
            object[prop[1]] = value[1];
            return object
        }
    })
    .reduce((obj, item) => {
        let prop = Object.keys(item)[0];
        obj[prop] = item[prop];
        return obj;
    }, {});


var runmode = args.runmode;


//var akaconfig = require('');

//console.log(akaconfig.DBSetting.knightrun.host);
//console.log(akaconfig.DBSetting.knightrun.port);
//console.log(akaconfig.DBSetting.knightrun.user);
//console.log(akaconfig.DBSetting.knightrun.password);
//console.log(akaconfig.DBSetting.knightrun.database);
//var log_conf = require('./db_info').log;
module.exports = function () {
  return {
    init: function () {
      return mysql.createConnection({
          host: akaconfig.host,
          port: akaconfig.port,
          user: akaconfig.user,
          password: akaconfig.password,
          database: akaconfig.database,
        connectionLimit:20,
        waitForConnections:false
      })
    },
    log: function () {
        return mysql.createConnection({
          host: log_conf.host,
          port: log_conf.port,
          user: log_conf.user,
          password: log_conf.password,
          database: log_conf.database,
          connectionLimit:20,
          waitForConnections:false
        })
    },
    test_open: function (con) {
      con.connect(function (err) {
        if (err) {
          console.error('mysql connection error :' + err);
        } else {
          console.info('mysql is connected successfully.');

        }
      })
    }
  }
};