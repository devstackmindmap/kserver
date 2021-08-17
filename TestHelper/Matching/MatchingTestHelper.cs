using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using AkaSerializer;
using AkaUtility;
using BattleLogic;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestHelper.Matching
{
    public class MatchingTestHelper : IDisposable
    {
        string _ip;
        int _port;
        List<(UserController user, MatchingServerConnector connector)> _connectors = new List<(UserController user, MatchingServerConnector connector)>();

        public MatchingTestHelper(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        UserController[] MakeUserList(int count /* , Rank */)
        {
            var userControllers = Enumerable.Range(0, count).Select(n => new UserController()).ToArray();
            userControllers.AsParallel().ForAll(usercontrol => usercontrol.MakeOneUserData().Wait());
            return userControllers;
        }

        MatchingServerConnector[] MakeConnections(int count)
        {
            var connectors = Enumerable.Range(0, count).Select(n => new MatchingServerConnector(_ip, _port)).ToArray();
            var result = connectors.AsParallel().All(connector =>
            {
                connector.Connect();
                return connector.WaitConnected();
            });

            return connectors;
        }

        public void MakeUserAndConnection(int count)
        {
            var users = MakeUserList(count);
            var connectors = MakeConnections(count);
            var userConnectors = users.Zip(connectors, (userController, connector) => (user: userController, connector: connector));
            _connectors.AddRange(userConnectors);
        }

        public async Task<bool> TryMatching(string webUri)
        {
            Network.WebServerRequestor webRequest = new Network.WebServerRequestor();
            var rawGetOnBattleServer = await webRequest.RequestAsync(MessageType.GetBattleServer, AkaSerializer<ProtoGetBattleServer>.Serialize(new ProtoGetBattleServer
            {
                MessageType = MessageType.GetBattleServer
            }), webUri);

            _connectors.AsParallel().ForAll(userConnect =>
            {

                var serverInfo = AkaSerializer<ProtoOnGetBattleServer>.Deserialize(rawGetOnBattleServer);


                userConnect.connector.Send(MessageType.TryMatching, AkaSerializer<ProtoTryMatching>.Serialize(new ProtoTryMatching()
                {
                    BattleServerIp = serverInfo.BattleServerIp,
                    DeckNum = 0,
                    UserId = userConnect.user.Player1UserId,
                    BattleType = AkaEnum.Battle.BattleType.LeagueBattle,
                    MessageType = MessageType.TryMatching
                }));

            });

            return true;
        }

        public int Count => _connectors.Count;

        public MatchingResultState[] WaitForSendResult()
        {
            var results = _connectors.AsParallel().Select(userConnect => userConnect.connector.WaitForReceiveResult()).ToArray();
            var res = results.Distinct(new DataComparer<MatchingResultState>(
                (result1, result2) => result1.RoomId != null && result2.RoomId != null && result1.RoomId.Equals(result2.RoomId)
                , result => result.RoomId?.GetHashCode() ?? 0))
            .ToArray();
            return res;
        }

        public void Dispose()
        {
            _connectors.AsParallel().ForAll(userConnect =>
            {
                userConnect.user.Dispose();
                userConnect.connector.Close();
            });
        }
    }
}
