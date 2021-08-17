using AkaEnum;
using AkaEnum.Battle;
using BattleLogic;
using CommonProtocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BattleServer
{
    public static class RoomManager
    {
        private static ConcurrentDictionary<string, Room> _rooms = new ConcurrentDictionary<string, Room>();

        internal static int RoomCount => _rooms.Count;

        public static RoomStatus GetRoomStatus(string roomId)
        {
            return InRoom(roomId, room => room.Status );
        }

        public static bool MakeRoom(string roomId, BattleInfo battleInfo)
        {
            Room room = RoomFactory.CreateRoom(battleInfo.BattleType, roomId, battleInfo);
            return _rooms.TryAdd(roomId, room);
        }

        public static bool TryEnterGuestPlayer(string roomId, BattleInfo playerInfo)
        {
            if (false == InRoom(roomId, room => room.TryEnterGuestPlayer(playerInfo)))
            {
                RemoveRoom(roomId);
                return false;
            }
            return true;
        }

        public static async Task<ResultType> BattleInitialize(string roomId)
        {
            if (true == _rooms.TryGetValue(roomId, out var room))
            {
                return await room.BattleInitialize();
            }
            return ResultType.Success;
        }

        public static PlayerInfo GetPlayerInfo(string roomId)
        {
            return InRoom(roomId, room => room.PlayerInfo);
        }

        public static void BattleStart(string roomId)
        {
            InRoom(roomId, room => room.BattleStart());
        }

        public static void FillBattleStartInfo(string roomId, Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts)
        {
            InRoom(roomId, room => room.FillBattleStartInfo(protoBattleStarts));
        }

        public static void ReEnterRoom(string roomId, uint userId, string sessionId, ProtoCurrentBattleStatus protoCurrentBattleStatus)
        {
            InRoom(roomId, room => room.ReEnterRoom(userId, sessionId, protoCurrentBattleStatus));
        }

        public static string GetPlayer1SessionId(string roomId)
        {
            return InRoom(roomId, room => room.GetPlayer1SessionId());
        }

        public static void RemoveRoom(string roomId)
        {
            _rooms.TryRemove(roomId, out var outObject);
        }

        public static void CardUse(ProtoCardUse protoCardUse)
        {
            InRoom(protoCardUse.RoomId, room => room.CardUse(protoCardUse));
        }

        public static void EmoticonUse(ProtoEmoticonUse protoEmoticonUse)
        {
            InRoom(protoEmoticonUse.RoomId, room => room.EmoticonUse(protoEmoticonUse));
        }

        public static string GetPlayer1MemberKey(string roomId)
        {
            return InRoom(roomId, room => room.GetPlayer1MemberKey());
        }

        public static void Retreat(ProtoRetreat protoRetreat, uint userId)
        {
            InRoom(protoRetreat.RoomId, room => room.Retreat(protoRetreat, userId));
        }

        public static void BattleReady(ProtoBattleReady protoBattleReady)
        {
            InRoom(protoBattleReady.RoomId, room => room.BattleReady(protoBattleReady.UserId));

        }

        public static async Task RemoveRoomForException(string roomId)
        {
            if (true == _rooms.TryRemove(roomId, out var room))
            {
                await room.DeleteBattlePlayerInfoRedisKey();
            }
        }

        private static bool InRoom(string roomId, Action<Room> action)
        {
            if (true == _rooms.TryGetValue(roomId, out var room))
            {
                action(room);
            }
            return false;
        }

        private static Result InRoom<Result>(string roomId, Func<Room, Result> action)
        {
            if (true == _rooms.TryGetValue(roomId, out var room))
            {
                return action(room);
            }
            return default(Result);
        }
    }
}
