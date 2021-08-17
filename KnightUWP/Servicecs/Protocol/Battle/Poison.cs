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
    [Message(MessageType = MessageType.Poison)]
    public class Poison : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoPoison>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoPoison;

            //TODO 
            // change unit status, hp takedamage

            var targetUnits = protocol.PlayerType == UserInfo.CurrentBattleInfo.MyPlayer ? UserInfo.CurrentBattleInfo.MyUnits : UserInfo.CurrentBattleInfo.EnemyUnits;

            foreach (var unit in targetUnits.Where(unit => unit.UnitId == protocol.UnitId))
            {
                unit.Hp -= protocol.Damage;
                unit.Shield = protocol.Shield;

                if (unit.Hp <= 0)
                {
                    unit.Hp = 0;
                    unit.Shield = 0;
                    unit.IsDeath = true;
                }
            }
        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var protocol = Event as ProtoPoison;
            return $"{protocol.PlayerType} Unit:{protocol.UnitId}";
        }



        protected override HorizontalAlignment GetPerformer(BaseProtocol _)
        {
            var protocol = Event as ProtoPoison;
            if (protocol.PlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
                return HorizontalAlignment.Left;
            else
                return HorizontalAlignment.Right;
        }


        protected override string GetLeftSide(BaseProtocol protocol)
        {
            var poison = protocol as ProtoPoison;
            if (poison.PlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return $"{Data.GetUnit(poison.UnitId).UnitInitial}(Damage:{poison.Damage})";
            }
            else
            {
                return "";
            }
        }

        protected override string GetRightSide(BaseProtocol protocol)
        {
            var poison = protocol as ProtoPoison;
            if (poison.PlayerType != UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return $"{Data.GetUnit(poison.UnitId).UnitInitial}(Damage:{poison.Damage})";
            }
            else
            {
                return "";
            }
        }

    }
}
