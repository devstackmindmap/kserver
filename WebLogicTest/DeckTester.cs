using AkaEnum;
using AkaUtility;
using CommonProtocol;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WebServer.Controller.Battle;
using WebServer.Controller.Deck;

namespace WebLogicTest
{
    class DeckTester
    {
        private ProtoOnLogin _protoOnLogin;

        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task SetDeck()
        {
            var deckElements = new List<ProtoDeckElement>();

            deckElements.Add(new ProtoDeckElement {
                DeckNum = 10,
                ClassId = 1004,
                OrderNum = 0,
                SlotType = SlotType.Unit,
                ModeType = ModeType.PVP
            });

            deckElements.Add(new ProtoDeckElement
            {
                DeckNum = 10,
                ClassId = 1005,
                OrderNum = 1,
                SlotType = SlotType.Unit,
                ModeType = ModeType.PVP
            });

            var protoSetDeck = new ProtoSetDeck
            {
                UserId = 986,
                UpdateDecks = deckElements
            };

            var webSetDeck = new WebSetDeck();
            var result = await webSetDeck.DoPipeline(protoSetDeck);
        }

        [Test]
        public void GetDeck()
        {
            var protoGetDeck = new ProtoGetDeck
            {
                ModeType = ModeType.PVP,
                UserId = _protoOnLogin.UserId
            };

            var webGetDeck = new WebGetDeck();
            var resultTask = webGetDeck.DoPipeline(protoGetDeck);
            resultTask.Wait();

            var protoOnGetDeck = resultTask.Result as ProtoOnGetDeck;
        }

        [Test]
        public void GetDeckForBattleTest()
        {
            var protoGetDeckForBattle = new ProtoGetDeckWithDeckNum
            {
                UserIdAndDeckNums = new List<KeyValuePair<uint, byte>>()
                {
                    new KeyValuePair<uint, byte>(15,(byte)0)
                },
                ModeType = ModeType.PVP,
            };
            var webGetDeckForBattle = new WebServer.Controller.Deck.WebGetDeckWithDeckNum();
            var resultTask = webGetDeckForBattle.DoPipeline(protoGetDeckForBattle);

            var protoOnGetDeckForBattle = resultTask.Result as ProtoDeckWithDeckNum;
        }


        [Test]
        public void GetDeckForRoguelikeStageTest()
        {
            /*
            var protoGetDeckForStageBattle = new ProtoGetDeckForStageBattle
            {
                UserId = 22,
                StageLevelId = 1,
                BattleType = AkaEnum.Battle.BattleType.Roguelike
            };
            var webGetDeckForBattle = new WebGetDeckForStageBattle();
            var resultTask = webGetDeckForBattle.DoPipeline(protoGetDeckForStageBattle);
            resultTask.Wait();

            var protoOnGetDeckForBattle = resultTask.Result as ProtoOnGetDeckForBattle;
            */
        }
    }
}
