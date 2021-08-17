using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Season;
using Common.Entities.SquareObject;
using Common.Pass;
using CommonProtocol;
using System;
using System.Threading.Tasks;

namespace WebServer.Controller.SquareObject
{
    public class WebSquareObjectPowerInject : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var powerInjection = requestInfo as ProtoSquareObjectPowerInject;
            var res = new ProtoOnSquareObjectPowerInject();

            using (var db = new DBContext(powerInjection.UserId))
            {
                using (var accountDb = new DBContext("AccountDBSetting"))
                {
                    var squareObjectIo = new SquareObjectIO(db, accountDb);
                    var squareObject = await squareObjectIo.SelectSquareObjectWithState(powerInjection.UserId);
                    res.SquareObjectInfo = squareObject;

                    if (true == IsNotFailed(squareObject, res))
                    {
                        var hasSeasonPass = await HasPremiumPass(powerInjection.UserId, db, accountDb);
                        var squareObjectWork = new SquareObjectWork(squareObject);
                        var oldEnergyRefreshTime = squareObjectWork.State.EnergyRefreshTime;
                        var isUsedEnergy = squareObjectWork.UseEnergy(powerInjection.SquareObjectEnergy, powerInjection.AgencyEnergy, hasSeasonPass);

                        if (isUsedEnergy)
                        {
                            var updated = false;
                            if (squareObjectWork.IsMaxPlanetBoxExp())
                            {
                                updated = await accountDb.BeginTransactionCallback(async () =>
                                {
                                    return await db.BeginTransactionCallback(async () =>
                                    {
                                        squareObjectWork.Stop();
                                        if (true == await squareObjectIo.UpdateEnergy(squareObjectWork.ObjectInfo, true, oldEnergyRefreshTime)
                                            && true == await squareObjectIo.Stop(squareObjectWork.ObjectInfo))
                                        {
                                            res.RewardList = await squareObjectIo.GetRewards(squareObjectWork.ObjectInfo);
                                            return true;
                                        }
                                        return updated;
                                    });
                                });

                            }
                            else
                            {
                                updated = await squareObjectIo.UpdateEnergy(squareObjectWork.ObjectInfo, false, oldEnergyRefreshTime);
                            }

                            if (true == updated)
                            {
                                res.SquareObjectInfo = squareObjectWork.ObjectInfo;
                                res.Result = SquareObjectResponseType.Success;
                            }

                            AkaLogger.Log.User.SquareObject.UseEnergyLog(squareObjectWork.State.UserId, powerInjection.AgencyEnergy, powerInjection.SquareObjectEnergy
                                                                        , squareObjectWork.State.CurrentPlanetBoxLevel, squareObjectWork.State.SquareObjectPower, squareObject.CoreLevel, squareObject.AgencyLevel
                                                                        , updated);
                        }
                        else
                        {
                            res.Result = SquareObjectResponseType.NotEnoughEnergy;
                        }
                    }
                }
            }
            return res;
        }

        private async Task<bool> HasPremiumPass(uint userId, DBContext userDb, DBContext accountDb)
        {
            var serverSeason = new ServerSeason(accountDb);
            var seasonInfo = await serverSeason.GetSeasonPassInfo();
            var purchasedSeasons = await (new SeasonPassManager(userId, seasonInfo.CurrentSeason, userDb))
                .GetBeforeAndCurrentPurchasedSeasonPassList();
            return purchasedSeasons.Contains(seasonInfo.CurrentSeason);
        }

        private bool IsNotFailed(ProtoSquareObject squareObject, ProtoOnSquareObjectPowerInject response)
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
            return result;
        }
    }
}
