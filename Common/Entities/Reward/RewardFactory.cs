using AkaData;
using AkaDB.MySql;
using System.Collections.Generic;

namespace Common.Entities.Box
{
    public static class RewardFactory
    {
        public static IReward CreateBox(DBContext db, uint userId, uint classId, List<List<DataItem>>  listItems, uint itemValue)
        {
            return new Box(db, userId, classId, listItems, itemValue);
        }
    }
}
