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
    [Message(MessageType = MessageType.CardUseResult)]
    public class CardUseResult : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoCardUseResult>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoCardUseResult;

            var elixirCountState = protocol.ElixirCountState;

            //TODO


        }


        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var protocol = Event as ProtoCardUseResult;
            return $" State:{protocol.ElixirCountState}";
        }

        protected override HorizontalAlignment GetPerformer(BaseProtocol protocol)
        {
            return HorizontalAlignment.Left;
        }
        protected override string GetLeftSide(BaseProtocol _)
        {
            var protocol = Event as ProtoCardUseResult;
            return $"사용된 카드: {protocol.HandIndex} ({protocol.ElixirCountState})";
        }

    }
}
