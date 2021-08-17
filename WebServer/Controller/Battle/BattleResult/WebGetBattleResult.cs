using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using Common.Entities.Battle;
using Common.Entities.Season;
using Common.Pass;
using CommonProtocol;

namespace WebServer.Controller.Battle
{
    public class WebGetBattleResult : BaseController
    {
        private BattleType _battleType;

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
                    IBattleResultManager resultManager = CreateBattleResultManager(userDb, playerInfo, protoOnBattleResult);
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

        private async Task UpdateSeasonPass(uint userId, DBContext accountDb, DBContext userDb)
        {
            var serverSeason = new ServerSeason(accountDb);
            var seasonInfo = await serverSeason.GetSeasonPassInfo();
            await new SeasonPassManager(userId, seasonInfo.CurrentSeason, userDb).Update();
        }

        private IBattleResultManager CreateUserExpInfoManager(DBContext db, ProtoBattleResultPlayerInfo playerInfo, ProtoOnBattleResultRankData protoOnBattleResult)
        {
            return new UserLevel(db, _battleType, playerInfo.UserId, protoOnBattleResult);
        }

        private IBattleResultManager CreateBattleResultManager(DBContext userDb,
            ProtoBattleResultPlayerInfo playerInfo, ProtoOnBattleResultRankData protoOnBattleResult)
        {
            var deckModeType = AkaData.Data.GetContentsConstant(_battleType).DeckModeType;
            switch (_battleType)
            {
                case BattleType.PracticeBattle:
                    return new PracticeBattleResult(protoOnBattleResult);
                default:
                    throw new System.Exception("Not Supported DeckModeType Exception");
            }
        }
    }
}
