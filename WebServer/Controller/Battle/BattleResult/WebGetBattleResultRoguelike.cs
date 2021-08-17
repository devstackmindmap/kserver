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
    public class WebGetBattleResultRoguelike : BaseController
    {
        private BattleType _battleType;
        private uint _stageLevelId;

        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoBattleResultStage;

            _battleType = req.BattleType;
            _stageLevelId = req.StageLevelId;

            if (req.PlayerInfoList.Count != 1)
                throw new System.Exception();

            return await GetBattleResult(req.PlayerInfoList[0]);
        }

        //private async Task<ProtoOnBattleResultRoguelike> GetBattleResult(ProtoBattleResultPlayerInfo playerInfo)
        private async Task<ProtoOnBattleResultRankData> GetBattleResult(ProtoBattleResultPlayerInfo playerInfo)
        {
            var protoOnBattleResult = new ProtoOnBattleResultRoguelike();
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

            return new ProtoOnBattleResultRankData
            {
                UserLevelAndExp = protoOnBattleResult.UserLevelAndExp,
                BattleResultType = protoOnBattleResult.BattleResultType,
                CurrentSeason = protoOnBattleResult.CurrentSeason,
                ItemResults = protoOnBattleResult.ItemResults,
                QuestInfoList = protoOnBattleResult.QuestInfoList
            };
        }

        private async Task UpdateSeasonPass(uint userId, DBContext accountDb, DBContext userDb)
        {
            var serverSeason = new ServerSeason(accountDb);
            var seasonInfo = await serverSeason.GetSeasonPassInfo();
            await new SeasonPassManager(userId, seasonInfo.CurrentSeason, userDb).Update();
        }

        private IBattleResultManager CreateUserExpInfoManager(DBContext db, ProtoBattleResultPlayerInfo playerInfo, ProtoOnBattleResultRoguelike protoOnBattleResult)
        {
            return new UserLevel(db, _battleType, playerInfo.UserId, protoOnBattleResult);
        }

        private IBattleResultManager CreateBattleResultManager(DBContext userDb,
            ProtoBattleResultPlayerInfo playerInfo, ProtoOnBattleResultRoguelike protoOnBattleResult)
        {
            var deckModeType = AkaData.Data.GetContentsConstant(_battleType).DeckModeType;
            switch (_battleType)
            {
                case BattleType.AkasicRecode_RogueLike:
                case BattleType.AkasicRecode_UserDeck:
                case BattleType.DamageStrike:
                case BattleType.DoubleElixir:
                case BattleType.KillTheLeader:
                case BattleType.KillTheOneUnit:
                case BattleType.Max20Elixir:
                case BattleType.ThripleElixir:
                case BattleType.Tutorial:
                    var manager = deckModeType == ModeType.SaveDeck ?
                                     new StageLevelClear(userDb, playerInfo.UserId, _stageLevelId, protoOnBattleResult, _battleType)
                                   : new StageLevelClear(userDb, playerInfo.UserId, _stageLevelId, protoOnBattleResult);

                    return manager;
                default:
                    throw new System.Exception("Not Supported DeckModeType Exception");
            }
        }
    }
}
