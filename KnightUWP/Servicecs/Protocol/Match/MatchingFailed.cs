using AkaDB.MySql;
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
    [Message(MessageType = MessageType.MatchingFail)]
    public class MatchingFailed :  BaseProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return null;
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol resData)
        {
            var userInfo = context as UserInfo;
            userInfo.MatchingConnecter.Close();

            userInfo.State = UserState.None;
        }
    }
}
