using AkaData;
using AkaDB.MySql;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebLogic.User.DataUpdator
{
    public class EmoticonUpdator : UserInitDataUpdator
    {
        internal EmoticonUpdator(uint userId, DBContext db, IEnumerable<uint> updateIdList ) : base(userId,db,updateIdList)
        {
        }
        
        public override async Task Run()
        {
            var targetTable = TableName.EMOTICON;
            var insertColumns = "(userId, id, orderNum, unitId)";
            var insertValues = string.Join(",", UpdateIdList.Select(id => $"({StrUserId}, " +
            $"{id.ToString()}, -1, {Data.GetEmoticon(id).UnitId})"));
            await ExecuteInsertQuery(targetTable, insertColumns, insertValues);
        }
    }
}
