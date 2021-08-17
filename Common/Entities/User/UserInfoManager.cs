using AkaData;
using AkaDB.MySql;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.User
{
    public class UserInfoManager
    {
        StringBuilder _query = new StringBuilder();

        public async Task<uint> GetUserId(string nickname, DBContext accountDb)
        {
            _query.Clear();
            _query.Append("SELECT userId FROM accounts WHERE nickname='").Append(nickname).Append("';");
            var cursor = await accountDb.ExecuteReaderAsync(_query.ToString());
            if (false == cursor.Read())
                return 0;

            return (uint)cursor["userId"];
        }


        public async Task<IEnumerable<(uint sendUserId, uint receivedUserId)>> GetSquareObjectDonatedMeUsers(uint userId, DateTime initDateTime, DBContext accountDb)
        {
            _query.Clear();
            _query.Append("SELECT sendUserId, receivedUserId  FROM square_object_friends WHERE ").Append(userId).Append(" IN (receivedUserId ,sendUserId) ")
                  .Append(" AND receivedDate >= '").Append(initDateTime.ToTimeString()).Append("';");
            using (var cursor = await accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                return cursor.Cast<System.Data.IDataRecord>().Select(record => ((uint)record["sendUserId"] , (uint)record["receivedUserId"])).ToList();
            }
        }

        public async Task<IEnumerable<uint>> GetSquareObjectNeedHelpUsers(uint userId, IEnumerable<uint> userIdList, DateTime initDateTime, DBContext accountDb)
        {
            if (false == userIdList.Any())
                return Enumerable.Empty<uint>();

            var maxReceiveCount = (int)(float.Epsilon + Data.GetConstant(AkaEnum.DataConstantType.SQUARE_OBJECT_MAX_RECEIVE_DONATION).Value);
            _query.Clear();
            _query.Append("SELECT receivedUserId FROM square_object_friends "
                        + "WHERE receiveduserid IN (").Append(string.Join(",", userIdList))
                  .Append(") AND receivedDate >= '").Append(initDateTime.ToTimeString())
                  .Append("' GROUP BY receivedUserId HAVING  COUNT(*) >=").Append(maxReceiveCount)
                  .Append(" OR MAX( IF( sendUserId = ").Append(userId).Append(", 1,0)) != 0;");
            using (var cursor = await accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                return userIdList.Except(cursor.Cast<System.Data.IDataRecord>().Select(record => (uint)record["receivedUserId"])).ToList();
            }
        }
    }
}
