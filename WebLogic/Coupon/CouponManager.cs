using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WebLogic.Coupon
{
    public class CouponManager
    {
        private DBContext _accountDb;
        private uint _userId;
        private string _couponCode;
        private CouponInfo _couponInfo = new CouponInfo();
        private StringBuilder _query = new StringBuilder();

        public CouponManager(DBContext accountDb, uint userId, string couponCode)
        {
            _accountDb = accountDb;
            _userId = userId;
            _couponCode = couponCode;
        }

        public async Task<bool> GetCouponInfo()
        {
            _query.Clear();
            _query.Append("SELECT couponType, validDateTime, productId, eventCode FROM _coupon " +
                "WHERE couponCode = '").Append(_couponCode)
                .Append("';");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return false;

                _couponInfo.CouponType = (CouponType)(int)cursor["couponType"];
                _couponInfo.ValidDateTime = (DateTime)cursor["validDateTime"];
                _couponInfo.ProductId = (uint)cursor["productId"];
                _couponInfo.EventCode = (int)cursor["eventCode"];

                return true;
            }
        }

        public bool IsInvalidDateTime()
        {
            return DateTime.UtcNow >= _couponInfo.ValidDateTime;
        }

        public async Task<bool> IsAlreadyRewardedByPublicType()
        {
            _query.Clear();
            _query.Append("SELECT userId FROM coupons " +
                "WHERE userId = ").Append(_userId)
                .Append(" AND couponCode = '").Append(_couponCode).Append("';");

            return await GetUserCouponInfo();
        }

        public async Task<bool> IsAlreadyRewardedByPrivateType()
        {
            _query.Clear();
            _query.Append("SELECT userId FROM coupons WHERE couponCode = '").Append(_couponCode).Append("';");

            return await GetUserCouponInfo();
        }

        public async Task<bool> GetUserCouponInfo()
        {
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                return cursor.Read();
            }
        }

        public CouponType GetCouponType()
        {
            return _couponInfo.CouponType;
        }

        public uint GetProductId()
        {
            return _couponInfo.ProductId;
        }

        public async Task SetCouponRewarded()
        {
            _query.Clear();
            _query.Append("INSERT INTO coupons (userId, couponCode, getDateTime, eventCode) VALUES (")
                .Append(_userId).Append(", '").Append(_couponCode).Append("', '")
                .Append(DateTime.UtcNow.ToTimeString()).Append("',").Append(_couponInfo.EventCode)
                .Append(");");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }
    }
}
