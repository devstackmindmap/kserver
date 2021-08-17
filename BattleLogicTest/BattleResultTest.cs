using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkaConfig;
using AkaData;
using AkaDB;
using AkaEnum;
using AkaEnum.Battle;
using AkaRedis;
using AkaSerializer;
using BattleServer;
using CommonProtocol;
using NUnit.Framework;
using SuperSocket.ClientEngine;

namespace BattleLogicTest
{
    [TestFixture]
    public partial class BattleResultTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

    }
}
