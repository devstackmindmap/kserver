using AkaRedisLogic;
using AkaUtility;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace MatchingServer
{
    public static class WaitingRoomManager
    {
        public static string MakeRoom()
        {
            WaitingRoom room = new WaitingRoom();
            var roomId = KeyMaker.GetNewRoomId();
            WaitingRooms.Instance.Add(roomId, room);
            return roomId;  
        }

        public static async Task<SortedSetEntry?> GetSearchingTargetAsync(IDatabase redis, string key, SearchingRange searchingRange, string member)
        {
            if( searchingRange.BeforeMax < 0 )
            {
                var afterList = await MatchingRedisJob.GetRangeListAsync(redis, key, searchingRange.AfterMin, searchingRange.AfterMax);

                if (afterList.Length == 0)
                    return null;

                return afterList[AkaRandom.Random.Next(afterList.Length)];
            }
            else
            {
                var beforeList = await MatchingRedisJob.GetRangeListAsync(redis, key, searchingRange.BeforeMin, searchingRange.BeforeMax);
                var afterList = await MatchingRedisJob.GetRangeListAsync(redis, key, searchingRange.AfterMin, searchingRange.AfterMax);


                if (beforeList.Length == 0 && afterList.Length == 0)
                {
                    return null;
                }

                if (beforeList.Length == 1 && afterList.Length == 0)
                {
                    if (beforeList[0].Element == member)
                        return null;
                    else
                        return beforeList[0];
                }

                if (beforeList.Length == 0 && afterList.Length == 1)
                    return afterList[0];

                if (beforeList.Length == 0)
                    return afterList[AkaRandom.Random.Next(afterList.Length)];

                if (afterList.Length == 0)
                    return beforeList[AkaRandom.Random.Next(beforeList.Length)];

                var BeforeListOrAfterList = AkaRandom.Random.Next(0, 2);

                if (BeforeListOrAfterList == 0)
                    return beforeList[AkaRandom.Random.Next(beforeList.Length)];
                else
                    return afterList[AkaRandom.Random.Next(afterList.Length)];
            }
            
        }

        public static async Task<long?> GetMyRank(IDatabase redis, string key, string member)
        {
            return await MatchingRedisJob.GetRankAsync(redis, key, member);
        }
        
        public static bool TryEnterRoom(string targetRoomId)
        {
            if (WaitingRooms.Instance.ContainsKey(targetRoomId))
            {
                if (WaitingRooms.Instance.TryEnterGuestPlayer(targetRoomId))
                    return true;
            }

            return false;
        }

        public static void RemoveRoom(string roomId)
        {
            WaitingRooms.Instance.Remove(roomId);
        }

        public static  async Task AddTeamRankScoreAsync(string roomId, IDatabase redis, string member, string key, int teamRankPoint)
        {
            await WaitingRooms.Instance.AddTeamRankScoreAsync(roomId, redis, member, key, teamRankPoint);
        }

        public static async Task ClearTeamRankScoreAsync(string roomId, IDatabase redis, string member)
        {
            await WaitingRooms.Instance.ClearTeamRankScoreAsync(roomId, redis, member);
        }
    }
}
