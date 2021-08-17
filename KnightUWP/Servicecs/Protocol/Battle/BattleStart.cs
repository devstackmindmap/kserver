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
    [Message(MessageType = MessageType.BattleStart)]
    public class BattleStart : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoBattleStart>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoBattleStart;

            userInfo.CurrentBattleInfo.BattleStartTime = new DateTime(protocol.BattleStartTime);
            userInfo.State = UserState.BattleStart;

            userInfo.CurrentBattleInfo.BattleTime = "";


            userInfo.CurrentBattleInfo.ElixirInit();
            userInfo.CurrentBattleInfo.SetNextCard();
        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoBattleStart;
            return $" {userInfo.CurrentBattleInfo.BattleStartTime} {userInfo.accounts.userId}:{userInfo.accounts.nickName} vs {userInfo.CurrentBattleInfo.BeforeBattleStartInfo.EnemyPlayer.UserId}:{userInfo.CurrentBattleInfo.BeforeBattleStartInfo.EnemyPlayer.Nickname}";
        }
    }
}
