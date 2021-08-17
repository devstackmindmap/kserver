using AkaEnum;
using NUnit.Framework;
using CommonProtocol;
using System.Threading.Tasks;
using WebServer.Controller.Clan;

namespace WebLogicTest
{
    class WebClanTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [TestCase("D클랜", ClanPublicType.Private, 1000, (uint)1)]
        [TestCase("B클랜", ClanPublicType.Public, 0, (uint)1)]
        [TestCase("X클랜", ClanPublicType.Public, 20000, (uint)3)]
        [TestCase("X클랜", ClanPublicType.Public, 20000, (uint)4)]
        public async Task ClanCreateTest(string clanName, ClanPublicType clanPublicType
            , int joinConditionRankPoint, uint userId)
        {
            var web = new WebClanCreate();
            var result = await web.DoPipeline(new ProtoClanCreate {
                ClanName = clanName,
                ClanPublicType = clanPublicType,
                ClanSymbolId = 1,
                JoinConditionRankPoint = joinConditionRankPoint,
                MessageType = MessageType.ClanCreate,
                UserId = userId,
                ClanExplain = "HI",
                CountryCode = "KR"
            });            
        }

        [TestCase((uint)126)]
        public async Task ClanRecommendTest(uint userId)
        {
            var web = new WebGetClanRecommend();
            var result = await web.DoPipeline(new ProtoUserId
            {
                UserId = userId
            });
        }

        [TestCase((uint)5, (uint)9)]
        public async Task ClanJoinTest(uint userId, uint clanId)
        {
            var web = new WebClanJoin();
            var result = await web.DoPipeline(new ProtoUserIdTargetId
            {
                MessageType = MessageType.ClanJoin,
                UserId = userId,
                TargetId = clanId
            });
        }

        [TestCase((uint)5)]
        public async Task ClanOutTest(uint userId)
        {
            var web = new WebClanOut();
            var result = await web.DoPipeline(new ProtoUserId
            {
                MessageType = MessageType.ClanJoin,
                UserId = userId
            });
        }

        [TestCase((uint)1, (uint)2)]
        public async Task ClanBanishTest(uint userId, uint targetId)
        {
            var web = new WebClanBanish();
            var result = await web.DoPipeline(new ProtoUserIdTargetId
            {
                MessageType = MessageType.ClanBanish,
                UserId = userId,
                TargetId = targetId
            });
        }

        [TestCase((uint)2, (uint)3, ClanMemberGrade.Number1)]
        public async Task ClanModifyMemberGradeTest(uint userId, uint targetId, ClanMemberGrade clanMemberGrade) 
        {
            var web = new WebClanModifyMemberGrade();
            var result = await web.DoPipeline(new ProtoModifyMemberGrade
            {
                MessageType = MessageType.ClanModifyMemberGrade,
                UserId = userId,
                TargetId = targetId,
                ClanMemberGrade = clanMemberGrade
            });
        }

        [TestCase((uint)1, (uint)12, (uint)1, ClanPublicType.Public, 5000, "JP", "HI3")]
        public async Task ClanProfileModifyTest(uint userId, uint clanId, uint clanSymbolId, 
            ClanPublicType clanPublicType, int joinConditionRankPoint, string countryCode, string clanExplain)
        {
            var web = new WebClanProfileModify();
            var result = await web.DoPipeline(new ProtoClanProfileModify
            {
                MessageType = MessageType.ClanProfileModify,
                UserId = userId,
                ClanId = clanId,
                ClanExplain = clanExplain,
                ClanPublicType = clanPublicType,
                ClanSymbolId = clanSymbolId,
                CountryCode = countryCode,
                JoinConditionRankPoint = joinConditionRankPoint
            });
        }
    }
}
