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
    [Message(MessageType = MessageType.IgnitionBomb)]
    public class IgnitionBomb : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoElectricShock>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoElectricShock;


            DamageProcessing(protocol.TargetUnitInfo);
        }
        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var protocol = Event as ProtoElectricShock;
            return $"TargetUnit:{protocol.TargetUnitInfo.UnitId}";
        }

        protected override HorizontalAlignment GetPerformer(BaseProtocol _)
        {
            var protocol = Event as ProtoElectricShock;
            if (protocol.TargetUnitInfo.PlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
                return HorizontalAlignment.Left;
            else
                return HorizontalAlignment.Right;
        }

        protected override string GetLeftSide(BaseProtocol protocol)
        {
            var shock = protocol as ProtoElectricShock;
            if (shock.TargetUnitInfo.PlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return $"{Data.GetUnit(shock.TargetUnitInfo.UnitId).UnitInitial}(Hp:{shock.TargetUnitInfo.Hp})";
            }
            else
            {
                return "";
            }
        }

        protected override string GetRightSide(BaseProtocol protocol)
        {
            var shock = protocol as ProtoElectricShock;
            if (shock.TargetUnitInfo.PlayerType != UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return $"{Data.GetUnit(shock.TargetUnitInfo.UnitId).UnitInitial}(Hp:{shock.TargetUnitInfo.Hp})";
            }
            else
            {
                return "";
            }
        }
    }
}
