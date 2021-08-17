using AkaData;
using AkaDB.MySql;
using AkaSerializer;
using CommonProtocol;
using CommonProtocol.Battle;
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
    [Message(MessageType = MessageType.UpdateUnitAttackTime, Name = "UpdateAttackTime")]
    public class UpdateUnitAttackTime : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoUpdateUnitAttackTime>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;

            //TODO 
            //attack speed, next attack time

        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var protocol = Event as ProtoUpdateUnitAttackTime;
            return $"{protocol.PlayerType}:{protocol.UnitId} - {protocol.AttackSpeed} , Next {protocol.NextAttackTime}";
        }


        protected override HorizontalAlignment GetPerformer(BaseProtocol _)
        {
            var protocol = Event as ProtoUpdateUnitAttackTime;
            if (protocol.PlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
                return HorizontalAlignment.Left;
            else
                return HorizontalAlignment.Right;
        }


        protected override string GetLeftSide(BaseProtocol protocol)
        {
            var updateTime = protocol as ProtoUpdateUnitAttackTime;
            if (updateTime.PlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return $"{Data.GetUnit(updateTime.UnitId).UnitInitial}(Speed:{updateTime.AttackSpeed}, Next:{new DateTime(updateTime.NextAttackTime).ToString("mm:ss.fff")})";
            }
            else
            {
                return "";
            }
        }

        protected override string GetRightSide(BaseProtocol protocol)
        {
            var updateTime = protocol as ProtoUpdateUnitAttackTime;
            if (updateTime.PlayerType != UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return $"{Data.GetUnit(updateTime.UnitId).UnitInitial}(Speed:{updateTime.AttackSpeed}, Next:{new DateTime(updateTime.NextAttackTime).ToString("mm:ss.fff")})";
            }
            else
            {
                return "";
            }
        }
    }
}
