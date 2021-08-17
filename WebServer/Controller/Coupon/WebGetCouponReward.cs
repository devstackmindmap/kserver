using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System;
using System.Threading.Tasks;
using WebLogic.Coupon;
using System.Collections.Generic;
using Common.Entities.Product;

namespace WebServer.Controller.Coupon
{
    public class WebGetCouponReward : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdStringValue;

            
            List<List<ProtoItemResult>> itemReward = null;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var couponManager = new CouponManager(accountDb, req.UserId, req.Value);
                if (false == await couponManager.GetCouponInfo())
                    return new ProtoGetCouponReward { ResultType = ResultType.InvalidCoupon };

                if (couponManager.IsInvalidDateTime() )
                    return new ProtoGetCouponReward { ResultType = ResultType.InvalidDateTime };

                bool IsAlreadyRewarded = true;
               
                if (couponManager.GetCouponType() == CouponType.Public)
                    IsAlreadyRewarded = await couponManager.IsAlreadyRewardedByPublicType();
                else if (couponManager.GetCouponType() == CouponType.Private)
                    IsAlreadyRewarded = await couponManager.IsAlreadyRewardedByPrivateType();
                else
                    throw new Exception("Wrong Coupon Type");

                if (IsAlreadyRewarded)
                    return new ProtoGetCouponReward { ResultType = ResultType.AlreadyRewarded };

                using (var userDb = new DBContext(req.UserId))
                {
                    await accountDb.BeginTransactionCallback(async () =>
                    {
                        await userDb.BeginTransactionCallback(async () =>
                        {
                            await couponManager.SetCouponRewarded();
                            itemReward
                            = await ProductRewardManager.GiveProduct(accountDb, userDb, req.UserId, 0, couponManager.GetProductId(), "CouponReward");
                            return true;
                        });
                        return true;
                    });
                }
            }

            return new ProtoGetCouponReward
            {
                ResultType = ResultType.Success,
                ItemResults = itemReward
            };
        }
    }
}
