using System;

namespace AkaLogger.Battle
{
    // 실패 했을때 남기는 로그로서 DB 작업은 진행된 상태에서 Redis 작업이 안된 것이기 때문에 
    // 해당 유저 ID 에 대해서 랭킹정보Redis에 zincrby 작업 필요
    public sealed class LogBattleResultRedisFail
    {
        public void Log(uint userId, int changedRankPoint)
        {
            Logger.Instance().Analytics("BattleResultRedisFail", "BattleStatus",
                "UserId", userId,
                "ChangedRankPoint", changedRankPoint);
        }
    }
}
