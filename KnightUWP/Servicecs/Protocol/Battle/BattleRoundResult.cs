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
    [Message(MessageType = MessageType.BattleChallengeRoundResult, Name ="RoundResult")]
    public class BattleRoundResult : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoStageRoundResult>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoStageRoundResult;

            //TODO round process, next , need, treasure, 

        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var protocol = Event as ProtoStageRoundResult;
            return $"";
        }
    }
}
