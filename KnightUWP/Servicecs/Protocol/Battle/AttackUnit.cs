using AkaData;
using AkaDB.MySql;
using AkaEnum.Battle;
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
    [Message(MessageType = MessageType.AttackUnit, Name = "Attack")]
    public class AttackUnit : AbstractBattleProcess
    {
        int counterHp = 0;
        int counterShield = 0;

        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoNormalAttack>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoNormalAttack;


            //TODO 
            // change unit status, hp takedamage
            //enqueue actor queue

            ObservableCollection<UnitInfo> targetUnits;
            UnitInfo performerUnit;

            if (protocol.PerformerPlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
            {
                targetUnits = userInfo.CurrentBattleInfo.EnemyUnits;
                performerUnit = userInfo.CurrentBattleInfo.MyUnits.Where(unit => unit.UnitId == protocol.PerformerUnitId).FirstOrDefault();
            }
            else
            {
                targetUnits = userInfo.CurrentBattleInfo.MyUnits;
                performerUnit = userInfo.CurrentBattleInfo.EnemyUnits.Where(unit => unit.UnitId == protocol.PerformerUnitId).FirstOrDefault();
            }

            foreach (var unitInfo in protocol.TargetUnitInfos)
            {
                foreach( var targetUnit in targetUnits.Where( unit => unit.UnitId == unitInfo.UnitId))
                {
                    if (unitInfo.Hp <= 0)
                    {
                        targetUnit.Hp = 0;
                        targetUnit.Shield = 0;
                        targetUnit.IsDeath = true;
                    }
                    else
                    {
                        targetUnit.Hp = unitInfo.Hp;
                        targetUnit.Shield = (int)unitInfo.Shield;
                    }
                }
            }

            var counterInfo = protocol.CounterUnitInfos.LastOrDefault();
            if (counterInfo != null && performerUnit != null)
            {
                counterHp = performerUnit.Hp = counterInfo.TargetUnitInfo.Hp;
                counterShield = performerUnit.Shield = (int)counterInfo.TargetUnitInfo.Shield;
            }

            string targetLog = string.Join("\n\t\t\t", protocol.TargetUnitInfos.Select(unit => $"{unit.UnitId} Damage:{unit.Damage} Hp:{unit.Hp} Shield:{unit.Shield} Critical:{unit.IsCritical}"));

            Utility.Log($"{EventHeadString("Attack")} {protocol.PerformerPlayerType}:{protocol.PerformerUnitId}-Atk{protocol.GrowthAtk} \n\t\t\t{targetLog}");
        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var protocol = Event as ProtoNormalAttack;
            string targetLog = string.Join("\n\t\t\t", protocol.TargetUnitInfos.Select(unit => $"{unit.UnitId} Damage:{unit.Damage} Hp:{unit.Hp} Shield:{unit.Shield} Critical:{unit.IsCritical}"));

            return $"{protocol.PerformerPlayerType}:{protocol.PerformerUnitId}-Atk{protocol.GrowthAtk} \n\t\t\t{targetLog}";
        }



        protected override HorizontalAlignment GetPerformer(BaseProtocol protocol)
        {
            var normalAttack = protocol as ProtoNormalAttack;
            if (normalAttack.PerformerPlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
                return HorizontalAlignment.Left;
            else
                return HorizontalAlignment.Right;
        }

        protected override string GetLeftSide(BaseProtocol protocol)
        {
            var normalAttack = protocol as ProtoNormalAttack;
            if (normalAttack.PerformerPlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return $"{Data.GetUnit(normalAttack.PerformerUnitId).UnitInitial}({normalAttack.PerformerUnitId})";
            }
            else
            {
                return string.Join(", ", normalAttack.TargetUnitInfos.Select(unit => $"{Data.GetUnit(unit.UnitId).UnitInitial}({ unit.UnitId})"));
            }
        }

        protected override string GetRightSide(BaseProtocol protocol)
        {
            var normalAttack = protocol as ProtoNormalAttack;
            if (normalAttack.PerformerPlayerType != UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return $"{Data.GetUnit(normalAttack.PerformerUnitId).UnitInitial}({normalAttack.PerformerUnitId})";
            }
            else
            {
                return string.Join(", ", normalAttack.TargetUnitInfos.Select(unit => $"{Data.GetUnit(unit.UnitId).UnitInitial}({ unit.UnitId})"));
            }
        }

        protected override string GetCenter(BaseProtocol protocol)
        {
            var normalAttack = protocol as ProtoNormalAttack;
            if (normalAttack.PerformerPlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return counterHp == 0 ? $"  ====== >>  " : $" = 카운터({counterHp}/{counterShield}) =>>";
            }
            else
            {
                return counterHp == 0 ? $"  << ======  " : $" <<= 카운터({counterHp}/{counterShield}) = ";
            }
        }
    }
}
