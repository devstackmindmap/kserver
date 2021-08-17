using AkaData;
using AkaEnum;
using AkaRedisLogic;
using CommonProtocol;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Controller.Store;

namespace WebLogicTest
{
    public class WebGetSeasonRewardTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task GetRankScore()
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();
            var score =await GameBattleRankRedisJob.GetScoreRankKnightLeagueUserAsync(redis, 1, 4);
            if (score.HasValue)
            {
                //Data.GetUserRankLevelIdFromPoint((int)score.Value);
            }
            //DataSeasonReward
        }

        [Test]
        public void GetUserRankLevelIdFromPoint()
        {
            var rows = Data.GetPrimitiveValues<uint, DataUserRank>(DataType.data_user_rank);

            int sumRankPoint = 0;
            uint userRankLevelId = 0;
            foreach (var data in rows)
            {
                userRankLevelId = data.UserRankLevelId;
                sumRankPoint += data.NeedRankPointForNextLevelUp;
                if (1200 < sumRankPoint)
                {
                    break;
                }
            }
        }

        [Test]
        public async Task GetRankSeasonReward()
        {
            var web = new WebGetSeasonReward();
            var res = await web.DoPipeline(new ProtoUserId { UserId = 989 });
        }
    }
}
