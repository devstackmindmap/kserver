using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Season;
using CommonProtocol;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestHelper;
using WebServer.Controller.Battle;
using WebServer.Controller.Deck;
using WebServer.Controller.Rank;

namespace WebLogicTest
{
    public class RedisTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task GetTopRanksKnightLeagueTest()
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();

            using (var db = new DBContext("AccountDBSetting"))
            {
                var season = new Common.Entities.Season.ServerSeason(db);
                var serverSeasonInfo = await season.GetKnightLeagueSeasonInfo();
                var result = await AkaRedisLogic.GameBattleRankRedisJob.GetTopRanksKnightLeagueUserAsync(redis, serverSeasonInfo.CurrentSeason, "");
                int a;
                var b = result[1].Element.TryParse(out a);
                var result2 = string.Join(",", result);
            }
        }

        [Test]
        public void MakeUser()
        {
            MultiUserController multiUserController = new MultiUserController();
            multiUserController.MakeUserList(10);
        }

        [Test]
        public async Task SetDeck()
        {
            for (int i = 1; i < 11; i++)
            {
                ProtoSetDeck protoSetDeck = new ProtoSetDeck();
                protoSetDeck.UserId = (uint)i;
                protoSetDeck.UpdateDecks = new List<ProtoDeckElement>();
                protoSetDeck.UpdateDecks.Add(new ProtoDeckElement
                {
                    DeckNum = 0,
                    ClassId = 1001,
                    ModeType = ModeType.PVP,
                    OrderNum = 0,
                    SlotType = SlotType.Unit,
                });

                protoSetDeck.UpdateDecks.Add(new ProtoDeckElement
                {
                    DeckNum = 0,
                    ClassId = 1002,
                    ModeType = ModeType.PVP,
                    OrderNum = 1,
                    SlotType = SlotType.Unit,
                });

                protoSetDeck.UpdateDecks.Add(new ProtoDeckElement
                {
                    DeckNum = 0,
                    ClassId = 1003,
                    ModeType = ModeType.PVP,
                    OrderNum = 2,
                    SlotType = SlotType.Unit,
                });

                protoSetDeck.UpdateDecks.Add(new ProtoDeckElement
                {
                    DeckNum = 0,
                    ClassId = 102,
                    ModeType = ModeType.PVP,
                    OrderNum = 0,
                    SlotType = SlotType.Card,
                });

                protoSetDeck.UpdateDecks.Add(new ProtoDeckElement
                {
                    DeckNum = 0,
                    ClassId = 103,
                    ModeType = ModeType.PVP,
                    OrderNum = 1,
                    SlotType = SlotType.Card,
                });

                protoSetDeck.UpdateDecks.Add(new ProtoDeckElement
                {
                    DeckNum = 0,
                    ClassId = 103,
                    ModeType = ModeType.PVP,
                    OrderNum = 2,
                    SlotType = SlotType.Card,
                });

                protoSetDeck.UpdateDecks.Add(new ProtoDeckElement
                {
                    DeckNum = 0,
                    ClassId = 202,
                    ModeType = ModeType.PVP,
                    OrderNum = 3,
                    SlotType = SlotType.Card,
                });

                protoSetDeck.UpdateDecks.Add(new ProtoDeckElement
                {
                    DeckNum = 0,
                    ClassId = 203,
                    ModeType = ModeType.PVP,
                    OrderNum = 4,
                    SlotType = SlotType.Card,
                });

                protoSetDeck.UpdateDecks.Add(new ProtoDeckElement
                {
                    DeckNum = 0,
                    ClassId = 203,
                    ModeType = ModeType.PVP,
                    OrderNum = 5,
                    SlotType = SlotType.Card,
                });

                protoSetDeck.UpdateDecks.Add(new ProtoDeckElement
                {
                    DeckNum = 0,
                    ClassId = 302,
                    ModeType = ModeType.PVP,
                    OrderNum = 6,
                    SlotType = SlotType.Card,
                });

                protoSetDeck.UpdateDecks.Add(new ProtoDeckElement
                {
                    DeckNum = 0,
                    ClassId = 303,
                    ModeType = ModeType.PVP,
                    OrderNum = 7,
                    SlotType = SlotType.Card,
                });

                var web = new WebSetDeck();
                await web.DoPipeline(protoSetDeck);
            }

        }

        [Test]
        public async Task RandomBattle()
        {
            uint[] userIds = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            for (int i = 0; i < 100; i++)
            {
                await RandomBattle(userIds);
            }
        }

        public async Task RandomBattle(uint[] userIds)
        {
            var index = AkaRandom.Random.Next(0, userIds.Length);

            var battleResult = AkaRandom.Random.Next(0, 2) == 0 ? BattleResultType.Win : BattleResultType.Lose;

            var web = new WebGetBattleResult();
            await web.DoPipeline(new ProtoBattleResultStage
            {
                PlayerInfoList = new List<ProtoBattleResultPlayerInfo>()
                    {
                        new ProtoBattleResultPlayerInfo
                        {
                            BattleResultType = battleResult,
                            DeckNum = 0,
                            UserId = userIds[index]
                        }
                    },
                StageLevelId = 0,
                BattleType = AkaEnum.Battle.BattleType.LeagueBattle
            });
        }

        [Test]
        public async Task WebGetRankingBoardTest()
        {
            var web = new WebGetRankingBoard();
            var response = await web.DoPipeline(new ProtoRankingBoard
            {
                UserId = 4
            });
        }

        [Test]
        public void GetMyRank()
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();
            var a = redis.SortedSetRank("ZRankingKnightLeague", 6);
            
        }
    }
}
