using AkaData;
using AkaRedis;
using AkaSerializer;
using Common;
using CommonProtocol;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using AkaUtility;
using AkaEnum;
using System.Collections.Generic;
using System.Linq;
using AkaRedisLogic;
using AkaEnum.Battle;

namespace MatchingServer
{
    public abstract class MatchCommonBattle
    {

        protected int _matchingLine;
        protected DataContentsConstant BattleEnviroment { get; }

        public uint UserId { get; }
        public byte DeckNum { get; }
        public int AreaNum { get; }
        public string BattleServerIp { get; }

        public string BattleServerPort { get; }
        public int RedisDBIndex { get; protected set; }
        public DateTime NextSearchExpansionTime { get; protected set; }

        public string MatchingKey { get; protected set; }
        public string MemberKey { get; }
        public int MatchingSearchExpantionInterval { get; protected set; }

        public ModeType DeckModeType => BattleEnviroment.DeckModeType;

        public BattleType BattleType { get; protected set; }

        public BattleType AiBattleType { get; protected set; }

        public uint Wins { get; protected set; }


        public MatchCommonBattle(ProtoTryMatching protoTryMatching, int matchingLine)
        {
            UserId = protoTryMatching.UserId;
            DeckNum = protoTryMatching.DeckNum;
            BattleServerIp = protoTryMatching.BattleServerIp;
            BattleServerPort = protoTryMatching.BattleServerPort.ToString();

            BattleEnviroment = Data.GetContentsConstant(protoTryMatching.BattleType);

            MemberKey = KeyMaker.GetMemberKey(UserId);
            MatchingSearchExpantionInterval = (int)Data.GetConstant(DataConstantType.MATCHING_SEARCH_EXPANSION_INTERVAL).Value;
            NextSearchExpansionTime = DateTime.UtcNow.AddMilliseconds(MatchingSearchExpantionInterval);

            RedisDBIndex = 0;
            _matchingLine = matchingLine;
        }

        public bool IsTimeToSearchExpansion()
        {
            if (NextSearchExpansionTime <= DateTime.UtcNow)
            {
                NextSearchExpansionTime = DateTime.UtcNow.AddMilliseconds(MatchingSearchExpantionInterval);
                return true;
            }
            return false;

        }

        public virtual async Task InitMatchingPoint()
        {
        }

        public abstract Task AddMatchingScore(string roomId, IDatabase redis);

        public abstract Task<SortedSetEntry?> GetTargetAsync(IDatabase redis);

        public abstract int GetMyUserRankPoint();

        public abstract int GetAiUserRankPoint();

        public abstract int GetMyTeamRankPoint();

        public abstract int GetAiTeamRankPoint();

        public abstract void SetMatchingWaitingSecond();

        public abstract bool EnableTryMatching();

        public abstract bool EnableExpansionSearch();

        public abstract Task UpdateSearchExpansion(string roomId, IDatabase redis);

        public abstract SearchingRange GetSearchRange(long rankPoint);

        public abstract bool GetAiMatchRoundInfo(out uint stageRoundId);

        public abstract string GetRoomType();

        //public void SendTryMatchingLog(string roomId)
        //{
        //    AkaLogger.Log.Matching.TryMatching.Log(UserId, roomId, (byte)BattleEnviroment.BattleType, DeckNum,
        //                                           (byte)DeckModeType, BattleServerIp, MatchingKey, MemberKey,
        //                                           GetMyTeamRankPoint(), GetMyUserRankPoint());
        //}

        public void SendSuccessLog(string roomId, string battleServerIp, int enemyRankPoint, int enemyUserRankPoint, string member, string matchingGroupKey)
        {
            AkaLogger.Log.Matching.Result.Log(roomId, (byte)BattleEnviroment.BattleType, GetMyTeamRankPoint(), GetMyUserRankPoint(),
                                              enemyRankPoint, enemyUserRankPoint, battleServerIp, member, matchingGroupKey);
        }

        public void SendSuccessLog(int enemyRankPoint, int enemyUserRankPoint, uint stageRoundId)
        {
            AkaLogger.Log.Matching.Result.Log((byte)BattleEnviroment.BattleType, GetMyTeamRankPoint(), GetMyUserRankPoint(),
                                              enemyRankPoint, enemyUserRankPoint, BattleServerIp, MemberKey, MatchingKey);
        }

    }
}
