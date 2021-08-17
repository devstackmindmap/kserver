using AkaData;
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
    [Message(MessageType = MessageType.AddElixir)]
    public class AddElixir : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoAddElixir>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoAddElixir;

            userInfo.CurrentBattleInfo.AddElixir(protocol.AddElixir);
            //TODO
            //elixir rewind

        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoAddElixir;
            return $" {protocol.AddElixir}";
        }

        protected override HorizontalAlignment GetPerformer(BaseProtocol protocol)
        {
            return HorizontalAlignment.Left;
        }

        protected override string GetLeftSide(BaseProtocol protocol)
        {
            var addElixir = protocol as ProtoAddElixir;
            return $"엘릭서 추가 :{addElixir.AddElixir}";
        }

    }
}
