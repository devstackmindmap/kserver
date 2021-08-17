using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingServer
{
    public class WaitingRooms
    {
        private ConcurrentDictionary<string, WaitingRoom> _rooms;

        private WaitingRooms()
        {
            _rooms = new ConcurrentDictionary<string, WaitingRoom>();
        }

        private static WaitingRooms _instance = null;
        public static WaitingRooms Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WaitingRooms();
                }
                return _instance;
            }
        }

        public bool ContainsKey(string key)
        {
            return _rooms.ContainsKey(key);
        }

        public void Add(string key, WaitingRoom room)
        {
            _rooms.TryAdd(key, room);   
        }

        public void Remove(string key)
        {
            WaitingRoom removedRoom;
            _rooms.TryRemove(key, out removedRoom);
        }

        public bool TryEnterGuestPlayer(string roomId)
        {
            return _rooms[roomId].TryEnterGuestPlayer();
        }

        public int Count()
        {
            return _rooms.Count;
        }

        public async Task AddTeamRankScoreAsync(string roomId, IDatabase redis, string member, string key, int teamRankPoint)
        {
            await _rooms[roomId].AddTeamRankScoreAsync(redis, member, key, teamRankPoint);
        }

        public async Task ClearTeamRankScoreAsync(string roomId, IDatabase redis, string member)
        {
            if (_rooms.ContainsKey(roomId))
                await _rooms[roomId].ClearTeamRankScoreAsync(redis, member);
        }
    }
}
