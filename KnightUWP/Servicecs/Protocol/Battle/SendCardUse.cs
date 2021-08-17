using AkaDB.MySql;
using AkaSerializer;
using CommonProtocol;
using KnightUWP.Dao;
using Microsoft.Toolkit.Uwp.Helpers;
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
    public class SendCardUse : AbstractBattleProcess
    {

        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return null;
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoCardUse;

        }

        public override BaseProcess OnPreResponse<TContext>(TContext context, BaseProtocol protocol)
        {

            //TODO
            base.OnPreResponse(context, protocol);
            return this;
        }


        protected override HorizontalAlignment GetPerformer(BaseProtocol protocol)
        {
            return HorizontalAlignment.Left;
        }
        protected override string GetLeftSide(BaseProtocol _)
        {
            var protocol = Event as ProtoCardUse;
            return $"사용된 카드: {protocol.HandIndex} ({protocol.Performer.PlayerType}:{protocol.Performer.UnitPositionIndex}) {UserInfo.CurrentBattleInfo.NextCardStatId} ";
        }

        protected override string GetRightSide(BaseProtocol _)
        {
            var protocol = Event as ProtoCardUse;
            return $"타겟: ({protocol.Target.PlayerType}):{protocol.Target.UnitPositionIndex} ";
        }
    }
}
