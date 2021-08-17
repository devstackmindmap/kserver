using AkaConfig;
using AkaEnum;
using CommonProtocol;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebServer.Controller.Battle;
using WebServer.Controller.Matching;

namespace WebLogicTest
{
    public class WebGetBattleServerTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }


        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public async Task GetBattleServerTest(int groupCode)
        {
            var web = new WebGetBattleServer();

            var res = await web.DoPipeline(new ProtoGetBattleServer
            {
                GroupCode = groupCode
            }) as ProtoOnGetBattleServer;

            Console.WriteLine("" + groupCode + ":"+ res.BattleServerIp);
        }
    }
}
