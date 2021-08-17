using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;

namespace Common.Entities.Battle
{
    public class RankAi : RankResult
    {
        public RankAi(DBContext accountDb, DBContext userDb, uint userId, byte deckNum, int enemyTeamRankPoint, 
            ModeType modeType, ProtoOnBattleResultRank protoOnBattleResult, List<ProtoActionStatus> actionStatusLog, 
            RankType rankType = RankType.AllUnitRankPoint, bool isAi = true)
            : base(accountDb, userDb, userId, deckNum, 0, modeType, protoOnBattleResult, actionStatusLog, rankType, isAi)
        {
        }
    }
}
