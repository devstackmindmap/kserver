var express = require('express');
var http = require('http');
var path = require('path');
var cors = require('cors');
var fs = require('fs');
var https = require('https');
var bodyParser = require('body-parser');
//const swaggerJSdoc = require('swagger-jsdoc');



var app = express();
app.use(cors());
app.set('sport', process.env.PORT || 443);
app.set('port', process.env.PORT || 3000);
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'ejs');
app.use(express.urlencoded({
    extended: true
}));
app.use(express.json());


//var swaggerDefinition = {
//  info: { // API informations (required)
//    title: 'Hello World', // Title (required)
//    version: '1.0.0', // Version (required)
//    description: 'A sample API', // Description (optional)
//  },
//  host: 'localhost:3001', // Host (optional)
//  basePath: '/', // Base path (optional)
//}

//// Options for the swagger docs
//var options = {
//  // Import swaggerDefinitions
//  swaggerDefinition: swaggerDefinition,
//  // Path to the API docs
//  apis: ['app.js'],
//}

//// Initialize swagger-jsdoc -> returns validated swagger spec in json format
//var swaggerSpec = swaggerJSdoc(options)



var Inapp = require('./index');

app.post('/inappcheck',Inapp.check);
//app.post('/qooappcall',Inapp.qooapp);
app.post('/admob',Inapp.admob);
app.get('/admob',Inapp.admob);



http.createServer(app).listen(app.get('port'), function () {
    console.log('Express server listening on port ' + app.get('port'));
});




// ssl 통신용
/*   
var cert = {
    key: fs.readFileSync('_wildcard_.1on1.games_20180829MOE7.key.pem'),
    cert: fs.readFileSync('_wildcard_.1on1.games_20180829MOE7.crt.pem'),
    ca: fs.readFileSync('ca-bundle.pem')
};

https.createServer(cert, app).listen(app.get('sport'), function () {
    console.log('Express server listening on port ' + app.get('sport'));
});
*/