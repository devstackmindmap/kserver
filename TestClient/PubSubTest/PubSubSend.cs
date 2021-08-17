using System;
using System.Collections.Generic;

namespace TestClient.PubSubTest
{
    public class PubSubSend
    {
        public static void Run(uint userId)
        {
            while (true)
            {
                var message = Console.ReadLine();
                if (message == "m") Send(CommonProtocol.MessageType.PubSubMatchmaking, userId);
                else if (message == "b") Send(CommonProtocol.MessageType.PubSubBattle, userId);
                else if (message == "o") Send(CommonProtocol.MessageType.PubSubOnline, userId);
                else if (message == "fi")
                {
                    Console.Write("친구 UserId:");
                    var askUserId = Console.ReadLine();
                    Send(CommonProtocol.MessageType.PubSubFriendlyBattleInvite, userId, uint.Parse(askUserId));
                }
                else if (message == "fa")
                {
                    Console.Write("친구 UserId:");
                    var askUserId = Console.ReadLine();
                    Send(CommonProtocol.MessageType.PubSubFriendlyBattleAccept, userId, uint.Parse(askUserId));
                }
                else if (message == "fd")
                {
                    Console.Write("친구 UserId:");
                    var askUserId = Console.ReadLine();
                    Send(CommonProtocol.MessageType.PubSubFriendlyBattleDecline, userId, uint.Parse(askUserId));
                }
                else if (message == "fc")
                {
                    Console.Write("친구 UserId:");
                    var askUserId = Console.ReadLine();
                    Send(CommonProtocol.MessageType.PubSubFriendlyBattleCancel, userId, uint.Parse(askUserId));
                }
                else if (message == "fr")
                {
                    Console.Write("친구 UserId:");
                    var askUserId = Console.ReadLine();
                    Send(CommonProtocol.MessageType.PubSubFriendlyBattleReady, userId, uint.Parse(askUserId));
                }
                else if (message == "frc")
                {
                    Console.Write("친구 UserId:");
                    var askUserId = Console.ReadLine();
                    Send(CommonProtocol.MessageType.PubSubFriendlyBattleReadyCancel, userId, uint.Parse(askUserId));
                }
                else if (message == "a")
                {
                    Console.Write("친구 신청할 UserId:");
                    var askUserId = Console.ReadLine();
                    Send(CommonProtocol.MessageType.PubSubFriendAsked, userId, uint.Parse(askUserId));
                }
                else if (message == "s")
                {
                    Console.Write("친구 수락할 UserId:");
                    var askUserId = Console.ReadLine();
                    Send(CommonProtocol.MessageType.PubSubFriendSigned, userId, uint.Parse(askUserId));
                }
                else if (message == "d")
                {
                    Console.Write("삭제할 친구 UserId:");
                    var askUserId = Console.ReadLine();
                    Send(CommonProtocol.MessageType.PubSubFriendRemoved, userId, uint.Parse(askUserId));
                }
                else if (message == "clan join")
                {
                    Console.Write("클랜 clanId:");
                    var clanId = Console.ReadLine();
                    List<uint> clanMemberIds = new List<uint>();
                    while (true)
                    {
                        Console.Write("ClanMemberId(end:q):");
                        var clanMemberId = Console.ReadLine();
                        if (clanMemberId == "q")
                            break;
                        clanMemberIds.Add(uint.Parse(clanMemberId));
                    }

                    ClanSubscribeSend(CommonProtocol.MessageType.PubSubClanJoin, userId, Convert.ToUInt32(clanId), clanMemberIds);
                }
                else if (message == "clan out")
                {
                    Console.Write("클랜 clanId:");
                    var clanId = Console.ReadLine();
                    ClanSubscribeSend(CommonProtocol.MessageType.PubSubClanOut, userId, Convert.ToUInt32(clanId));
                }
                else if (message == "clan banish")
                {
                    Console.Write("target userId:");
                    var targetId = Console.ReadLine();
                    Console.Write("clanId :");
                    var clanId = Console.ReadLine();
                    ClanSubscribeSend(CommonProtocol.MessageType.PubSubClanBanish, userId, Convert.ToUInt32(targetId), Convert.ToUInt32(clanId));
                }
                else if (message == "clan g up")
                {
                    Console.Write("타겟 userId:");
                    var targetid = Console.ReadLine();
                    Console.Write("클랜 clanId:");
                    var clanId = Console.ReadLine();
                    ClanSubscribeSend(CommonProtocol.MessageType.PubSubClanModifyMemberGradeUp, userId, Convert.ToUInt32(targetid), Convert.ToUInt32(clanId));
                }
                else if (message == "clan g down")
                {
                    Console.Write("타겟 userId:");
                    var targetid = Console.ReadLine();

                    Console.Write("클랜 clanId:");
                    var clanId = Console.ReadLine();
                    ClanSubscribeSend(CommonProtocol.MessageType.PubSubClanModifyMemberGradeDown, userId, Convert.ToUInt32(targetid), Convert.ToUInt32(clanId));
                }
                else if (message == "clan modify")
                {
                    Console.Write("클랜 clanId:");
                    var clanId = Console.ReadLine();
                    ClanSubscribeSend(CommonProtocol.MessageType.PubSubClanProfileModify, Convert.ToUInt32(clanId));
                }
                else if (message == "clan msg")
                {
                    Console.Write("클랜 clanId:");
                    var clanId = Console.ReadLine();
                    Console.Write("message:");
                    var msg = Console.ReadLine();
                    ClanSubscribeSend(CommonProtocol.MessageType.PubSubClanChatting, userId, Convert.ToUInt32(clanId), msg);
                }
                else if (message == "q")
                {
                    Send(CommonProtocol.MessageType.PubSubLogout, userId);
                    Connector.Instance.Close();
                }
            }
        }

        private static void Send(CommonProtocol.MessageType messageType, uint userId)
        {
            PubSubConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoOne2One>.
                Serialize(new CommonProtocol.PubSub.ProtoOne2One
                {
                    MessageType = messageType,
                    UserId = userId
                }));
        }

        private static void Send(CommonProtocol.MessageType messageType, uint userId, uint friendId)
        {
            PubSubConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoOne2N>.
                Serialize(new CommonProtocol.PubSub.ProtoOne2N
                {
                    MessageType = messageType,
                    UserId = userId,
                    FriendId = friendId
                }));
        }

        private static void ClanSubscribeSend(CommonProtocol.MessageType messageType, uint userId, uint targetId)
        {
            PubSubConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.ProtoUserIdTargetId>.
                Serialize(new CommonProtocol.ProtoUserIdTargetId
                {
                    MessageType = messageType,
                    UserId = userId,
                    TargetId = targetId
                }));
        }

        private static void ClanSubscribeSend(CommonProtocol.MessageType messageType, uint userId, uint targetId, uint clanId)
        {
            PubSubConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.ProtoUserIdTargetIdClanId>.
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
            PubSubConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoClanChatting>.
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
            PubSubConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoClanId>.
                Serialize(new CommonProtocol.PubSub.ProtoClanId
                {
                    MessageType = messageType,
                    ClanId = clanId
                }));
        }

        private static void ClanSubscribeSend(CommonProtocol.MessageType messageType, uint userId, uint clanId, List<uint> clanMemberIds)
        {
            PubSubConnector.Instance.Send(messageType, AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoClanJoin>.
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
