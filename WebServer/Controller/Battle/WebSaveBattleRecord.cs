using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using AkaDB.MySql;
using AkaSerializer;
using Common.Entities.Stage;
using CommonProtocol;

namespace WebServer.Controller.Battle
{
    public class WebSaveBattleRecord : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoBattleRecord;

            using (var db = new DBContext(req.UserId))
            {
                var battleRecord = new DBBattleRecord(db);
                await battleRecord.SaveRecord(req);
            }

            if (req.EnemyUserId != 0 )
            {
                using (var db = new DBContext(req.EnemyUserId))
                {
                    var myUserId = req.EnemyUserId;
                    req.EnemyUserId = req.UserId;
                    req.UserId = myUserId;
                    req.IsHost = false;

                    var battleRecord = new DBBattleRecord(db);
                    await battleRecord.SaveRecord(req);
                }
            }

            return new ProtoResult
            {
                ResultType = AkaEnum.ResultType.Success
            };
        }

    }
}
