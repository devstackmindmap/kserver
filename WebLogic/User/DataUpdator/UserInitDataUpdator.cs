using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common;
using Common.Entities.Charger;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLogic.User.DataUpdator
{
    public  class UserInitDataUpdator
    {
        protected DBContext DB { get; private set; }
        protected string StrUserId { get; private set; }
        protected IEnumerable<uint> UpdateIdList { get; private set; }
        protected UserInitDataType UserInitDataType { get; private set; }

        protected UserInitDataUpdator(uint userId, DBContext db, IEnumerable<uint> updateIdList )
        {
            DB = db;
            StrUserId = userId.ToString();
            UpdateIdList = updateIdList;
        }

        public virtual async Task Run() { }

        public static UserInitDataUpdator CreateUpdator(uint userId, DBContext db, DBContext accountDb, UserInitDataType userInitDataType, IEnumerable<uint> updateIdList)
        {
            switch (userInitDataType)
            {
                case UserInitDataType.Quest:
                    return new QuestUpdator(userId, db, updateIdList);
                case UserInitDataType.Unit:
                    return new UnitUpdator(userId, db, updateIdList);
                case UserInitDataType.Emoticon:
                    return new EmoticonUpdator(userId, db, updateIdList);
                case UserInitDataType.UserProfile:
                    return new UserProfileUpdator(userId, db, updateIdList);
                case UserInitDataType.DBSquareObjectFriend:
                    return new SquareObjectFriendUpdator(userId, accountDb);
                default:
                    return null;
            }
        }

        protected async Task ExecuteInsertQuery(string targetTable, string insertColumns, string insertValues)
        {
            if (insertValues.Length == 0)
                return;
            var query = new StringBuilder();
            query.Append("INSERT IGNORE INTO ")
                    .Append(targetTable)
                    .Append(insertColumns)
                    .Append(" VALUES ")
                    .Append(insertValues)
                    .Append(";");
            await DB.ExecuteNonQueryAsync(query.ToString());
            query.Clear();
        }


    }
}
