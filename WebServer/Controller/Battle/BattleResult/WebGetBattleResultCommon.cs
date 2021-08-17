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
    public abstract class WebGetBattleResultCommon : BaseController
    {
        protected BattleType _battleType;

        protected IBattleResultManager CreateBattleResultManager<Req, Res>(DBContext accountDb, DBContext userDb,
            ProtoBattleResultPlayerInfo playerInfo, int enemyTeamRankPoint, Req protoBattleResult, Res protoOnBattleResult)
        {
            var deckModeType = AkaData.Data.GetContentsConstant(_battleType).DeckModeType;
            switch (_battleType)
            {
                case BattleType.LeagueBattle:
                    return new RankResult(accountDb, userDb, playerInfo.UserId, playerInfo.DeckNum,
                        enemyTeamRankPoint, deckModeType, protoOnBattleResult as ProtoOnBattleResultRank,
                        playerInfo.ActionStatusLog, RankType.AllUnitRankPoint);
                case BattleType.LeagueBattleAi:
                    return new RankAi(accountDb, userDb, playerInfo.UserId, playerInfo.DeckNum,
                        enemyTeamRankPoint, deckModeType, protoOnBattleResult as ProtoOnBattleResultRank, 
                        playerInfo.ActionStatusLog, RankType.AllUnitRankPoint);

                case BattleType.VirtualLeagueBattle:
                    return new VirtualRank(userDb, playerInfo.UserId, playerInfo.DeckNum, deckModeType, 
                        protoOnBattleResult as ProtoOnBattleResultRankData, RankType.AllUnitVirtualRankPoint);

                case BattleType.Challenge:
                    return new ChallengeResult(accountDb, userDb, playerInfo.UserId, protoBattleResult as ProtoBattleResultChallenge,
                        protoOnBattleResult as ProtoOnBattleResult);
                case BattleType.EventChallenge:
                    return new EventChallengeResult(accountDb, userDb, playerInfo.UserId, protoBattleResult as ProtoBattleResultEventChallenge,
                        protoOnBattleResult as ProtoOnBattleResult);
                default:
                    throw new System.Exception("Not Supported BattleType Exception");
            }
        }

        protected async Task UpdateSeasonPass(uint userId, DBContext accountDb, DBContext userDb)
        {
            var serverSeason = new ServerSeason(accountDb);
            var seasonInfo = await serverSeason.GetSeasonPassInfo();
            await new SeasonPassManager(userId, seasonInfo.CurrentSeason, userDb).Update();
        }
    }
}
