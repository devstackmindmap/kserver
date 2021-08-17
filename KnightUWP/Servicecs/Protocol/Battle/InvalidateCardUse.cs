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
using Windows.UI.Xaml;

namespace KnightUWP.Servicecs.Protocol
{
    [Message(MessageType = MessageType.InvalidateCardUse)]
    public class InvalidateCardUse : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoInvalidateCardUse>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoInvalidateCardUse;

            if (protocol.ReplacedCardStatId == 0)
                return;

            switch (protocol.ReplacedHandIndex)
            {
                case 0:
                    userInfo.CurrentBattleInfo.Card1 = protocol.ReplacedCardStatId; break;
                case 1:
                    userInfo.CurrentBattleInfo.Card2 = protocol.ReplacedCardStatId; break;
                case 2:
                    userInfo.CurrentBattleInfo.Card3 = protocol.ReplacedCardStatId; break;
                case 3:
                    userInfo.CurrentBattleInfo.Card4 = protocol.ReplacedCardStatId; break;
            }

        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var protocol = Event as ProtoInvalidateCardUse;
            return $"ReplaceCard:{protocol.ReplacedCardStatId} index:{protocol.ReplacedHandIndex}  Next:{protocol.NextCardStatId}";
        }

        protected override HorizontalAlignment GetPerformer(BaseProtocol protocol)
        {
            return HorizontalAlignment.Left;
        }


    }
}
