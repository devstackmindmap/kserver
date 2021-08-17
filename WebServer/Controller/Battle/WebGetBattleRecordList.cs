using System.Threading.Tasks;
using AkaDB.MySql;
using Common.Entities.Stage;
using CommonProtocol;

namespace WebServer.Controller.Battle
{
    public class WebGetBattleRecordList : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoGetBattleRecordList;
            ProtoOnGetBattleRecordList recordList = null;
            using (var db = new DBContext(req.UserId))
            {
                var battleRecord = new DBBattleRecord(db);
                recordList = await battleRecord.GetRecordList(req);
            }
            return recordList;
        }
    }
}
