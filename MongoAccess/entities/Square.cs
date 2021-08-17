using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace MongoAccess
{
    public class Square
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("message")]
        public string message { get; set; }

        [BsonElement("timestamp")]
        public string timestamp { get; set; }

        [BsonExtraElements()]
        public BsonDocument fields{ get; set; }


    }

    public class dbSquareEntitiy
    {
        public string userId;

        public bool wrongLevel;
        public bool wrongCoreLevel;
        public bool wrongAgencyLevel;
        public int objectExp;
        public uint objectLevel;
        public int coreExp;
        public uint coreLevel;
        public int agencyExp;
        public uint agencyLevel;


        public int usedObjectExp;
        public int fixedObjectExp;
        public uint fixedObjectLevel;
        public int distLevel;
        public int objectGold;

        public int usedCoreExp;
        public int fixedCoreExp;
        public uint fixedCoreLevel;
        public int distCoreLevel;
        public int coreGold;

        public int usedAgencyExp;
        public int fixedAgencyExp;
        public uint fixedAgencyLevel;
        public int distAgencyLevel;
        public int agencyGold;
        public uint rewardId;
        public int totalGold;
    }

}
