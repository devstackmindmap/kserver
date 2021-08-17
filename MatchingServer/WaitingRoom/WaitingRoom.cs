using AkaRedisLogic;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchingServer
{
    public class WaitingRoom
    {
        private RoomStatus _roomStatus = RoomStatus.Waiting;
        private List<string> _rankKeys = new List<string>();

        public bool TryEnterGuestPlayer()
        {
            if (_roomStatus == RoomStatus.Waiting)
            {
                _roomStatus = RoomStatus.Matched;
                return true;
            }

            return false;
        }

        public async Task AddTeamRankScoreAsync(IDatabase redis, string member, string key, int teamRankPoint)
        {
            _rankKeys.Add(key);
            await MatchingRedisJob.AddTeamRankScoreAsync(redis, key, member, teamRankPoint);
        }

        public async Task ClearTeamRankScoreAsync(IDatabase redis, string member)
        {
            await Task.WhenAll(_rankKeys.Select(key => MatchingRedisJob.SortedSetRemoveAsync(redis, key, member)));
        }
    }
}
