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

namespace KnightUWP.Servicecs.Protocol
{
    [Message(MessageType = MessageType.SyncTime)]
    public class SyncTime : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoOnSyncTime>.Deserialize(data);
        }


        public override BaseProcess OnPreResponse<TContext>(TContext context, BaseProtocol protocol)
        {
            if (protocol != null)
            {
                UserInfo = context as UserInfo;

                EventTime = DateTime.Now;
                Event = protocol;
                MessageType = MessageType.SyncTime;

            }
            return this;

        }


        public override void OnResponse<TContext>(TContext context, BaseProtocol _ )
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoOnSyncTime;


            //    var serverTime = new DateTime(protocol.ServerTime);
            //    var sendLatency = (serverTime - UserInfo.LastSyncTimeSended).TotalMilliseconds;
            //    var receivLatency = (DateTime.UtcNow - serverTime).TotalMilliseconds;
            var latency = (DateTime.UtcNow - UserInfo.LastSyncTimeSended).TotalMilliseconds;
            UserInfo.Latencties.Enqueue((int)latency);
            UserInfo.LastLatency = (int)latency;
            UserInfo.ReceivedSyncTime = true;
            UserInfo.AverageLatency = (int)UserInfo.Latencties.Average();

            if (UserInfo.MaxLatency < latency)
                UserInfo.MaxLatency = (int)latency;

         //   Utility.Log($"Receive {DateTime.UtcNow.ToString("ss.fff")} - {UserInfo.LastSyncTimeSended.ToString("ss.fff")} ");
         //  Utility.Log($"{latency} ");
         //TODO 
         // ping



        }
        public override string OutLogOnResponse<TContext>(TContext context)
        {
            return null;    
        }
    }
}
