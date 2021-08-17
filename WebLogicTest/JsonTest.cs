using AkaRedisLogic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebLogicTest
{

    [TestFixture]
    public class JsonTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public void Test()
        {
            List<string> ipList = new List<string>();
            ipList.Add("172.30.1.222");
            ipList.Add("172.30.1.223");
            ipList.Add("127.0.0.1");

            var redis = AkaRedis.AkaRedis.GetDatabase();
            var jsonString = JsonConvert.SerializeObject(ipList);
            redis.StringSet(RedisKeyType.SDeveloperList.ToString(), jsonString);
            var a = redis.StringGet(RedisKeyType.SDeveloperList.ToString());
            var list = JsonConvert.DeserializeObject<List<string>>(redis.StringGet(RedisKeyType.SDeveloperList.ToString()));
        }
        
        //string to object , 


        //JObject.Parse
    }

    public class DeveloperIpList
    {
        List<string> list = new List<string>();
    }
}
