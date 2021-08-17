using AkaDB.MySql;
using AkaSerializer;
using CommonProtocol;
using KnightUWP.Dao;
using KnightUWP.Servicecs.Network;
using KnightUWP.Servicecs.Protocol;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs.Net
{
    static class Match
    {

     //   private static string Url => ProfileManager.ServerInfo.Servers["Android"][ProfileManager.Current.ToString()].MatchingServerIp;
        private static string Url => AkaConfig.Config.GameServerConfig.MatchingServer.ip;

        private static void Matching_ConnectedEvent(UserInfo userInfo, bool connected)
        {

        }

        private static void Matching_DataReceived( UserInfo userInfo, DataEventArgs e )
        {
            if ((e?.Data?.Length ?? 0) == 0)
            {
                return;
            }

            e = new DataEventArgs()
            {
                Data = e.Data.CloneRange(0, e.Data.Length),
                Length = e.Length,
                Offset = e.Offset
            };
            

            lock (userInfo)
            {
                if (userInfo.MatchingDatas.TryPeek(out var targ))
                    userInfo.MatchingDatas.Enqueue(e);
                else
                {
                    userInfo.MatchingDatas.Enqueue(e);
                    Task.Factory.StartNew(() =>
                    {
                        DoProcess(userInfo);
                    });
                }
            }
        }

        private static void DoProcess(UserInfo userInfo)
        {
            while (userInfo.MatchingDatas.TryPeek(out var e))
            {
                ProtocolProcessGenerator.DoProcess(userInfo, e);

                lock (userInfo)
                {
                    userInfo.MatchingDatas.TryDequeue(out e);
                }
            }
        }


        public static async Task TryMatching(UserInfo userInfo)
        {
            if (userInfo.MatchingConnecter?.IsConnected == true)
                userInfo.MatchingConnecter.Close();

            var onBattleServer = await API.GetBattleServer();
            if (onBattleServer == null)
                return;

            userInfo.MatchingDatas?.Clear();
            userInfo.MatchingDatas = new System.Collections.Concurrent.ConcurrentQueue<DataEventArgs>();

            userInfo.MatchingConnecter = new ClientNetworkConnector($"{userInfo.accounts.socialAccount}_m_connector");
            //userInfo.MatchingConnecter.Connect(Url, Port, Matching_ConnectedEvent, Matching_DataReceived, userInfo);
            userInfo.MatchingConnecter.Connect(onBattleServer.MatchingServerIp, onBattleServer.MatchingServerPort, Matching_ConnectedEvent, Matching_DataReceived, userInfo);

            userInfo.State = UserState.None;

            if (false == userInfo.MatchingConnecter.WaitConnect(1000))
                return;

            userInfo.State = UserState.Matching;
            userInfo.CurrentDeckNum = 0;
            
            userInfo.MatchingConnecter.Send(MessageType.TryMatching, AkaSerializer<ProtoTryMatching>.Serialize(new ProtoTryMatching()
            {
                BattleServerIp = onBattleServer.BattleServerIp,
                BattleServerPort = onBattleServer.BattleServerPort,
                DeckNum = (byte)userInfo.CurrentDeckNum,
                UserId = userInfo.users.userId,
                BattleType = AkaEnum.Battle.BattleType.LeagueBattle,
                MessageType = MessageType.TryMatching,
                GroupCode = 1
            }));

        }


    }
}
