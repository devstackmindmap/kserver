using AkaUtility;
using CommonProtocol;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PubSubServer
{
    public class Login : BaseController
    {
        private NetworkSession _networkSession;
        private CommonProtocol.PubSub.ProtoLogin _req;

        public override async Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo)
        {
            _req = requestInfo as CommonProtocol.PubSub.ProtoLogin;
            _networkSession = networkSession;
            _networkSession.Init(_req.UserId, _req.ClanId, _req.StateMessageType, _req.FriendIds, _req.ClanMemberIds);
            PubSubManager pubSubManager = new PubSubManager();
            pubSubManager.Subscribe(networkSession);
        }
    }
}
