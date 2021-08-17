
using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AkaEnum.Battle;
using System.Text;
using AkaData;

namespace Common.Entities.Stage
{
    public class StageUnlocker : IContents
    {
        private DBContext _db;
        private uint _userId;
        private uint _stageLevelId;


        public StageUnlocker(DBContext db, uint userId, uint stageLevelId = 0)
        {
            _db = db;
            _userId = userId;
            _stageLevelId = stageLevelId;
        }



        public async Task<bool> UnlockContents(uint userId)
        {
            var newStageLevel = Data.GetStageLevel(_stageLevelId);
            if (newStageLevel != null)
            {
                var query = new StringBuilder();
                query.Append("INSERT IGNORE INTO `stage_levels` (`userId`, `stageLevelId`, `clearCount`) VALUES (")
                     .Append(userId.ToString()).Append(",")
                     .Append(_stageLevelId.ToString())
                     .Append(",0 );");
                await _db.ExecuteNonQueryAsync(query.ToString());
            }
            return false;
        }
    }
}
