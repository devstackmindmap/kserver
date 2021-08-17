using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.SquareObject;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Controller.SquareObject
{
    public class WebGetSquareObject : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var stopSquareObject = requestInfo as ProtoUserId;
            var res = new ProtoOnGetSquareObject();

            using (var db = new DBContext(stopSquareObject.UserId))
            {
                using (var accountDb = new DBContext("AccountDBSetting"))
                {
                    var squareObjectIo = new SquareObjectIO(db, accountDb);
                    var squareObject = await squareObjectIo.SelectSquareObjectWithState(stopSquareObject.UserId);
                    res.SquareObjectInfo = squareObject;
                    res.Result = IsNotFailed(squareObject);
                    if ( SquareObjectResponseType.Success == res.Result)
                    {
                        var squareObjectWork = new SquareObjectWork(squareObject);

                        var maintenenceTimes = await GetMaintenenceTimes(accountDb, squareObjectWork.State.NextInvasionTime);
                        var firstInvasionTime = squareObjectWork.State.NextInvasionTime;
                        if (true == squareObjectWork.IsInvade(DateTime.UtcNow, maintenenceTimes))
                        {
                            await accountDb.BeginTransactionCallback(async () =>
                            {
                                var updated = await db.BeginTransactionCallback(async () =>
                                {
                                    return await squareObjectIo.UpdateInvasionResult(squareObjectWork, firstInvasionTime);
                                });

                                if (false == updated)
                                {
                                    squareObject = await squareObjectIo.SelectSquareObjectWithState(stopSquareObject.UserId);
                                    squareObjectWork = new SquareObjectWork(squareObject);
                                }
                                return updated;
                            });
                        }

                        res.SquareObjectInfo = squareObjectWork.ObjectInfo;
                        res.Result = IsNotFailed(res.SquareObjectInfo);
                    }         
                }
            }
            return res;
        }

        private async Task<List<(DateTime startTime, DateTime endTime)>> GetMaintenenceTimes(DBContext db, DateTime nextInvasionTime)
        {
            string query = "SELECT startTime, endTime FROM _maintenance_time WHERE endTime >= '" + nextInvasionTime.ToTimeString() + "';";

            List<(DateTime startTime, DateTime endTime)> maintenenceTimes = new List<(DateTime startTime, DateTime endTime)>();
            
            using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
            {

                while (true == cursor.Read())
                {
                    maintenenceTimes.Add( (new DateTime(cursor.GetDateTime(0).Ticks, DateTimeKind.Utc), new DateTime(cursor.GetDateTime(1).Ticks, DateTimeKind.Utc)) );
                }
            }
            return maintenenceTimes.OrderBy( time => time.startTime.TimeOfDay).ToList();
        }

        private SquareObjectResponseType IsNotFailed(ProtoSquareObject squareObject)
        {
            if (squareObject.CurrentState.UserId == 0)
            {
                return SquareObjectResponseType.InvalidUserId;
            }
            else if (false == squareObject.CurrentState.IsActivated)
            {
                return SquareObjectResponseType.NotActivated;
            }
            return SquareObjectResponseType.Success;
        }


    }
}
