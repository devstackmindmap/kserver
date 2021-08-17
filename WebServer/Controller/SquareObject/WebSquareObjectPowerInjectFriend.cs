using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using Common.Entities.SquareObject;
using AkaData;

namespace WebServer.Controller.SquareObject
{
    public class WebSquareObjectPowerInjectFriend : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var powerInjection = requestInfo as ProtoUserIdTargetId;

            using (DBContext accountDb = new DBContext("AccountDBSetting")
                            ,friendDb = new DBContext(powerInjection.TargetId)
                            , myDb = new DBContext(powerInjection.UserId))
            {
                var res = new ProtoOnSquareObjectPowerInjectFriend();
                
                var mySquareObjectIo = new SquareObjectIO(myDb, accountDb);
                var donationCount = await mySquareObjectIo.GetDonationCount(powerInjection.UserId);
                var maxDonationCount = (int)(float.Epsilon + Data.GetConstant(DataConstantType.SQUARE_OBJECT_MAX_DONATION).Value);

                if (donationCount < maxDonationCount)
                {
                    var friendSquareObjectIo = new SquareObjectIO(friendDb, accountDb);
                    var friendSquareObject = await friendSquareObjectIo.SelectSquareObjectWithState(powerInjection.TargetId);
                    var squareObjectWork = new SquareObjectWork(friendSquareObject);
                    var oldEnergyRefreshTime = squareObjectWork.State.EnergyRefreshTime;
                    squareObjectWork.UseEnergy(0, 0, false);

                    await accountDb.BeginTransactionCallback(async () =>
                    {
                        res.Result = await friendSquareObjectIo.DonatePower(powerInjection.TargetId, powerInjection.UserId);
                        if (res.Result != SquareObjectResponseType.Success)
                            return false;

                        res.Result = SquareObjectResponseType.NotActivated;
                        var updateResult = await friendDb.BeginTransactionCallback(async () =>
                        {
                            var donatePower = (int)(float.Epsilon + Data.GetConstant(DataConstantType.SQUARE_OBJECT_POWER_DONATION_VALUE).Value);
                            squareObjectWork.ObjectInfo.CurrentState.SquareObjectPower += donatePower;
                            return await friendSquareObjectIo.UpdateEnergy(squareObjectWork.ObjectInfo, false, oldEnergyRefreshTime);
                        });

                        if (updateResult)
                        {
                            friendDb.Commit();
                            res.Result = SquareObjectResponseType.Success;
                            await mySquareObjectIo.AddExtraCoreEnergy(powerInjection.UserId);
                        }
                        return updateResult;
                    });
                }
                else
                {
                    res.Result = SquareObjectResponseType.MaxDonate;
                }

                    
                AkaLogger.Log.User.SquareObject.Donate(powerInjection.UserId, powerInjection.TargetId, res.Result);
                return res;
                
            }
        }
    }
}
