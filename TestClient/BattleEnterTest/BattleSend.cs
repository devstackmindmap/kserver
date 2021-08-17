using CommonProtocol;
using System;
using System.Collections.Generic;

namespace TestClient.BattleTest
{
    public class BattleSend
    {
        private uint _userId;
        public BattleSend(uint userId)
        {
            _userId = userId;
        }

        public void Run(string ip)
        {
            while (true)
            {
                var message = Console.ReadLine();
                if (message == "ecr")
                {
                    Send(CommonProtocol.MessageType.EnterChallengeRoom, new ProtoEnterChallengeRoom
                    {
                        MessageType = MessageType.EnterChallengeRoom,
                        UserId = _userId,
                        DeckNum = 0,
                        BattleServerIp = ip,
                        BattleType = AkaEnum.Battle.BattleType.Challenge,
                        Season = 1,
                        Day = 1,
                        DifficultLevel = 2,
                        IsStart = true                        
                    });
                }
                else if (message == "eecr")
                {
                    Send(CommonProtocol.MessageType.EnterEventChallengeRoom, new ProtoEnterEventChallengeRoom
                    {
                        MessageType = MessageType.EnterEventChallengeRoom,
                        UserId = _userId,
                        DeckNum = 0,
                        BattleServerIp = ip,
                        BattleType = AkaEnum.Battle.BattleType.EventChallenge,
                        ChallengeEventId = 1,
                        DifficultLevel = 1,
                        IsStart = true
                    });
                }
                else if (message == "q")
                {
                    Send(CommonProtocol.MessageType.PubSubLogout, _userId);
                    Connector.Instance.Close();
                }
            }
        }
        private static void Send(CommonProtocol.MessageType messageType, ProtoEnterChallengeRoom protoEnterNormalChallengeRoom)
        {
            BattleServerConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.ProtoEnterChallengeRoom>.
                Serialize(protoEnterNormalChallengeRoom));
        }

        private static void Send(CommonProtocol.MessageType messageType, ProtoEnterEventChallengeRoom protoEnterEventChallengeRoom)
        {
            BattleServerConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.ProtoEnterEventChallengeRoom>.
                Serialize(protoEnterEventChallengeRoom));
        }

        private static void Send(CommonProtocol.MessageType messageType, uint userId)
        {
            BattleServerConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoOne2One>.
                Serialize(new CommonProtocol.PubSub.ProtoOne2One
                {
                    MessageType = messageType,
                    UserId = userId
                }));
        }

        private static void Send(CommonProtocol.MessageType messageType, uint userId, uint friendId)
        {
            BattleServerConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoOne2N>.
                Serialize(new CommonProtocol.PubSub.ProtoOne2N
                {
                    MessageType = messageType,
                    UserId = userId,
                    FriendId = friendId
                }));
        }

        private static void ClanSubscribeSend(CommonProtocol.MessageType messageType, uint userId, uint targetId)
        {
            BattleServerConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.ProtoUserIdTargetId>.
                Serialize(new CommonProtocol.ProtoUserIdTargetId
                {
                    MessageType = messageType,
                    UserId = userId,
                    TargetId = targetId
                }));
        }

        private static void ClanSubscribeSend(CommonProtocol.MessageType messageType, uint userId, uint targetId, uint clanId)
        {
            BattleServerConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.ProtoUserIdTargetIdClanId>.
                Serialize(new CommonProtocol.ProtoUserIdTargetIdClanId
                {
                    MessageType = messageType,
                    UserId = userId,
                    TargetId = targetId,
                    ClanId = clanId
                }));
        }

        private static void ClanSubscribeSend(CommonProtocol.MessageType messageType, uint userId, uint clanId, string msg)
        {
            BattleServerConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoClanChatting>.
                Serialize(new CommonProtocol.PubSub.ProtoClanChatting
                {
                    MessageType = messageType,
                    UserId = userId,
                    ClanId = clanId,
                    Message = msg
                }));
        }

        private static void ClanSubscribeSend(CommonProtocol.MessageType messageType, uint clanId)
        {
            BattleServerConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoClanId>.
                Serialize(new CommonProtocol.PubSub.ProtoClanId
                {
                    MessageType = messageType,
                    ClanId = clanId
                }));
        }

        private static void ClanSubscribeSend(CommonProtocol.MessageType messageType, uint userId, uint clanId, List<uint> clanMemberIds)
        {
            BattleServerConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoClanJoin>.
                Serialize(new CommonProtocol.PubSub.ProtoClanJoin
                {
                    MessageType = messageType,
                    UserId = userId,
                    ClanId = clanId,
                    ClanMemberIds = clanMemberIds
                }));
        }
    }
}
