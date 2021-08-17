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
    [Message(MessageType = MessageType.EnterRoomFail)]
    public class EnterRoomFail : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return null;
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            userInfo.BattleConnecter.Close();

            userInfo.State = UserState.None;
            userInfo.WriteLog("EnterRoomFail");
        }
        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var userInfo = context as UserInfo;
            return $"{userInfo.users.userId}";
        }
    }
}
