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
    [Message(MessageType = MessageType.GetBattleResult)]
    public class GetBattleResult :  BaseProcess
    {
        public override void OnResponse<TContext>(TContext context, byte[] data)
        {
            var battleResult = AkaSerializer<ProtoOnBattleResult>.Deserialize(data);

            var userInfo = context as UserInfo;
            userInfo.BattleConnecter.Close();

            userInfo.State = UserState.None;
        }
    }
}
