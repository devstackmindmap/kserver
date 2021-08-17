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
    [Message(MessageType = MessageType.OnEmoticonUse, Name ="Emoticon")]
    public class EmoticonUse : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoOnEmoticonUse>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoOnEmoticonUse;

        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoOnEmoticonUse;
            var dataEmoticon = AkaData.Data.GetEmoticon(protocol.EmoticonId);

            return $" {protocol.PlayerType} used {dataEmoticon.UnitId}'s {protocol.EmoticonId}.   in Player : {protocol.IsPlayer}";
        }
    }
}
