using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.SquareObject;
using CommonProtocol;
using System;
using System.Threading.Tasks;

namespace WebServer.Controller.SquareObject
{
    public class WebSquareObjectStop : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var stopSquareObject = requestInfo as ProtoUserId;
            var res = new ProtoOnSquareObjectStop();

            using (var db = new DBContext(stopSquareObject.UserId))
            {
                using (var accountDb = new DBContext("AccountDBSetting"))
                {
                    var squareObjectIo = new SquareObjectIO(db, accountDb);
                    var squareObject = await squareObjectIo.SelectSquareObjectWithState(stopSquareObject.UserId);
                    res.SquareObjectInfo = squareObject;

                    if (true == IsNotFailed(squareObject, res))
                    {
                        var squareObjectWork = new SquareObjectWork(squareObject);

                        squareObjectWork.Stop();

                        var updated = await accountDb.BeginTransactionCallback(async () =>
                        {
                            return await db.BeginTransactionCallback(async () =>
                            {
                                if (true == await squareObjectIo.Stop(squareObjectWork.ObjectInfo))
                                {
                                    res.RewardList = await squareObjectIo.GetRewards(squareObjectWork.ObjectInfo);
                                    return true;
                                }
                                return false;
                            });
                        });

                        if (true == updated)
                        {
                            res.SquareObjectInfo = squareObjectWork.ObjectInfo;
                            res.Result = SquareObjectResponseType.Success;
                        }
                    }
                }
            }
            return res;
        }

        private bool IsNotFailed(ProtoSquareObject squareObject, ProtoOnSquareObjectStop response)
        {
            bool result = true;
            var now = DateTime.UtcNow;
            var waitingInvasaionTime = squareObject.CurrentState.NextInvasionTime.AddSeconds(-Common.ConstValue.SQUARE_OBJECT_INVADING_SECOND);
            if (squareObject.CurrentState.UserId == 0)
            {
                response.Result = SquareObjectResponseType.InvalidUserId;
                result = false;
            }
            else if (false == squareObject.CurrentState.IsActivated)
            {
                response.Result = SquareObjectResponseType.NotActivated;
                result = false;
            }
            else if (now >= waitingInvasaionTime)
            {
                response.Result = SquareObjectResponseType.Invading;
                result = false;
            }
            else {
                var elapsedTime = (now - squareObject.CurrentState.ActivatedTime).TotalMinutes;
                if (elapsedTime < (Data.GetConstant(DataConstantType.SQUARE_OBJECT_LIMIT_TIME_OF_PLANET_BOX_OPEN).Value))
                {
                    response.Result = SquareObjectResponseType.NotEnoughElapsedStopTime;
                    result = false;
                }
            }
            return result;
        }
    }
}
