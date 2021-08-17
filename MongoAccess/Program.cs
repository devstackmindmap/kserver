using AkaConfig;
using AkaData;
using AkaDB;
using AkaDB.MySql;
using AkaEnum;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MongoAccess
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            _main().Wait();
        }


        static async Task _main()
        {
            var dataversion = 21;
            var runmode = "1.0.21";
            Console.WriteLine("Data Version : " + dataversion);
            Console.WriteLine("RunMode : " + runmode);

            Config.GameServerInitConfig(Server.GameServer, "Live");
            DBEnv.AllSetUp();

            var fileList = await new FileLoader(FileType.Table, runmode, dataversion)
                .GetFileList("http://download.akastudio.co.kr/table/Live/"+ dataversion  + "/table.json");
            new DataSetter().DataSet(fileList);

            var client = new MongoClient("mongodb://live-mongo.akastudio.co.kr:27017/");
            var db = client.GetDatabase("KnightRun");


            await SquareExp.DoProcess(db);

        }

        public async static Task<List<T>> GetCollection<T>(IMongoDatabase db, string collection, string message, string[] userIdList)
        {
            var squareCollection = db.GetCollection<T>(collection);
            BsonDocument filter = new BsonDocument();
            filter.Add("message", message);
            filter.Add("fields.UserId", new BsonDocument().Add("$in", new BsonArray().AddRange(userIdList)));
            //    filter.Add("fields.InvasionTime", new BsonDocument().Add("$gt", "2020-06-03 09:00:00"));


            BsonDocument aggrateFilter = new BsonDocument
            {
                {
                    "$group", new BsonDocument
                    {
                        {
                            "id", new BsonDocument { { "UserId", "$UserId" } }
                        }
                    }
                }
            };


            var elements = await squareCollection.Find(filter)//.SortBy(element => element.fields["fields"]["UserId"])
                                                 .ToListAsync();
            return elements;
        }

    }
}
