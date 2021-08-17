using AkaDB.MySql;
using AkaEnum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using CommonProtocol;
using AkaUtility;
using AkaData;

namespace WebLogic.Friend
{
    public class FriendManager
    {
        StringBuilder _query = new StringBuilder();

        public async Task<ResultType> RequestFriend(uint userId, uint friendId)
        {
            using (var db = new DBContext(userId))
            {
                if (await IsAlreadyFriend(userId, friendId, db))
                    return ResultType.AlreadyFriend;

                if (await IsRequestedFriend(userId, friendId, db))
                    return ResultType.AlreadyRequestedFriend;
            }

            bool IsFriendToMeFriend = false;
            using (var db = new DBContext(friendId))
            {
                if (await IsMaxFriendCount(friendId, "request_friends", DataConstantType.MAX_FRIEND_REQUESTS_COUNT, db))
                    return ResultType.FullFriendsFriendRequests;

                if (await IsAlreadyFriend(friendId, userId, db))
                    IsFriendToMeFriend = true;
                else
                {

                    await AddRequestFriend(friendId, userId, db);
                    await RemoveRecommendFriend(friendId, userId, db);

                    using (var _db = new DBContext(userId))
                    {
                        await RemoveRecommendFriend(userId, friendId, _db);
                    }
                }
            }

            if (IsFriendToMeFriend)
            {
                await AddFriend(userId, friendId);
                return ResultType.AlreadyFriend;
            }

            return ResultType.Success;
        }

        public async Task AddFriend(uint userId, uint friendId, DBContext db = null)
        {
            if (db != null)
            {
                await RemoveRequestFriend(userId, friendId, db);
                await RemoveRecommendFriend(userId, friendId, db);
                await AddFriendWithDb(userId, friendId, db, "friends");
                return;
            }

            using (var _db = new DBContext(userId))
            {
                await RemoveRequestFriend(userId, friendId, _db);
                await RemoveRecommendFriend(userId, friendId, _db);
                await AddFriendWithDb(userId, friendId, _db, "friends");
            }
        }

        public async Task AddRequestFriend(uint userId, uint friendId, DBContext db = null)
        {
            if (db != null)
            {
                await AddFriendWithDb(userId, friendId, db, "request_friends");
                return;
            }

            using (var _db = new DBContext(userId))
            {
                await AddFriendWithDb(userId, friendId, _db, "request_friends");
            }
        }

        private async Task AddFriendWithDb(uint userId, uint friendId, DBContext db, string tableName)
        {
            _query.Clear();
            _query.Append("INSERT INTO ").Append(tableName).Append("(userId, friendId) VALUES (").Append(userId).Append(",").Append(friendId)
                .Append(") ON DUPLICATE KEY UPDATE friendId=").Append(friendId).Append(";");

            await db.ExecuteNonQueryAsync(_query.ToString());
        }

        public async Task<bool> IsAlreadyFriend(uint userId, uint friendId, DBContext db = null)
        {
            if (db != null)
                return await IsAlreadyFriendWithDb(userId, friendId, db, "friends");

            using (var _db = new DBContext(userId))
            {
                return await IsAlreadyFriendWithDb(userId, friendId, _db, "friends");
            }
        }

        public async Task<bool> IsRequestedFriend(uint userId, uint friendId, DBContext db = null)
        {
            if (db != null)
                return await IsAlreadyFriendWithDb(userId, friendId, db, "request_friends");

            using (var _db = new DBContext(userId))
            {
                return await IsAlreadyFriendWithDb(userId, friendId, _db, "request_friends");
            }
        }

        private async Task<bool> IsAlreadyFriendWithDb(uint userId, uint friendId, DBContext db, string tableName)
        {
            _query.Clear();
            _query.Append("SELECT userId FROM ").Append(tableName).Append(" WHERE userId=").Append(userId)
                .Append(" AND friendId=").Append(friendId).Append(";");

            using (var cursor = await db.ExecuteReaderAsync(_query.ToString()))
            {
                if (cursor.Read())
                    return true;

                return false;
            }
        }

        public async Task RemoveFriendJobs(uint userId, uint friendId, DBContext db)
        {
            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$userId", userId),
                new InputArg("$friendId", friendId));
            await db.CallStoredProcedureAsync("p_deleteFriend", paramInfo);
        }

        public async Task RemoveRequestFriend(uint userId, uint friendId, DBContext db = null)
        {
            _query.Clear();
            _query.Append("DELETE FROM request_friends WHERE userId=").Append(userId).Append(" AND friendId=")
                .Append(friendId).Append(";");

            if (db != null)
            {
                await db.ExecuteNonQueryAsync(_query.ToString());
                return;
            }

            using (var _db = new DBContext(userId))
            {
                await db.ExecuteNonQueryAsync(_query.ToString());
            }
        }

        public async Task RemoveRecommendFriend(uint userId, uint friendId, DBContext db = null)
        {
            _query.Clear();
            _query.Append("DELETE FROM recommend_friends WHERE userId=").Append(userId).Append(" AND friendId=")
                .Append(friendId).Append(";");

            if (db != null)
            {
                await db.ExecuteNonQueryAsync(_query.ToString());
                return;
            }

            using (var _db = new DBContext(userId))
            {
                await db.ExecuteNonQueryAsync(_query.ToString());
            }
        }

        public async Task<List<uint>> GetFriendsUserIds(uint userId, DBContext db)
        {
            return await GetUserIds(userId, db, "friends");
        }

        public async Task<List<ProtoFriendInfo>> GetFriendList(uint userId, DBContext accountDb, DBContext userDb)
        {
            var ids = await GetUserIds(userId, userDb, "friends");
            return await GetUserInfos(ids, accountDb);
        }

        public async Task<List<ProtoFriendInfo>> GetRequestedFriendList(uint userId, DBContext accountDb, DBContext userDb)
        {
            var ids = await GetUserIds(userId, userDb, "request_friends");
            return await GetUserInfos(ids, accountDb);
        }

        public async Task<List<ProtoFriendInfo>> GetRecommendFriendList(uint userId, DBContext accountDb, DBContext userDb)
        {
            var ids = await GetRecommendUserIds(userId, userDb);
            return await GetUserInfos(ids, accountDb);
        }

        private async Task<List<uint>> GetUserIds(uint userId, DBContext userDb, string tableName)
        {
            var ids = new List<uint>();
            _query.Clear();
            _query.Append("SELECT friendId FROM ").Append(tableName).Append(" WHERE userId=").Append(userId).Append(";");

            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    ids.Add((uint)cursor["friendId"]);
                }

                return ids;
            }
        }

        private async Task<List<uint>> GetRecommendUserIds(uint userId, DBContext userDb)
        {
            var ids = new List<uint>();
            _query.Clear();
            _query.Append("SELECT friendId FROM recommend_friends WHERE userId=").Append(userId)
                .Append(" AND isSigned=0 AND recommendDateTime > '").Append(DateTime.UtcNow.AddDays(-3).ToTimeString()).Append("';");

            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    ids.Add((uint)cursor["friendId"]);
                }

                return ids;
            }
        }

        public async Task<List<ProtoFriendInfo>> GetUserInfos(IEnumerable<uint> ids, DBContext db)
        {
            var friendInfos = new List<ProtoFriendInfo>();
            if (ids.Any() == false)
                return friendInfos;

            var strIds = string.Join(",", ids.Select(id => id.ToString()));

            _query.Clear();
            _query.Append("SELECT userId, nickName, loginDateTime, " +
                "currentSeason, currentSeasonRankPoint, nextSeasonRankPoint, profileIconId, clanName " +
                ", isActivatedSO " +
                "FROM accounts ")
                .Append("WHERE userId IN (").Append(strIds).Append(");");

            using (var cursor = await db.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    friendInfos.Add(new ProtoFriendInfo
                    {
                        UserId = (uint)cursor["userId"],
                        Nickname = (string)cursor["nickName"],
                        CurrentSeason = (int)cursor["currentSeason"],
                        CurrentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"],
                        NextSeasonRankPoint = (int)cursor["nextSeasonRankPoint"],
                        RecentLoginDateTime = ((DateTime)cursor["loginDateTime"]).Ticks,
                        ProfileIconId = (uint)cursor["profileIconId"],
                        ClanName = (string)cursor["clanName"],
                        IsActivatedSquareObject = 0 != (int)cursor["isActivatedSO"],
                    });
                }

                return friendInfos;
            }
        }

        public async Task<ProtoFriendInfo> GetUserInfo(uint userId, DBContext db)
        {
            _query.Clear();
            _query.Append("SELECT userId, nickName, loginDateTime, " +
                "currentSeason, currentSeasonRankPoint, nextSeasonRankPoint, profileIconId, clanName " +
                ", isActivatedSO " +
                "FROM accounts ")
                .Append("WHERE userId=").Append(userId).Append(";");

            using (var cursor = await db.ExecuteReaderAsync(_query.ToString()))
            {
                if (cursor.Read())
                {
                    return new ProtoFriendInfo
                    {
                        ResultType = ResultType.Success,
                        UserId = (uint)cursor["userId"],
                        Nickname = (string)cursor["nickName"],
                        CurrentSeason = (int)cursor["currentSeason"],
                        CurrentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"],
                        NextSeasonRankPoint = (int)cursor["nextSeasonRankPoint"],
                        RecentLoginDateTime = ((DateTime)cursor["loginDateTime"]).Ticks,
                        ProfileIconId = (uint)cursor["profileIconId"],
                        ClanName = (string)cursor["clanName"],
                        IsActivatedSquareObject = 0 != (int)cursor["isActivatedSO"],
                    };
                }

                return null;
            }
        }

        public async Task AddRecommendFriend(uint userId, uint friendId, DBContext db)
        {
            _query.Clear();
            var strNow = DateTime.UtcNow.ToTimeString();
            _query.Append("INSERT INTO recommend_friends (userId, friendId, isSigned, recommendDateTime) ")
                .Append("VALUES (").Append(userId).Append(",").Append(friendId).Append(", 0,'").Append(strNow).Append("') ")
                .Append("ON DUPLICATE KEY UPDATE isSigned=0, recommendDateTime = '").Append(strNow).Append("';");

            await db.ExecuteNonQueryAsync(_query.ToString());
        }

        public async Task<uint> GetInviteCodeUserId(DBContext db, string InviteCode)
        {
            _query.Clear();
            _query.Append("SELECT userId FROM accounts WHERE friendCode='").Append(InviteCode).Append("';");
            using (var cursor = await db.ExecuteReaderAsync(_query.ToString()))
            {
                if (cursor.Read() == false)
                    return 0;

                return (uint)cursor["userId"];
            }
        }

        public async Task<bool> IsMaxFriendCount(uint userId, string tableName, DataConstantType dataConstantType, DBContext db = null)
        {
            if (db != null)
            {
                return await IsMaxFriendCountDb(userId, tableName, dataConstantType, db);
            }

            using (var _db = new DBContext(userId))
            {
                return await IsMaxFriendCountDb(userId, tableName, dataConstantType, _db);
            }
        }

        private async Task<bool> IsMaxFriendCountDb(uint userId, string tableName, DataConstantType dataConstantType, DBContext db = null)
        {
            _query.Clear();
            _query.Append("SELECT count(friendId) AS count FROM ").Append(tableName)
                .Append(" WHERE userId = ").Append(userId).Append(";");

            using (var cursor = await db.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return false;

                var currentFriendCount = Convert.ToInt32(cursor["count"]);
                var maxFriendCount = (int)Data.GetConstant(dataConstantType).Value;

                if (currentFriendCount >= maxFriendCount)
                    return true;
            }
            return false;
        }

    }
}
