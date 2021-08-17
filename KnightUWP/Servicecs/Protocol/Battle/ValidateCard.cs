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
    [Message(MessageType = MessageType.ValidateCard)]
    public class ValidateCard : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoValidateCard>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoValidateCard;

            userInfo.CurrentBattleInfo.ReplaceElixir(protocol.CurrentElixirCount);
        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoValidateCard;
            return $"{protocol.PlayerType} card use {protocol.ResultType}";
        }

        protected override HorizontalAlignment GetPerformer(BaseProtocol _)
        {
            return HorizontalAlignment.Left;
        }


    }
}
