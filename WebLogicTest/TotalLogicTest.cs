using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaSerializer;
using Common.Entities.Item;
using Common.Entities.Reward;
using CommonProtocol;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestHelper;
using WebServer.Controller.Battle;
using WebServer.Controller.Box;
using WebServer.Controller.LevelUp;
using WebServer.Controller.Rank;
using System.Collections.Concurrent;

namespace WebLogicTest
{
    class TotalLogicTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [TestCase(ItemType.UnitPiece, 100, (uint)1, 0)]
        [TestCase(ItemType.UnitPiece, 10000, (uint)1, 0)]
        [TestCase(ItemType.UnitPiece, 100, (uint)1, 50)]
        [TestCase(ItemType.UnitPiece, 10000, (uint)1, 50)]
        [TestCase(ItemType.UnitPiece, 100, (uint)2, 0)]
        [TestCase(ItemType.UnitPiece, 10000, (uint)2, 0)]
        [TestCase(ItemType.UnitPiece, 100, (uint)2, 50)]
        [TestCase(ItemType.UnitPiece, 10000, (uint)2, 50)]
        [TestCase(ItemType.UnitPiece, 100, (uint)0, 50)]
        [TestCase(ItemType.UnitPiece, 10000, (uint)0, 50)]

        [TestCase(ItemType.CardPiece, 100, (uint)1, 0)]
        [TestCase(ItemType.CardPiece, 10000, (uint)1, 0)]
        [TestCase(ItemType.CardPiece, 100, (uint)1, 50)]
        [TestCase(ItemType.CardPiece, 10000, (uint)1, 50)]
        [TestCase(ItemType.CardPiece, 100, (uint)2, 0)]
        [TestCase(ItemType.CardPiece, 10000, (uint)2, 0)]
        [TestCase(ItemType.CardPiece, 100, (uint)2, 50)]
        [TestCase(ItemType.CardPiece, 10000, (uint)2, 50)]
        [TestCase(ItemType.CardPiece, 100, (uint)0, 50)]
        [TestCase(ItemType.CardPiece, 10000, (uint)0, 50)]

        [TestCase(ItemType.WeaponPiece, 100, (uint)1, 0)]
        [TestCase(ItemType.WeaponPiece, 10000, (uint)1, 0)]
        [TestCase(ItemType.WeaponPiece, 100, (uint)1, 50)]
        [TestCase(ItemType.WeaponPiece, 10000, (uint)1, 50)]
        [TestCase(ItemType.WeaponPiece, 100, (uint)2, 0)]
        [TestCase(ItemType.WeaponPiece, 10000, (uint)2, 0)]
        [TestCase(ItemType.WeaponPiece, 100, (uint)2, 50)]
        [TestCase(ItemType.WeaponPiece, 10000, (uint)2, 50)]
        [TestCase(ItemType.WeaponPiece, 100, (uint)0, 50)]
        [TestCase(ItemType.WeaponPiece, 10000, (uint)0, 50)]
        public async Task 조각획득테스트(ItemType itemType, int increaseCount, uint level, int count)
        {
            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();
                var id = await userController.AddRandomPiece(itemType, level, count);

                if (level == 0)
                    level = userController.GetMaxLevel(itemType, id);

                await userController.SetPiece(itemType, id, level, count);

                userController.UserInventoryChecker.SetBeforeInventory();

                using (var db = new DBContext(userController.Player1UserId))
                {
                    var reardId = DataController.MakeReward(itemType, id, increaseCount, 1);
                    List<ProtoItemResult> rewards;
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                }

                userController.UserInventoryChecker.SetAfterInventory();

                Assert.True(userController.IsValidBeforeAfterInventoryStatus(itemType, id, level, increaseCount));
            }
        }


        [TestCase(ItemType.UnitPieceRandom, (uint)0, 100, (uint)1, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 500, (uint)1, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 1000, (uint)1, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 3000, (uint)1, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 5000, (uint)1, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 10000, (uint)1, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 500, (uint)2, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 1000, (uint)2, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 3000, (uint)2, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 5000, (uint)2, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 10000, (uint)2, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 500, (uint)0, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 1000, (uint)0, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 3000, (uint)0, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 5000, (uint)0, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 10000, (uint)0, 0)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 500, (uint)1, 100)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 1000, (uint)1, 200)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 3000, (uint)1, 300)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 5000, (uint)1, 400)]
        [TestCase(ItemType.UnitPieceRandom, (uint)0, 10000, (uint)1, 500)]
        public async Task Unit보상획득테스트
            (ItemType itemType, uint id, int increaseCount, uint level, int count)
        {
            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();
                await userController.LeftOnlyOneUnit();

                userController.UserInventoryChecker.SetBeforeInventory();

                var unitId = userController.GetOneUnitId();
                if (level == 0)
                    level = userController.GetMaxLevel(ItemType.UnitPiece, unitId);

                await userController.SetUnit(unitId, level, count);

                userController.UserInventoryChecker.SetBeforeInventory();

                List<ProtoItemResult> rewards;
                using (var db = new DBContext(userController.Player1UserId))
                {
                    var reardId = DataController.MakeReward(itemType, id, increaseCount, 1);
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                }

                userController.UserInventoryChecker.SetAfterInventory();

                Assert.True(userController.IsValidBeforeAfterInventoryStatus(ItemType.UnitPiece, unitId, level, increaseCount));
            }
        }

        [TestCase(ItemType.CardPieceRandom, (uint)0, 100, (uint)1, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 500, (uint)1, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 1000, (uint)1, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 3000, (uint)1, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 5000, (uint)1, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 10000, (uint)1, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 500, (uint)2, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 1000, (uint)2, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 3000, (uint)2, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 5000, (uint)2, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 10000, (uint)2, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 500, (uint)0, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 1000, (uint)0, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 3000, (uint)0, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 5000, (uint)0, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 10000, (uint)0, 0)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 500, (uint)1, 100)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 1000, (uint)1, 200)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 3000, (uint)1, 300)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 5000, (uint)1, 400)]
        [TestCase(ItemType.CardPieceRandom, (uint)0, 10000, (uint)1, 500)]
        public async Task Card보상획득테스트
            (ItemType itemType, uint id, int increaseCount, uint level, int count)
        {
            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();
                await userController.LeftOnlyOneCard();

                userController.UserInventoryChecker.SetBeforeInventory();

                var cardId = userController.GetOneCardId();
                if (level == 0)
                    level = userController.GetMaxLevel(ItemType.CardPiece, cardId);

                await userController.SetCard(cardId, level, count);

                userController.UserInventoryChecker.SetBeforeInventory();

                List<ProtoItemResult> rewards;
                using (var db = new DBContext(userController.Player1UserId))
                {
                    var reardId = DataController.MakeReward(itemType, id, increaseCount, 1);
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                }

                userController.UserInventoryChecker.SetAfterInventory();

                Assert.True(userController.IsValidBeforeAfterInventoryStatus(ItemType.CardPiece, cardId, level, increaseCount));
            }
        }

        [TestCase(ItemType.UnitPieceUnlockRandom, (uint)0, 100)]
        public async Task 모든유닛소유시_UnlockUnit획득테스트(ItemType itemType, uint id, int increaseCount)
        {
            var units = Data.GetUnitIdsByRewardProbabilityType();
            var maxUnitCount = units.Count;
            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();
                userController.UserInventoryChecker.SetBeforeInventory();
                var currentUnitCount = userController.UserInventoryChecker._beforeInventory.Units.Count;

                var reardId = DataController.MakeReward(itemType, id, increaseCount, 1);

                List<ProtoItemResult> rewards;

                for (int i = currentUnitCount; i < maxUnitCount + 1; i++)
                {
                    if (i == maxUnitCount)
                    {
                        userController.UserInventoryChecker.SetBeforeInventory();
                    }
                    using (var db = new DBContext(userController.Player1UserId))
                    {
                        rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                    }
                }

                userController.UserInventoryChecker.SetAfterInventory();

                if (userController.UserInventoryChecker._beforeInventory.Units.Count
                    == userController.UserInventoryChecker._afterInventory.Units.Count)
                {
                    var countUpUnitNum = 0;

                    foreach (var unit in userController.UserInventoryChecker._beforeInventory.Units)
                    {
                        if (unit.Value.count + increaseCount == userController.UserInventoryChecker._afterInventory.Units[unit.Value.id].count)
                            countUpUnitNum++;
                    }

                    if (countUpUnitNum == 1)
                    {
                        Assert.True(true);
                        return;
                    }
                    else if (countUpUnitNum == 0)
                    {
                        if (userController.UserInventoryChecker._beforeInventory.Gold + increaseCount
                            == userController.UserInventoryChecker._afterInventory.Gold)
                        {
                            Assert.True(true);
                            return;
                        }
                    }
                }
            }
            Assert.True(false);
        }

        [TestCase(PieceType.Unit)]
        [TestCase(PieceType.Card)]
        [TestCase(PieceType.Weapon)]
        public async Task PieceLevelUpTest(PieceType pieceType)
        {
            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();
                userController.UserInventoryChecker.SetBeforeInventory();
                var id = GetId(pieceType, userController);

                var reardId = DataController.MakeReward(GetItemType(pieceType), id, 100000, 1);
                using (var db = new DBContext(userController.Player1UserId))
                {
                    await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                }

                var web = new WebLevelUp();
                var res = await web.DoPipeline(new ProtoLevelUp
                {
                    UserId = userController.Player1UserId,
                    PieceType = pieceType,
                    ClassId = id
                });

                userController.UserInventoryChecker.SetAfterInventory();

                if (GetLevel(pieceType, userController.UserInventoryChecker._afterInventory, id) == 2)
                {
                    Assert.True(true);
                    return;
                }
            }

            Assert.True(false);
        }

        private uint GetId(PieceType pieceType, UserController userController)
        {
            uint id = 0;
            if (pieceType == PieceType.Unit)
            {
                foreach (var unit in userController.UserInventoryChecker._beforeInventory.Units)
                {
                    id = unit.Value.id;
                    break;
                }
            }
            else if (pieceType == PieceType.Card)
            {
                foreach (var card in userController.UserInventoryChecker._beforeInventory.Cards)
                {
                    id = card.Value.id;
                    break;
                }
            }
            else if (pieceType == PieceType.Weapon)
            {
                foreach (var weapon in userController.UserInventoryChecker._beforeInventory.Weapons)
                {
                    id = weapon.Value.id;
                    break;
                }
            }

            return id;
        }

        private ItemType GetItemType(PieceType pieceType)
        {
            if (pieceType == PieceType.Unit)
                return ItemType.UnitPiece;
            else if (pieceType == PieceType.Card)
                return ItemType.CardPiece;
            else if (pieceType == PieceType.Weapon)
                return ItemType.WeaponPiece;

            throw new Exception();
        }

        private uint GetLevel(PieceType pieceType, UserInventory userInventory, uint id)
        {
            if (pieceType == PieceType.Unit)
                return userInventory.Units[id].level;
            else if (pieceType == PieceType.Card)
                return userInventory.Cards[id].level;
            else if (pieceType == PieceType.Weapon)
                return userInventory.Weapons[id].level;

            throw new Exception();
        }


        [TestCase(100, 0, 100, 10, 20, 10, 9999, 1000, 100, 0, BattleResultType.Win)]
        [TestCase(100, 10, 100, 10, 20, 10, 9999, 1000, 100, 0, BattleResultType.Win)]
        [TestCase(100, 15, 100, 10, 20, 10, 9999, 1000, 100, 0, BattleResultType.Win)]
        [TestCase(100, 20, 100, 10, 20, 10, 9999, 1000, 100, 0, BattleResultType.Win)]
        [TestCase(100, 30, 100, 10, 20, 10, 9999, 1000, 100, 0, BattleResultType.Win)]
        //[TestCase(50, 30, 30, 10, 30, 10, 9999, 1000, 100, 9980, BattleResultType.Win)]
        //[TestCase(50, 30, 30, 10, 30, 10, 9999, 1000, 100, 9950, BattleResultType.Win)]
        public async Task 에너지박스테스트_승리후에너지박스(int startEnergy, int startBonusEnergy,
            float energyQuantityEachGet, float energyAcquisitionUnitMinute,
            float winInfusionEnergy, float loseInfusionEnergy,
            float maxInfusionEnergy, float maxEnergy, int firstBoxNeedEnergy, int boxEnergy, BattleResultType battleResultType)
        {
            DataController.SetEnergyBoxDataConstant(startEnergy, startBonusEnergy, energyQuantityEachGet,
                energyAcquisitionUnitMinute, winInfusionEnergy, loseInfusionEnergy, maxInfusionEnergy,
                maxEnergy, firstBoxNeedEnergy);

            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();
                await userController.SetInfusionBox("boxEnergy", boxEnergy);
                userController.UserInventoryChecker.SetBeforeInventory();
                var web = new WebGetBattleResult();
                await web.DoPipeline(new ProtoBattleResultStage
                {
                    PlayerInfoList = new List<ProtoBattleResultPlayerInfo>()
                    {
                        new ProtoBattleResultPlayerInfo
                        {
                            BattleResultType = battleResultType,
                            DeckNum = 0,
                            UserId = userController.Player1UserId
                        }
                    },
                    StageLevelId = 0,
                    BattleType = AkaEnum.Battle.BattleType.LeagueBattle
                });

                userController.UserInventoryChecker.SetAfterInventory();

                var addBonusEnergy = Math.Min(startBonusEnergy, winInfusionEnergy);

                var t1 = userController.UserInventoryChecker._beforeInventory.InfusionBox[InfusionBoxType.LeagueBox].userEnergy == startEnergy;
                var t2 = userController.UserInventoryChecker._afterInventory.InfusionBox[InfusionBoxType.LeagueBox].userEnergy == (startEnergy - winInfusionEnergy);
                var t3 = userController.UserInventoryChecker._afterInventory.InfusionBox[InfusionBoxType.LeagueBox].BoxEnergy == winInfusionEnergy + addBonusEnergy;
                Assert.True(t1 && t2 && t3);


            }
        }

        [TestCase(1000, 30, 500, 10, 1000, 10, 9999, 1000, 100, 0, BattleResultType.Win)]
        public async Task 에너지박스오픈(int startEnergy, int startBonusEnergy,
            float energyQuantityEachGet, float energyAcquisitionUnitMinute,
            float winInfusionEnergy, float loseInfusionEnergy,
            float maxInfusionEnergy, float maxEnergy, int firstBoxNeedEnergy, int boxEnergy, BattleResultType battleResultType)
        {
            DataController.SetEnergyBoxDataConstant(startEnergy, startBonusEnergy, energyQuantityEachGet,
                energyAcquisitionUnitMinute, winInfusionEnergy, loseInfusionEnergy, maxInfusionEnergy,
                maxEnergy, firstBoxNeedEnergy);

            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();
                await userController.SetInfusionBox("boxEnergy", boxEnergy);
                userController.UserInventoryChecker.SetBeforeInventory();

                var web = new WebGetBattleResult();
                await web.DoPipeline(new ProtoBattleResultStage
                {
                    PlayerInfoList = new List<ProtoBattleResultPlayerInfo>()
                    {
                        new ProtoBattleResultPlayerInfo
                        {
                            BattleResultType = battleResultType,
                            DeckNum = 0,
                            UserId = userController.Player1UserId
                        }
                    },
                    StageLevelId = 0,
                    BattleType = AkaEnum.Battle.BattleType.LeagueBattle
                });


                var web2 = new WebInfusionBoxOpen();
                await web2.DoPipeline(new ProtoInfusionBoxOpen
                {
                    Type = InfusionBoxType.LeagueBox,
                    UserId = userController.Player1UserId
                });

                var web3 = new WebInfusionBoxOpen();
                await web3.DoPipeline(new ProtoInfusionBoxOpen
                {
                    Type = InfusionBoxType.LeagueBox,
                    UserId = userController.Player1UserId
                });
                userController.UserInventoryChecker.SetAfterInventory();
            }
        }

        [Test]
        public async Task SkinTest1()
        {
            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();
                using (var db = new DBContext(userController.Player1UserId))
                {
                    var item = ItemFactory.CreateSkin(ItemType.Skin, userController.Player1UserId, 100101, db);

                    if (!await item.IsHave())
                        await item.Get("Test");
                    else
                    {
                        if (item.IsValidData(1001))
                            await item.PutOn(1001);
                    }
                }
            }
        }

        [Test]
        public async Task WebGetRankingBoardTest()
        {

            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();

                var web = new WebGetRankingBoard();
                var res = await web.DoPipeline(new ProtoRankingBoard
                {
                    UserId = userController.Player1UserId
                });
            }
        }


        [TestCase("joy_sq2", TestName = "이모티콘목록", Category = Category.Emoticon)]
        public void LoginForEmoticonListTest(string nickName)
        {
            var webServerUri = $"http://127.0.0.1:{AkaConfig.Config.GameServerConfig.GameServerPort}/";

            WebServerRequestor webServer = new WebServerRequestor();
            var result = webServer.Request(MessageType.Login, AkaSerializer.AkaSerializer<ProtoLogin>.Serialize(new ProtoLogin
            {
                MessageType = MessageType.Login

            }), webServerUri);

            var protoResult = AkaSerializer.AkaSerializer<ProtoOnLogin>.Deserialize(result);
            Assert.That(protoResult.EmoticonList.Count > 0);
        }


        [TestCase((uint)424, TestName = "이모티콘 저장", Category = Category.Emoticon)]
        public void SetEmoticonListTest(uint userId)
        {
            var webServerUri = $"http://127.0.0.1:{AkaConfig.Config.GameServerConfig.GameServerPort}/";
            List<ProtoEmoticonInfo> emoticons = new List<ProtoEmoticonInfo>();
            emoticons.Add(new ProtoEmoticonInfo
            {
                EmoticonId = 1,
                OrderNum = 0
            });
            emoticons.Add(new ProtoEmoticonInfo
            {
                EmoticonId = 2,
                OrderNum = 5
            });
            emoticons.Add(new ProtoEmoticonInfo
            {
                EmoticonId = 4,
                OrderNum = 1
            });



            WebServerRequestor webServer = new WebServerRequestor();
            var result = webServer.Request(MessageType.SetEmoticons, AkaSerializer.AkaSerializer<ProtoSetEmoticons>.Serialize(new ProtoSetEmoticons
            {
                MessageType = MessageType.SetEmoticons,
                UserId = userId,
                Emoticons = emoticons

            }), webServerUri);

            var protoResult = AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(result);
            Assert.That(protoResult.ResultType == ResultType.Success);
        }


        [TestCase(TestName = "계정생성 기본 이모티콘", Category = Category.Emoticon)]
        public async Task GetDefaultEmoticonTest()
        {
            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();
                var correct = Data.GetAllEmoticons().Where(emoticon => emoticon.IsFirstEmoticon)
                                      .Select(emoticon => emoticon.EmoticonId)
                                      .All( userController.LoginResult.EmoticonList.Select(emoticon => emoticon.EmoticonId).Contains);
                Assert.That(correct);
            }
        }

        [TestCase(TestName = "이모티콘 목록가져오기, 공용 포함", Category = Category.Emoticon)]
        public async Task GetEmoticons()
        {
            Action<List<ProtoEmoticonInfo>> outEmoticons = (emoticonList) =>
            {
                emoticonList.ForEach(emoticon => {
                    Console.WriteLine($"EmoticonId:{emoticon.EmoticonId}, UnitId:{emoticon.UnitId}, Order:{emoticon.OrderNum}");                    
                });
            };
            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();


                var firstEmoticonCount = userController.LoginResult.EmoticonList.Count;
                Assert.That(firstEmoticonCount == 0);

                //기본 이모티콘 insert 
                await userController.AddEmoticon(1);
                await userController.AddEmoticon(3);

                //올바르지 않은 emoticonid  not inserted
                await userController.AddEmoticon(9999);

                //잘못된 unitid not inserted
                await userController.UpdateEmoticon(5,1001,0);

                //기본 이모티콘 db에없지만 set (upsert) 
                await userController.UpdateEmoticon(5, 1003, 0);

                //이미 있는 이모티콘 순서변경
                await userController.UpdateEmoticon(5, 1003, 2);

                //기본 아닌 이모티콘 set not inserted
                await userController.UpdateEmoticon(6, 1003, 2);

                //공용
                await userController.UpdateEmoticon(9, 1003, 1);

                var rawDbEmoticons = await userController.GetEmoticons();
                outEmoticons(rawDbEmoticons);

                Assert.That(firstEmoticonCount + 4 == rawDbEmoticons.Count);
            }

        }


        [TestCase((uint)9, TestName = "이모티콘 보상", Category = Category.Emoticon)]
        [TestCase((uint)8, TestName = "이모티콘 보상", Category = Category.Emoticon)]
        public async Task Emoticon보상획득테스트(uint emoticonId)
        {
            Action<List<ProtoEmoticonInfo>> outEmoticons = (emoticonList) =>
            {
                emoticonList.ForEach(emoticon => {
                    Console.WriteLine($"EmoticonId:{emoticon.EmoticonId}, UnitId:{emoticon.UnitId}, Order:{emoticon.OrderNum}");
                });
            };

            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();

                List<ProtoItemResult> rewards;
                using (var db = new DBContext(userController.Player1UserId))
                {
                    var reardId = DataController.MakeReward(ItemType.Emoticon, emoticonId, 10, 100);
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                }

                var rawDbEmoticons = await userController.GetEmoticons();
                outEmoticons(rawDbEmoticons);

            }
        }

        [TestCase((uint)6, TestName = "이모티콘 박스 보상", Category = Category.Emoticon)]
        public async Task Emoticon박스보상획득테스트(uint emoticonId)
        {
            Action<List<ProtoEmoticonInfo>> outEmoticons = (emoticonList) =>
            {
                emoticonList.ForEach(emoticon => {
                    Console.WriteLine($"EmoticonId:{emoticon.EmoticonId}, UnitId:{emoticon.UnitId}, Order:{emoticon.OrderNum}");
                });
            };

            using (var userController = new UserController())
            {
                await userController.MakeOneUserData();

                List<ProtoItemResult> rewards;
                using (var db = new DBContext(userController.Player1UserId))
                {
                    var reardId = DataController.MakeReward(ItemType.Emoticon, emoticonId, 10, 100);
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");

                    var rawDbEmoticons = await userController.GetEmoticons();
                    outEmoticons(rawDbEmoticons);

                    Console.WriteLine("동일한 보상 또 요청");
                    reardId = DataController.MakeReward(ItemType.Emoticon, emoticonId, 10, 100);
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                    rawDbEmoticons = await userController.GetEmoticons();
                    outEmoticons(rawDbEmoticons);

                    Console.WriteLine("미소유 랜덤 요청");
                    reardId = DataController.MakeReward(ItemType.EmoticonUnlockRandom, emoticonId, 10, 100);
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                    rawDbEmoticons = await userController.GetEmoticons();
                    outEmoticons(rawDbEmoticons);

                    Console.WriteLine("미소유 랜덤 다채우고 요청");
                    reardId = DataController.MakeReward(ItemType.EmoticonUnlockRandom, emoticonId, 10, 100);
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                    reardId = DataController.MakeReward(ItemType.EmoticonUnlockRandom, emoticonId, 10, 100);
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                    reardId = DataController.MakeReward(ItemType.EmoticonUnlockRandom, emoticonId, 10, 100);
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                    reardId = DataController.MakeReward(ItemType.EmoticonUnlockRandom, emoticonId, 10, 100);
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");
                    reardId = DataController.MakeReward(ItemType.EmoticonUnlockRandom, emoticonId, 10, 100);
                    rewards = await Reward.GetRewards(db, userController.Player1UserId, reardId, "Test");


                    rawDbEmoticons = await userController.GetEmoticons();
                    outEmoticons(rawDbEmoticons);

                }

            }
        }


        [TestCase(TestName = "친선전 기본 유닛목록", Category = Category.FriendlyBattle)]
        public void GetFriendlyBattleUnitList()
        {
            var unitList = Data.GetUnitIdsByRewardProbabilityType();
            Assert.That(unitList.Count() == 8);
        }

        [TestCase(TestName = "친선전 기본 카드목록", Category = Category.FriendlyBattle)]
        public void GetFriendlyBattleCardList()
        {
            var cardList = Data.GetPrimitiveDict<uint, DataCard>(DataType.data_card).Values
                                .Where(data => data.CardType == CardType.User)
                                .Select(card => card.CardId).ToList();
            Assert.That(cardList.Count() == 36);
        }


        [TestCase((uint)470, 0, TestName = "친선전 덱", Category = Category.FriendlyBattle)]
        public void SetFriendlyBattleDeckTest(uint userId, byte decknum)
        {

            var webServerUri = $"http://127.0.0.1:{AkaConfig.Config.GameServerConfig.GameServerPort}/";
            WebServerRequestor webServer = new WebServerRequestor();


            var resBytes = webServer.Request(MessageType.GetDeckWithDeckNum, AkaSerializer<ProtoGetDeckWithDeckNum>.Serialize(new ProtoGetDeckWithDeckNum
            {
                UserIdAndDeckNums = new List<KeyValuePair<uint, byte>>()
                {
                    new KeyValuePair<uint, byte>(userId,(byte)decknum)
                },
                BattleType = AkaEnum.Battle.BattleType.FriendlyBattle,
                ModeType = ModeType.PVP
            }), webServerUri);
            var deck = AkaSerializer<ProtoDeckWithDeckNum>.Deserialize(resBytes);

            Assert.That(deck.CardsLevel.Count() == 8);
            Assert.That(deck.CardsLevel.Values.All( level => level == 10));
            Console.WriteLine("nickname : " + deck.Nickname);
            Assert.That(deck.UnitsInfo.Count() == 3);
            Assert.That(deck.UnitsInfo.Values.All(unitinfo => unitinfo.Level == 10));

        }

        [TestCase((uint)471, (uint)1, 1, (uint)1,  TestName = "퀘스트 완료 insert", Category = Category.Quest)]
        [TestCase((uint)471, (uint)2, 1, (uint)0, TestName = "퀘스트 미완료 insert", Category = Category.Quest)]
        [TestCase((uint)471, (uint)2, 3, (uint)0, TestName = "퀘스트 업데이트 insert", Category = Category.Quest)]
        public async Task SetQuestLogicTest(uint userid, uint questid,int countValue, uint completeOrder)
        {
            var questInfo = new ProtoQuestInfo
            {
                CompleteOrder = completeOrder,
                PerformCount = countValue,
                QuestGroupId = questid,
                ReceivedOrder = 0
            };
            using (var db = new DBContext(userid))
            {
              //  var result = await new QuestIO(userid, db).UpdatePerformCount(questInfo);
             //   Assert.That(result.Count == 1);
            }
        }

        [TestCase( TestName = "Transaction Last Id Test", Category = Category.Quest)]
        public async Task SetQuestLogicTest()
        {
            
            var result = new ConcurrentDictionary<uint, uint>();
            Enumerable.Range(1, 10000).AsParallel().ForAll(index =>
            {

                using (var db = new DBContext(1))
                {
                //    db.BeginTransactionCallback( async () =>
                   {
                        /*
                       var paramInfo = new DBParamInfo();
                       paramInfo.SetInputParam(
                           new InputArg("$p", index)
                           );
                       paramInfo.SetOutputParam(new OutputArg("$o", MySql.Data.MySqlClient.MySqlDbType.UInt32));

                  //      await db.CallStoredProcedureAsync("p_t1", paramInfo);
                        db.CallStoredProcedureAsync("p_t1", paramInfo).Wait();
                        uint r = paramInfo.GetOutValue<uint>("$o");
                        
                       */
                         db.ExecuteNonQueryAsync($"insert into trantest(t2) values ({index});").Wait();
                     //   Task.Delay(10).Wait();
                        using (var cursor =   db.ExecuteReaderAsync("select LAST_INSERT_ID() r1 from trantest;"))
                        {
                            cursor.Wait();
                           cursor.Result.Read();
                            result.TryAdd((uint)uint.Parse(cursor.Result["r1"].ToString()), (uint)index);
                        }
                        
                     //   result.TryAdd((uint)r, (uint)index);

                  //          return true;
                    }
              //         ).Wait();
                }
            });

            using (var db = new DBContext(1))
            {
                var cursor = await db.ExecuteReaderAsync("select t1, t2 from trantest");

                Console.WriteLine($"-----------------");
                while (cursor.Read())
                {
                    var t1 = (uint)cursor["t1"];
                    var t2 = (uint)cursor["t2"];
              //      if (result[t1] != t2)
                    {
                        Console.WriteLine($"{t1}: db-{t2} result-{result[t1]}");
                    }
                }
            }
            
        }


        [TestCase(50,40, TestName = "db side where vs caller side where", Category = Category.DB)]
        public async Task SQLSelectTest(int makeQuestCount, int filterCount)
        {

            using (var db = new DBContext(10000))
            {
                var userids = Enumerable.Range(10000, 100);
                var questGroupids = Enumerable.Range(1, makeQuestCount);
                var filterids = Enumerable.Range(20, filterCount);

                
                foreach (var userId in userids)
                {
                    var values = string.Join(",", questGroupids.Select(questGroupid => $" ({userId}, {questGroupid}, 11)") );
                    var query = $"insert into quests(userid,id,questtype) values {values}";
                    await db.ExecuteNonQueryAsync(query);
                }



                foreach (var userId in userids)
                {
                    var now = DateTime.UtcNow;
                    var selectQuery = $"select id, performCount, receivedOrder, completedOrder, questType from quests where userid = {userId} and (receivedOrder =0 or receivedOrder > completedOrder) and  id in ({ string.Join(",", filterids)})";
                    using (var cursor = await db.ExecuteReaderAsync(selectQuery))
                    {

                        var result = new List<ProtoQuestInfo>();
                        while (cursor.Read())
                        {
                            result.Add(
                                new ProtoQuestInfo
                                {
                                    QuestGroupId = (uint)cursor["id"],
                                    PerformCount = (int)cursor["performCount"],
                                    ReceivedOrder = (uint)cursor["receivedOrder"],
                                    CompleteOrder = (uint)cursor["completedOrder"],
                                });
                        }
                    }
                    var r1 = (DateTime.UtcNow - now).TotalMilliseconds;

                    now = DateTime.UtcNow;
                    selectQuery = $"select id, performCount, receivedOrder, completedOrder, questType from quests where userid = {userId} and (receivedOrder =0 or receivedOrder > completedOrder) ";
                    using (var cursor = await db.ExecuteReaderAsync(selectQuery))
                    {

                        var result = new List<ProtoQuestInfo>();
                        while (cursor.Read())
                        {

                            var QuestGroupId = (uint)cursor["id"];
                            if (filterids.Contains((int)QuestGroupId))
                            {

                                result.Add(
                                    new ProtoQuestInfo
                                    {
                                        QuestGroupId = QuestGroupId,
                                        PerformCount = (int)cursor["performCount"],
                                        ReceivedOrder = (uint)cursor["receivedOrder"],
                                        CompleteOrder = (uint)cursor["completedOrder"],
                                    });
                            }
                        }
                    }
                    var r2 = (DateTime.UtcNow - now).TotalMilliseconds;


                    now = DateTime.UtcNow;
                    foreach (var ix in filterids)
                    {
                        selectQuery = $"select id, performCount, receivedOrder, completedOrder, questType from quests where userid = {userId} and 1 > completedOrder and  id  ={ ix}";
                        using (var cursor = await db.ExecuteReaderAsync(selectQuery))
                        {

                            var result = new List<ProtoQuestInfo>();
                            while (cursor.Read())
                            {
                                result.Add(
                                    new ProtoQuestInfo
                                    {
                                        QuestGroupId = (uint)cursor["id"],
                                        PerformCount = (int)cursor["performCount"],
                                        ReceivedOrder = (uint)cursor["receivedOrder"],
                                        CompleteOrder = (uint)cursor["completedOrder"],
                                    });
                            }
                        }

                    }
                    
                    var r3 = (DateTime.UtcNow - now).TotalMilliseconds;
                    Console.WriteLine($"{userId} - result dbside : {r1} caller side:{r2} case by case:{r3}");
                }

                var deleteQuery = $"delete from quests where userid in ({ string.Join(",", userids)})";
                await db.ExecuteNonQueryAsync(deleteQuery);

            }
        }

    }
}
