using AkaEnum;
using AkaSerializer;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading;

namespace BattleNotificationTest
{
    public class NumberOfMatchingTest
    {
        List<Thread> _threads = new List<Thread>();
        public void Run()
        {
            for (int i = 30; i < 1030; i++)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(Matching));
                thread.IsBackground = false;
                thread.Start(i);

                //AkaLogger.Logger.Instance().Info("Thread Start {0}", i);
            }
        }

        public void Matching(object userId)
        {
            //var webServerRequestor = new WebServerRequestor();
            //var res = webServerRequestor.Request(MessageType.GetBattleServer,
            //    AkaSerializer<ProtoGetBattleServer>.Serialize(new ProtoGetBattleServer
            //    {
            //        MessageType = MessageType.GetBattleServer
            //    }));

            //var battleServerInfo = AkaSerializer<ProtoOnGetBattleServer>.Deserialize(res);

            var battleServerInfo = new ProtoOnGetBattleServer
            {
                BattleServerIp = "127.0.0.1",
                BattleServerPort = 30654
            };

            MatchingServerConnector matchingServerConnector = new MatchingServerConnector();
            matchingServerConnector.Connect(C2SInfo.InstanceInit(RunMode.Dylan2.ToString()).ServerInfo.MatchingServerIp, C2SInfo.Instance.ServerInfo.MatchingServerPort);

            while(!matchingServerConnector.IsConnected())
            {
                System.Threading.Thread.Sleep(100);
            }

            matchingServerConnector.Send(MessageType.TryMatching, AkaSerializer<ProtoTryMatching>.Serialize(new ProtoTryMatching
            {
                BattleServerIp = battleServerInfo.BattleServerIp,
                DeckNum = 0,
                UserId = uint.Parse(userId.ToString()),
                //DeckModeType = ModeType.PVP,
                MessageType = MessageType.TryMatching
            }));
        }
    }
}
