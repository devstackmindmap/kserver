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
    [Message(MessageType = MessageType.UnitDeath)]
    public class UnitDeath : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoUnitDeath>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoUnitDeath;

            var units = userInfo.CurrentBattleInfo.MyUnits.Where(unit => unit.UnitId == protocol.UnitId);
            foreach (var unit in units)
            {
                unit.Shield = 0;
                unit.Hp = 0;
                unit.IsDeath = true;
            }

            for( int i = 0; i < 4; i++)
            {
                uint val = 0;
                protocol.HandCardStatIds.TryGetValue(i, out val);
                switch (i)
                {
                    case 0:
                        userInfo.CurrentBattleInfo.Card1 = val; break;
                    case 1:
                        userInfo.CurrentBattleInfo.Card2 = val; break;
                    case 2:
                        userInfo.CurrentBattleInfo.Card3 = val; break;
                    case 3:
                        userInfo.CurrentBattleInfo.Card4 = val; break;
                }
            }


            //TODO
            // card deactive
        }


        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var protocol = Event as ProtoUnitDeath;
            return $"Unit:{protocol.UnitId} NextCard:{protocol.NextCardStatId}";
        }



        protected override HorizontalAlignment GetPerformer(BaseProtocol protocol)
        {
            return HorizontalAlignment.Left;
        }

        protected override string GetLeftSide(BaseProtocol protocol)
        {
            var unitDeath = protocol as ProtoUnitDeath;
            return $"{Data.GetUnit(unitDeath.UnitId).UnitInitial}({unitDeath.UnitId})";
        }


    }
}
