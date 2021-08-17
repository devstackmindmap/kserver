using AkaData;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebServer.Controller.Friend;

namespace WebLogicTest
{
    public class Test
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task TestTest()
        {
            var unit1001 = Data.GetUnitStat(1001, 1);
            unit1001.Atk = 600;
            var unit1001Second = Data.GetUnitStat(1001, 1);
        }

        [Test]
        public void AESCheck()
        {
            var encrypted = AES.AESEncrypt256("Encryption example");
            var decrypted = AES.AESDecrypt256(encrypted);
        }

        [Test]
        public void RandomStringTest()
        {
            SortedDictionary<string, int> strings = new SortedDictionary<string, int>();

            for (int i=0; i < 1; i++)
            {
                var rs = Utility.RandomString(16);
                if (strings.ContainsKey(rs))
                {
                    Assert.True(false);
                }

                strings.Add(rs, 123);
            }
            Assert.True(true);
        }

        [Test]
        public async Task GetFriendCodeTest()
        {
            var web = new WebGetFriendCode();
            var res = await web.DoPipeline(new ProtoUserId
            {
                MessageType = MessageType.GetFriendCode,
                UserId = 22
            });
        }

        [Test]
        public async Task AddFriendByCodeTest()
        {
            var web = new WebGetFriendCode();
            var res = await web.DoPipeline(new ProtoUserId
            {
                MessageType = MessageType.GetFriendCode,
                UserId = 22
            });

            var web2 = new WebAddFriendByCode();
            var res2 = await web2.DoPipeline(new ProtoAddInvite
            {
                MessageType = MessageType.GetFriendCode,
                UserId = 23,
                InviteCode = (res as ProtoInviteCode).InviteCode
            });
        }

        [Test]
        public void GenerateShortUniqueIdsFromIntegersTest()
        {
            /*
            var hashids = new Hashids("this is my salt");
            var id = hashids.Encode(1, 2, 3, 45);
            var numbers = hashids.Decode(id);


            for(int i = 1000000000; i < 1000000010; i++)
            {
                hashids = new Hashids("xyzkekr33jsk", 8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
                id = hashids.Encode(i);
                AkaLogger.Logger.Instance().Info(i + " : " + id);
                numbers = hashids.Decode(id);
            }
            */
        }

        class TestAnimationData
        {
            public int AnimationLength;
            public int BulletTime;
            public int TakeDamageTime;
        }

        [Test]
        public void DataTest()
        {
            var skill = Data.GetSkillWithoutAnimationData(1010101);

            TestAnimationData animationData = new TestAnimationData
            {
            };

            animationData.AnimationLength = 99939393;

            var a = Data.GetSkillWithoutAnimationData(1010101);
        }

        [Test]
        public void SlangDataTest()
        {
            var slangs = Data.GetSlang();
        }

        [Test]
        public void StringContainsTest()
        {
            var word = "빈 공 간";
            if (word.Contains(' '))
                Assert.True(true);
            else
                Assert.True(false);
        }

        [Test]
        public void StringContainsTest2()
        {
            var word = "빈 공 간";
            
            if (String.IsNullOrWhiteSpace(word))
                Assert.True(true);
            else
                Assert.True(false);
        }

        [Test]
        public void StringContainsTest3()
        {
            var word = "빈 공 간\"";

            if (word.Contains('"'))
                Assert.True(true);
            else
                Assert.True(false);
        }

        [Test]
        public void StringContainsTest4()
        {
            var word = "빈 공 간'";

            if (word.Contains("'"))
                Assert.True(true);
            else
                Assert.True(false);
        }


    }
}
