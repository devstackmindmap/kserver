using AkaDB.MySql;
using AkaSerializer;
using CommonProtocol;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs.Protocol
{
    [Message(MessageType = MessageType.MatchingSuccess)]
    public class MatchingSuccess:  BaseProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoMatchingSuccess>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol resData)
        {
            var userInfo = context as UserInfo;
            var protocol = resData as ProtoMatchingSuccess;
            userInfo.MatchingConnecter.Close();

            userInfo.CurrentMatchingInfo = protocol;
            userInfo.State = UserState.Matched;
        }
    }
}
