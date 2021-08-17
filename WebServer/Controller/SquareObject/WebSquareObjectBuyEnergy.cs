using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using Common.Entities.Season;
using Common.Entities.SquareObject;
using Common.Pass;
using CommonProtocol;
using System;
using System.Threading.Tasks;

namespace WebServer.Controller.SquareObject
{
    public class WebSquareObjectBuyEnergy : BaseController
    {


        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var userId = (requestInfo as ProtoUserId).UserId;
            var res = new ProtoOnSquareObjectBuyEnergy() { Result = SquareObjectResponseType.Fail };
            
            using (var db = new DBContext(userId))
            {
                var result = await db.BeginTransactionCallback(async () =>
                {
                    var refreshBaseHour = (int)(Data.GetConstant(DataConstantType.START_DAY_BASE_HOUR).Value + float.Epsilon);
                    var utcNow = DateTime.UtcNow.AddHours(-refreshBaseHour);
                    var initDateTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, refreshBaseHour, 0, 0, DateTimeKind.Utc);

                    var squareObjectIo = new SquareObjectIO(db, null);
                    var squareObject = await squareObjectIo.SelectSquareObjectWithState(userId);

                    res.Result = IsNotFailed(squareObject, initDateTime);
                    if (res.Result != SquareObjectResponseType.Success)
                    {
                        return false;
                    }

                    var needMetals = (int)(Data.GetConstant(DataConstantType.SQUARE_OBJECT_BUY_ENERGY).Value + float.Epsilon);
                    var material = MaterialFactory.CreateMaterial(MaterialType.Gem, userId, needMetals, db);
                    if (material != null && false == await material.IsEnoughCount())
                    {
                        res.Result = SquareObjectResponseType.NeedMaterials;
                        return false;
                    }

                    await material.Use("SquareObject_BuyEnergy");
                    res.RemainedGem = await material.GetRemainCount();

                    var maxEnergy = Data.GetSquareObjectPlanetCore(squareObject.CoreLevel).MaxPlanetCoreEnergy;
                    res.InjectedTime = DateTime.UtcNow;
                    if (false == await squareObjectIo.AddExtraCoreEnergy(userId, maxEnergy, res.InjectedTime, initDateTime))
                    {
                        res.Result = SquareObjectResponseType.Fail;
                        res.RemainedGem = 0;
                        return false;
                    }
                    return true;
                });
            }   
            return res;
        }

        private SquareObjectResponseType IsNotFailed(ProtoSquareObject squareObject, DateTime initDateTime)
        {

            var waitingInvasaionTime = squareObject.CurrentState.NextInvasionTime.AddSeconds(-Common.ConstValue.SQUARE_OBJECT_INVADING_SECOND);

            if (squareObject.CurrentState.UserId == 0)
            {
                return SquareObjectResponseType.InvalidUserId;
            }
            else if (false == squareObject.CurrentState.IsActivated)
            {
                return SquareObjectResponseType.NotActivated;
            }
            else if (DateTime.UtcNow >= waitingInvasaionTime)
            {
                return SquareObjectResponseType.Invading;
            }
            else if (squareObject.CurrentState.ExtraEnergyInjectedTime > initDateTime)
            {
                return SquareObjectResponseType.CantBuyEnergy;
            }

            return SquareObjectResponseType.Success;
        }
    }
}
