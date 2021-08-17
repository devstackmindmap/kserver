using System.Threading.Tasks;
using AkaConfig;
using AkaEnum;
using AkaRedisLogic;
using CommonProtocol;

namespace TriggerServer.Managers
{
    class BattleServerInfoUpdateManager : ServerInfoUpdateManager, IScheduleManager
    {
        public BattleServerInfoUpdateManager() 
            : base("battleservercheck", 
                  "Battle Server StateInfo Scheduler", 
                  Config.TriggerServerConfig.BattleServerList,
                  RedisKeyType.ZBattleServerState,
                  Server.BattleServer)
        {
        }

        public override async Task Initialize()
        {
            await base.Initialize();
        }

        public override void DoCommand(MessageType msgType, BaseProtocol requestInfo)
        {
            base.DoCommand(msgType, requestInfo);
        }

        public override string GetJobName()
        {
            return base.GetJobName();
        }

        public override void StartScheduler()
        {
            base.StartScheduler();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
