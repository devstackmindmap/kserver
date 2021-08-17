using System;

namespace AkaRedisLogic
{
    public static class KeyMaker
    {
        public static string GetNewRoomId()
        {
            return Guid.NewGuid().ToString();
        }

        public static string GetMemberKey(uint userId)
        {
            return userId.ToString();
        }

        public static string GetMatchingGroupScoreList(int matchingLine, string matchingId, int groupCode)
        {
            return $"{RedisKeyType.ZMatchingGroup.ToString()}[{groupCode}][{matchingLine}]{matchingId}";
        }

        public static string GetFvFMatchingGroupScoreList(string matchingId)
        {
            return $"{RedisKeyType.ZFvFMatchingGroup.ToString()}{matchingId}";
        }

        public static string GlobalUserRankBoard(uint serverCurrentSeason)
        {
            return $"{RedisKeyType.ZRankBoard.ToString()}{serverCurrentSeason}";
        }

        public static string GlobalClanRankBoard(uint serverCurrentSeason)
        {
            return $"{RedisKeyType.ZRankBoardClan.ToString()}{serverCurrentSeason}";
        }

        public static string GlobalUnitRankBoard(uint serverCurrentSeason, uint unitId)
        {
            return $"{RedisKeyType.ZRankBoardUnit.ToString()}{unitId}{RedisKeyType.Season.ToString()}{serverCurrentSeason}";
        }

        public static string GlobalUserRankBoard(uint serverCurrentSeason, string countryCode)
        {
            return $"{RedisKeyType.ZRankBoard.ToString()}{countryCode}{serverCurrentSeason}";
        }

        public static string GlobalClanRankBoard(uint serverCurrentSeason, string countryCode)
        {
            return $"{RedisKeyType.ZRankBoardClan.ToString()}{countryCode}{serverCurrentSeason}";
        }

        public static string GlobalUnitRankBoard(uint serverCurrentSeason, uint unitId, string countryCode)
        {
            return $"{RedisKeyType.ZRankBoardUnit.ToString()}{countryCode}{unitId}{RedisKeyType.Season.ToString()}{serverCurrentSeason}";
        }

        public static string BattleAreaServerIndex(int areaIndex, string serverIp)
        {
            return $"_{areaIndex}_{serverIp}";
        }

        public static string BattleAreaServerIndex(int areaIndex, int serverIndex)
        {
            return $"_{areaIndex}_{serverIndex}";
        }
    }
}
