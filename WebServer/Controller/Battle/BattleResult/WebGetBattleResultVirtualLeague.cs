using System.Threading.Tasks;
using AkaDB.MySql;
using Common.Entities.Battle;
using CommonProtocol;

namespace WebServer.Controller.Battle
{
    public class WebGetBattleResultVirtualLeague : WebGetBattleResultCommon
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoBattleResult;

            _battleType = req.BattleType;

            if (req.PlayerInfoList.Count != 1)
                throw new System.Exception();

            return await GetBattleResult(req.PlayerInfoList[0]);
        }

        private async Task<ProtoOnBattleResultRankData> GetBattleResult(ProtoBattleResultPlayerInfo playerInfo)
        {
            var protoOnBattleResult = new ProtoOnBattleResultRankData();
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(playerInfo.UserId))
                {
                    IBattleResultManager resultManager = CreateBattleResultManager(null, userDb, playerInfo, 0, 0, protoOnBattleResult);
                    IBattleResultManager userExpInfoManager = CreateUserExpInfoManager(userDb, playerInfo, protoOnBattleResult);

                    await userDb.BeginTransactionCallback(async () =>
                    {
                        await accountDb.BeginTransactionCallback(async () =>
                        {
                            await UpdateSeasonPass(playerInfo.UserId, accountDb, userDb);
                            return true;
                        });
                        await resultManager.BattleResultJob(playerInfo.BattleResultType);
                        await userExpInfoManager.BattleResultJob(playerInfo.BattleResultType);

                        return true;
                    });
                }
            }

            return protoOnBattleResult;
        }

        private IBattleResultManager CreateUserExpInfoManager(DBContext db, ProtoBattleResultPlayerInfo playerInfo, ProtoOnBattleResultRankData protoOnBattleResult)
        {
            return new UserLevel(db, _battleType, playerInfo.UserId, protoOnBattleResult);
        }
    }
}
