using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AkaConfig;
using AkaRedis;
using AkaRedisLogic;
using AkaSerializer;
using CommonProtocol;
using NUnit.Framework;
using SuperSocket.ClientEngine;

namespace BattleLogicTest
{
    public partial class StoryModeTest
    {

        private List<AsyncTcpSession> _asyncSessions = new List<AsyncTcpSession>();

        [Description("Set Redis for Pve Room")]
        private async Task CreateSetRedisPveRoomTest(int memid)
        {
            //TOOD mgNum -> enum
            var redis = AkaRedis.AkaRedis.Connector.GetDatabase(redisDbIndex);

            var battleServerInfo = new ProtoOnGetBattleServer
            {
                BattleServerIp = "127.0.0.1",
                BattleServerPort = 30654
            };
            var member = KeyMaker.GetMemberKey((uint)memid);
            var roomId = KeyMaker.GetNewRoomId();

            //TODO  Add Redis KeyType 
            var key = $"PveRoom{1000.ToString()}";  

            //XXX Lock이 필요한가? redis spec
            //Redis에 room id를 개설하는 정도로만 
            //TODO rank의 대응항목이 있다면 ( ex:high score )
         //   await MatchingRedisJob.AddRoomInfoAsync( redis, member, roomId, battleServerInfo.BattleServerIp, this.sessionid, key, 0);



            var roomInfoRaw = await MatchingRedisJob.GetRoomInfoAsync(redis, member);
            if (roomInfoRaw.HasValue)
            {
                memidList.Add(member);
            }
            else
            {
                Assert.Fail("Room 정보 생성실패");
            }
            // GameRedisData.AddTeamRankScore(redis, key, member, teamRankPoint);
            // GameRedisData.AddMatchingSessionInfo(redis, sessionId, member, roomId, key);

        }

        [Description("Get Redis for Pve Room")]
        private async Task GetRedisPveRoomTest(int memid)
        {
            var redis = AkaRedis.AkaRedis.Connector.GetDatabase(redisDbIndex);
            var member = KeyMaker.GetMemberKey((uint)memid);

            var roomInfoRaw = await MatchingRedisJob.GetRoomInfoAsync(redis, member);



            if (roomInfoRaw.HasValue)
            {
                //TODO RedisValueRoomInfo.MatchingGroupKey 대체를 위한 class 구성
                var roomInfo = AkaSerializer<RedisValueRoomInfo>.Deserialize(roomInfoRaw);
                Assert.That(roomInfo.Member, Is.EqualTo(member));
                return;
            }
            Assert.Fail("Room 정보가 없음");
        }

        [Description("Destroy Pve Room")]
        private async Task DeleteRedisPveRoomTest(int memid)
        {
            var redis = AkaRedis.AkaRedis.Connector.GetDatabase(redisDbIndex);
            var member = KeyMaker.GetMemberKey((uint)memid);
            redis.HashDelete(RedisKeyType.HRoomIdList.ToString(), member);

            var roomInfoRaw = await MatchingRedisJob.GetRoomInfoAsync(redis, member);


            Assert.That(roomInfoRaw.HasValue, Is.EqualTo(false));
        }


        private void CheckPossibleMatching(uint memid , uint enemyid, bool expect)
        {
            var msgType = MessageType.TryMatching;
            var controller = MatchingServer.ControllerFactory.CreateController(msgType, 1, 1) as MatchingServer.TryMatching;

            if (controller == null)
            {
                Assert.Fail("Cant Created PveMatch Controller");
                return;
            }
        }

        private async Task<ProtoEnterRoom> CreateDummyEnterRoomMessage(int memid = defaultCaseMemid, byte deckNum = 0)
        {
            var redis = AkaRedis.AkaRedis.Connector.GetDatabase(redisDbIndex);
            var member = KeyMaker.GetMemberKey((uint)memid);

            var roomInfoRaw = await MatchingRedisJob.GetRoomInfoAsync(redis, member);

            if (roomInfoRaw.HasValue)
            {
                var roomInfo = AkaSerializer<RedisValueRoomInfo>.Deserialize(roomInfoRaw);

                var protoRoom = new ProtoEnterRoom
                {
                    MessageType = MessageType.EnterRoom,
                    UserId = (uint)memid,
                    RoomId = roomInfo.RoomId,
                    DeckNum = deckNum,
                    BattleServerIp = roomInfo.BattleServerIp
                };
                return protoRoom;
            }

            Assert.Fail("not Created room");
            return null;
        }

        private async Task<ProtoEnterRoom> CreateEnemyEnterRoomMessage(int memid = defaultCaseMemid, byte deckNum = 0)
        {
            var redis = AkaRedis.AkaRedis.Connector.GetDatabase(redisDbIndex);
            var member = KeyMaker.GetMemberKey((uint)memid);

            var roomInfoRaw = await MatchingRedisJob.GetRoomInfoAsync(redis, member);

            if (roomInfoRaw.HasValue)
            {
                var roomInfo = AkaSerializer<RedisValueRoomInfo>.Deserialize(roomInfoRaw);

                var protoRoom = new ProtoEnterRoom
                {
                    MessageType = MessageType.EnterRoom,
                    UserId = (uint)memid,
                    RoomId = roomInfo.RoomId,
                    DeckNum = deckNum,
                    BattleServerIp = roomInfo.BattleServerIp
                };
                return protoRoom;
            }

            Assert.Fail("not Created room");
            return null;
        }


        private void ConnectPVEServer(string serverName, string ip,  int port)
        {
            var task = new TaskCompletionSource<bool>();

            var asyncSession = new AsyncTcpSession();

            asyncSession.Connected += delegate {
                Console.WriteLine($"{serverName} Connected");
                task.TrySetResult(true);
            };

            asyncSession.Closed += delegate
            {

            };

            _asyncSessions.Add(asyncSession);

            asyncSession.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port));

            var connected = task.Task.Wait(1000) && asyncSession.IsConnected;
            Assert.That(connected, Is.EqualTo(true));

        }


        private void Send(AsyncTcpSession session, MessageType messageType, byte[] data)
        {
            var header = BitConverter.GetBytes((int)messageType);
            using (var stream = new MemoryStream(header.Length + data.Length))
            {
                stream.Write(header, 0, header.Length);
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                session.Send(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }


        private  Task<bool> EnterPveRoomOnBattleServer(int area, int userid, byte deckNum, int enemyid, byte enemyDeckNum, string battleServerIp, string webServerIp)
        {
            var task = new TaskCompletionSource<bool>();

            var asyncSession = new AsyncTcpSession();

            asyncSession.Connected += delegate
            {
                Console.WriteLine("Battle Server Connected");

                /*
                var rawEnterroom = AkaSerializer<ProtoEnterPveRoom>.Serialize(new ProtoEnterPveRoom
                {
                    MessageType = MessageType.EnterPveRoom,
                    UserId = (uint)userid,
                    EnemyId = (uint) enemyid ,
                    AreaNum = area,
                    EnemyDeckNum = enemyDeckNum,
                    DeckNum = deckNum,
                    //  BattleServerIp = req.BattleServerIp,
                    WebServerIp = webServerIp

                });
                */
              //  Send(asyncSession, MessageType.EnterPveRoom, rawEnterroom);
            };

            asyncSession.Closed += delegate
            {

            };

            asyncSession.DataReceived += (sender, e) =>
            {
                Console.WriteLine("Battle Server EnterRoom received");
                task.TrySetResult(true);

                //roomid match에서 삭제후 battle로 접속하는데 이에 대한 문제점. 무결성체크
                //어쨌든 2client대기가 안되게 할 수 있는 구분 수단이 필요 (Monster Id의 전달수단 강구)
            };

            _asyncSessions.Add(asyncSession);

            asyncSession.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(battleServerIp), Config.BattleServerConfig.BattleServerPort));

            Console.WriteLine("Battle Server2 " );
            return task.Task;

        }

    }
}
