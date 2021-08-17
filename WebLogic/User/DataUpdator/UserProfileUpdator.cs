using AkaDB.MySql;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebLogic.User.DataUpdator
{
    public class UserProfileUpdator : UserInitDataUpdator
    {
        internal UserProfileUpdator(uint userId, DBContext db, IEnumerable<uint> updateIdList ) : base(userId,db,updateIdList)
        {
        }
        
        public override async Task Run()
        {
            var targetTable = TableName.PROFILE;
            var insertColumns = "(userId, id)";
            var insertValues = string.Join(",", UpdateIdList.Select(id => $"({StrUserId}, {id.ToString()})"));
            await ExecuteInsertQuery(targetTable, insertColumns, insertValues);
        }
    }
}
