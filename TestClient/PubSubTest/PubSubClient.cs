using AkaSerializer;
using CommonProtocol;
using System;
using System.Collections.Generic;

namespace TestClient.PubSubTest
{
    public class PubSubClient
    {
        public static void Run()
        {
            PubSubConnector.Instance.Connect("172.30.1.224", 40594);
            while (PubSubConnector.Instance.IsConnected() == false) ;

            List<uint> friendIds = new List<uint>();
            List<uint> clanMemberIds = new List<uint>();

            while (true)
            {
                Console.Write("FriendId(end:q):");
                var friendId = Console.ReadLine();
                if (friendId == "q")
                    break;
                friendIds.Add(uint.Parse(friendId));
            }

            Console.Write("ClanId(end:q):");
            var clanId = Console.ReadLine();

            while (true)
            {
                Console.Write("ClanMemberId(end:q):");
                var clanMemberId = Console.ReadLine();
                if (clanMemberId == "q")
                    break;
                clanMemberIds.Add(uint.Parse(clanMemberId));
            }

            Console.Write("MyId:");
            var strUserId = Console.ReadLine();
            var userId = uint.Parse(strUserId);

            if (false == uint.TryParse(clanId, out var cid))
                cid = 0;
            PubSubConnector.Instance.Send(MessageType.PubSubLogin, AkaSerializer<CommonProtocol.PubSub.ProtoLogin>.
                Serialize(new CommonProtocol.PubSub.ProtoLogin
                {
                    MessageType = CommonProtocol.MessageType.PubSubLogin,
                    FriendIds = friendIds,
                    UserId = uint.Parse(strUserId),
                    StateMessageType = MessageType.PubSubLogin,
                    ClanId = cid,
                    ClanMemberIds = clanMemberIds
                }));

            PubSubSend.Run(userId);
        }
    }
}
