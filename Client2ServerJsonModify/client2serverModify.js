var fs = require('fs');
var info = require('/Contentdata/renewServerInfo/Client2ServerInfo.json');
//var info = require('./Client2ServerInfo2.json');

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


//var obj = xlsx.parse(args.file); // parses a file
console.log("Mode Check : "+args)
var ServerMode = args.ServerMode;



if (ServerMode == "Dev1") {
    console.log("BeforeVersion : " + info.Servers.Android.Dev1.Version)
    info.Servers..Android.Dev1.Version++;
    console.log("AfterVersion : " + info.Servers.Android.Dev1.Version)
} else if (ServerMode == "Dev2") {
    console.log("BeforeVersion : " + info.Servers.Android.Dev2.Version)
    info.Servers.Android.Dev2.Version++;
    console.log("AfterVersion : " + info.Servers.Android.Dev2.Version)
} else if (ServerMode == "Business") {
    console.log("BeforeVersion : " + info.Servers.Android.Business.Version)
    info.Servers.Android.Business.Version++;
    console.log("AfterVersion : " + info.Servers.Android.Business.Version)
} else if (ServerMode == "Live") {
    console.log("BeforeVersion : " + info.Servers.Android.Live.Version)
    info.Servers.Android.Live.Version++;
    console.log("AfterVersion : " + info.Servers.Android.Live.Version)
}

fs.writeFile("/Contentdata/renewServerInfo/Client2ServerInfo.json", JSON.stringify(info), function (err) {
    if (err) throw err
})
console.log(JSON.stringify(info))
