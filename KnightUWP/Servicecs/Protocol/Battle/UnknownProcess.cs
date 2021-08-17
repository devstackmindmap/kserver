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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs.Protocol
{
    public class UnknownProtocol : BaseProtocol
    {
        public byte[] Raw { get; set; }
    }

    public class UnknownProcess : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return new UnknownProtocol { Raw = data };
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {

            //TODO 
            // change unit status, hp takedamage
            //enqueue actor queue
            //bullet time

        }

        public override BaseProcess OnPreResponse<TContext>(TContext context, BaseProtocol protocol)
        {
            if (protocol != null)
            {
                UserInfo = context as UserInfo;
                EventTime = DateTime.Now;
                Event = protocol;
                MessageType = protocol.MessageType;

                LastLatency = UserInfo.LastLatency;
                EverageLatency = (int)UserInfo.Latencties.Average();

                if (VOProvider.Instance.EnableClientState == true)
                    UserInfo.CurrentBattleInfo.History.Add(this);
                Index = UserInfo.CurrentBattleInfo.History.Count;


                var log = OutLogOnResponse(context);
                if (log != null)
                    Utility.Log($"{EventHeadString(MessageType.ToString())} {log}");
            }
            return this;

        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var userInfo = context as UserInfo;
            return $"{userInfo.users.userId}";
        }

    }
}
