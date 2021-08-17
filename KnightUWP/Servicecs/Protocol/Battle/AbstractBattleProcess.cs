using AkaDB.MySql;
using AkaEnum.Battle;
using AkaSerializer;
using CommonProtocol;
using KnightUWP.Dao;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KnightUWP.Servicecs.Protocol
{
    public abstract class AbstractBattleProcess :  BaseProcess
    {
        public DateTime EventTime { get; protected set; }
        public string EventTimeString => EventTime.ToString("mm:ss.ff");
        public BaseProtocol Event { get; protected set; }

        public MessageType MessageType { get; protected set; }

        public int Index { get; protected set; }

        //  public PlayerType Performer { get; set; }

        public HorizontalAlignment Performer { get; protected set; }

        public string LeftSideMessage { get; protected set; }
        public string CenterMessage { get; protected set; }
        public string RightSideMessage { get; protected set; }

        public int LastLatency { get; set; }

        public int EverageLatency { get; set; }

        public UserInfo UserInfo { get; protected set; }



        public override BaseProcess OnPreResponse<TContext>(TContext context, BaseProtocol protocol)
        {
            if (protocol != null)
            {
                UserInfo = context as UserInfo;

                EventTime = DateTime.Now;
                
                Event = protocol;
                MessageType = GetMessageType();

                Performer = GetPerformer(protocol);
                LeftSideMessage = GetLeftSide(protocol);
                RightSideMessage = GetRightSide(protocol);
                CenterMessage = GetCenter(protocol);
                LastLatency = UserInfo.LastLatency;
                EverageLatency = UserInfo.Latencties.Any() ? (int)UserInfo.Latencties.Average() : -1;


                if (VOProvider.Instance.EnableClientState == true)
                    UserInfo.CurrentBattleInfo.History.Add(this);
                Index = UserInfo.CurrentBattleInfo.History.Count;

                if (MessageType != MessageType.EnqueuedAction)
                    UserInfo.CurrentBattleInfo.FilteredHistory.Add(this);


                var log = OutLogOnResponse(context);
                if (log != null)
                    Utility.Log($"{EventHeadString( GetMessageName())} {log}");
            }
            return this;

        }

        protected virtual HorizontalAlignment GetPerformer(BaseProtocol protocol)
        {
            return HorizontalAlignment.Center;
        }

        protected virtual string GetLeftSide(BaseProtocol protocol)
        {
            return "";
        }

        protected virtual string GetRightSide(BaseProtocol protocol)
        {
            return "";
        }

        protected virtual string GetCenter(BaseProtocol protocol)
        {
            return "";
        }

        public virtual string GetLogString()
        {
            return "";
        }

        public virtual string OutLogOnResponse<TContext>(TContext context)
        {
            return null;
        }

        string GetMessageName()
        {
            var attribute = GetType().GetCustomAttribute(typeof(MessageAttribute)) as MessageAttribute;
            return attribute.Name;
        }

        MessageType GetMessageType()
        {
            var attribute = GetType().GetCustomAttribute(typeof(MessageAttribute)) as MessageAttribute;
            return attribute?.MessageType ?? MessageType.None;
        }

        protected string EventHeadString( string preMessage)
        {
            return "";
            //    var data = UserInfo.CurrentBattleInfo.History.Last();

            //    return $"[{preMessage,-15}] {data.EventTime:hh:mm:ss.fff} - {UserInfo.users.userId}";
        }

        protected void DamageProcessing(ProtoTargetUnitInfo targetUnit)
        {
            var targetUnits = targetUnit.PlayerType == UserInfo.CurrentBattleInfo.MyPlayer ? UserInfo.CurrentBattleInfo.MyUnits : UserInfo.CurrentBattleInfo.EnemyUnits;

            foreach (var unit in targetUnits.Where(unit => unit.UnitId == targetUnit.UnitId))
            {
                if (unit.Hp <= 0)
                {
                    unit.Hp = 0;
                    unit.Shield = 0;
                    unit.IsDeath = true;
                }
                else
                {
                    unit.Hp = targetUnit.Hp;
                    unit.Shield = (int)targetUnit.Shield;
                }
            }
        }
    }
}
