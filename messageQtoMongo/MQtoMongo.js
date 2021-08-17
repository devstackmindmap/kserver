const fs = require('fs');
const MongoClient = require('mongodb').MongoClient;
const assert = require('assert');
var amqp = require('amqplib/callback_api');

let rawdata = fs.readFileSync('./AkaConfig.json');
let configData = JSON.parse(rawdata);


// Connection URL
const MongoUrl = configData.MongoDBSetting.Url;
const dbName = configData.MongoDBSetting.DBName;
const CollectionName = configData.MongoDBSetting.CollectionName;

//const client = new MongoClient(MongoUrl, configData.MongoDBSetting.Port);
const client = new MongoClient(MongoUrl);



// Connection Rabbit
const RabbitUrl = configData.RabbitMQSetting.Url;
const QueueName = configData.RabbitMQSetting.QueueName;
const exchangeName = configData.RabbitMQSetting.exchangeName;
const RoutingKey = configData.RabbitMQSetting.RoutingKey;

// Use connect method to connect to the server

client.connect(function (err) {
    const db = client.db(dbName);
    amqp.connect(RabbitUrl, {
        useNewUrlParser: true,
        useUnifiedTopology: true,
    }, function (err, conn) {
        console.log(err)

        
        conn.createChannel(function (err, ch) {
            
            
            
            ch.assertQueue(QueueName, {
                durable: true
            });
            //ch.bindExchange(QueueName,exchangeName,RoutingKey);
            ch.bindQueue( QueueName, exchangeName, RoutingKey ); 
            
            console.log(" [*] Waiting for messages in %s. To exit press CTRL+C", QueueName);

            ch.consume(QueueName, function (msg) {
                    //console.log(msg)
                insertDocuments(db, msg, function () {
                    
                    //client.close();
                    //ch.ack(msg);
                   
                });
                //});

            },{
        noAck: true
    }); //amqp

        })
    });
});


const insertDocuments = function (db, msg, callback) {
    // Get the documents collection
    let tempName = JSON.parse(msg.content.toString());
    let collectionName = tempName.Logname.toString();
    
    const collection = db.collection(collectionName)
    
    // Insert some documents
    //console.log(JSON.parse((msg.content.toString())))
    collection.insertOne(
        JSON.parse(msg.content.toString())
    , function (err, result) {
        assert.equal(err, null);
        assert.equal(1, result.result.n);
        assert.equal(1, result.ops.length);
        //console.log("Inserted 3 documents into the collection");
        callback(result);
    });
}
