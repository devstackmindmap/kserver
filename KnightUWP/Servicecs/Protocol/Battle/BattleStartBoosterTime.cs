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
    [Message(MessageType = MessageType.StartBoosterTime, Name ="Boost Time")]
    public class BattleStartBoosterTime : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoEmpty>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            userInfo.CurrentBattleInfo.SetBoostTime();
            //TODO boost process
            Utility.Log($"{EventHeadString( "Boost Time")}");
        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            return "";
        }
    }
}
