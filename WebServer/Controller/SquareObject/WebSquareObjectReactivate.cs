using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using Common.Entities.SquareObject;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.SquareObject
{
    public class WebSquareObjectReactivate : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var userId = (requestInfo as ProtoUserId).UserId;
            var res = new ProtoOnSquareObjectStart();

            using (var db = new DBContext(userId))
            {
                using (var accountDb = new DBContext("AccountDBSetting"))
                {

                    await accountDb.BeginTransactionCallback(async () =>
                    {
                        var squareObjectIo = new SquareObjectIO(db, accountDb);
                        var squareObject = await squareObjectIo.SelectSquareObjectWithState(userId);
                        res.SquareObjectInfo = squareObject;

                        return await db.BeginTransactionCallback(async () =>
                        {
                            if (true == IsNotFailed(squareObject, res) && true == await UseTicket(db, userId, res))
                            {
                                var squareObjectWork = new SquareObjectWork(squareObject);
                                squareObjectWork.Recovery();
                                var result = await squareObjectIo.Start(squareObjectWork);

                                res.SquareObjectInfo = squareObjectWork.ObjectInfo;
                                res.Result = SquareObjectResponseType.Success;

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
            else if (squareObject.CurrentState.CurrentShield > 0)
            {
                response.Result = SquareObjectResponseType.NotDestroyed;
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
                await ticket.Use("SquareObjectReacivate");
                result = true;
            }
            response.RemainTicket =  await ticket.GetRemainCount(useCount);
            return result;
        }
    }
}
