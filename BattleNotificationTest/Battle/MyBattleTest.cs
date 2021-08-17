using AkaDB.MySql;
using AkaEnum.Battle;
using AkaSerializer;
using BattleLogic;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BattleLogic.Battle;

namespace BattleNotificationTest
{
    public class MyBattleTest
    {

        private List<Battle> battles = new List<Battle>();
        private uint _p1UserId;
        private byte _p1DeckNum;
        private uint _p2UserId;
        private byte _p2DeckNum;


        public MyBattleTest(uint p1UserId, byte p1DeckNum, uint p2UserId, byte p2DeckNum )
        {
            _p1UserId = p1UserId;
            _p1DeckNum = p1DeckNum;
            _p2UserId = p2UserId;
            _p2DeckNum = p2DeckNum;

        }

        public void BattleStart()
        {
            Run();
        }

        private async Task Run()
        {
            System.Threading.ThreadPool.GetMaxThreads(out var workerThreads, out var completionPortThreads);
            AkaLogger.Logger.Instance().Info(workerThreads.ToString() + " : " + completionPortThreads.ToString());

            /*
            var initializer = new BattleCreator(new PlayerInfo
                {
                    Player1UserId = _p1UserId,
                    Player1DeckNum = _p1DeckNum,
                    Player2UserId = _p2UserId,
                    Player2DeckNum = _p2DeckNum
                }
            , GetDeckInfo);

            var player1DecInfo = GetDeckInfo(PlayerType.Player1 , AkaEnum.ModeType.PVP);
            var player2DecInfo = GetDeckInfo(PlayerType.Player2, AkaEnum.ModeType.PVP);
            for (int i = 1; i < 500; ++i)
            {
                battles.Add(await initializer.GetBattle( i, BattleType.LeagueBattle, BattleEnd, null));
            }

            foreach (var battle in battles)
            {
                ThreadPool.QueueUserWorkItem(battle.BattleStart);
            }
            */
        }

        public void BattleEnd(PlayerType playerType)
        {

        }

        public BattleController GetBattleController()
        {
            foreach (var battle in battles)
            {
                return battle.GetBattleController();
            }
            throw new Exception();
        }

        private ProtoOnGetDeckWithDeckNum GetDeckInfo( AkaEnum.ModeType modeType)
        {
            var userId = _p1UserId;
            var deckNum = _p1DeckNum;

            /*
            if (playerType == PlayerType.Player2)
            {
                userId = _p2UserId;
                deckNum = _p2DeckNum;
            }
            */

            WebServerRequestor webServer = new WebServerRequestor();
            var resBytes = webServer.Request(MessageType.GetDeckWithDeckNum, AkaSerializer<ProtoGetDeckWithDeckNum>.Serialize(new ProtoGetDeckWithDeckNum
            {
                UserIdAndDeckNums = new List<KeyValuePair<uint, byte>>() { new KeyValuePair<uint, byte>(userId,deckNum)},
                ModeType = modeType,
            }));
            return AkaSerializer<ProtoOnGetDeckWithDeckNum>.Deserialize(resBytes);
        }
    }
}
