using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AkaEnum;
using CommonProtocol;
using Network;

namespace TriggerServer.Managers
{

    class ServerStateInfoComparer : IEqualityComparer<ServerStateInfo>
    {
        bool _onlyIpCheck;
        public ServerStateInfoComparer(bool onlyIpCheck)
        {
            _onlyIpCheck = onlyIpCheck;
        }
        public bool Equals(ServerStateInfo lhv, ServerStateInfo rhv)
        {
            return (_onlyIpCheck || lhv.Country == rhv.Country) && lhv.Ip == rhv.Ip;
        }

        public int GetHashCode(ServerStateInfo serverState)
        {
            var hash = serverState.Ip.GetHashCode();
            if (_onlyIpCheck)
                return hash;
            if (hash + serverState.Country > int.MaxValue)
                return hash - serverState.Country;
            else
                return hash + serverState.Country;
        }
    }

    class ServerStateInfo
    {
        internal int Country { get; set; }

        internal string Ip { get; set; }

        internal ServerStateType State { get; set; }

        internal BattleServerConnector Connector { get; set; }
    }

}
