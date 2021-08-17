using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using Common.Entities.SquareObject;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.SquareObject
{
    public class WebSquareObjectStart : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var startInfo = requestInfo as ProtoSquareObjectStart;
            var res = new ProtoOnSquareObjectStart();

            using (var db = new DBContext(startInfo.UserId))
            {
                using (var accountDb = new DBContext("AccountDBSetting"))
                {
                    var squareObjectIo = new SquareObjectIO(db, accountDb);

                    var squareObject = await squareObjectIo.SelectSquareObjectWithState(startInfo.UserId);
                    res.SquareObjectInfo = squareObject;

                    await accountDb.BeginTransactionCallback(async () =>
                    {
                        return await db.BeginTransactionCallback(async () =>
                        {
                            if (squareObject.SquareObjectLevel == 0)
                            {
                                res.Result = SquareObjectResponseType.LockedContents;
                            }
                            else if (startInfo.ObjectLevel == 0 || squareObject.SquareObjectLevel < startInfo.ObjectLevel)
                            {
                                res.Result = SquareObjectResponseType.InvalidObjectLevel;
                            }
                            else if (true == IsNotFailed(squareObject, res)
                                    && await UseTicket(db, startInfo.UserId, res))
                            {
                                var squareObjectWork = new SquareObjectWork(squareObject);
                                squareObjectWork.Reset(startInfo.UserId, startInfo.ObjectLevel);
                                var result = await squareObjectIo.Start(squareObjectWork);

                                res.SquareObjectInfo = squareObjectWork.ObjectInfo;
                                res.Result = SquareObjectResponseType.Success;

                                AkaLogger.Log.User.SquareObject.StartLog(squareObjectWork.State.UserId, squareObjectWork.State.ActivatedTime, squareObjectWork.State.NextInvasionTime
                                                                        , squareObjectWork.State.NextInvasionLevel, squareObjectWork.State.NextInvasionMonsterId, squareObject.SquareObjectLevel
                                                                        , squareObjectWork.ObjectInfo.SquareObjectLevel, squareObjectWork.ObjectInfo.CoreLevel, squareObjectWork.ObjectInfo.AgencyLevel);
                                return result;
                            }
                            return false;
                        });
                    });
                }
            }
            return res;
        }

        private bool IsNotFailed(ProtoSquareObject squareObject, ProtoOnSquareObjectStart response)
        {
            bool result = true;
            if (squareObject.CurrentState.UserId == 0)
            {
                response.Result = SquareObjectResponseType.InvalidUserId;
                result = false;
            }
            else if (true == squareObject.CurrentState.IsActivated)
            {
                response.Result = SquareObjectResponseType.AlreadyStart;
                result = false;
            }
            else if (true == squareObject.CurrentState.EnableReward)
            {
                response.Result = SquareObjectResponseType.EnableReward;
                result = false;
            }
            return result;
        }

        private async Task<bool> UseTicket(DBContext db, uint userId, ProtoOnSquareObjectStart response)
        {
            var useCount = 1;
            var result = false;
            var ticket = new SquareObjectStartTicket(userId, db, useCount);
            if (!await ticket.IsEnoughCount())
            {
                response.Result = SquareObjectResponseType.NotEnoughtTicket;
            }
            else
            {
                await ticket.Use("SquareObjectStart");
                result = true;
            }
            response.RemainTicket =  await ticket.GetRemainCount(useCount);
            return result;
        }
    }
}
