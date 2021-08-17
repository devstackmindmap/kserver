using AkaDB.MySql;
using AkaUtility;
using System.Text;
using System.Threading.Tasks;

namespace CouponCodeMaker
{
    public class CouponCodeMake
    {
        private DBContext _accountDb;
        private int _couponType;
        private string _validDateTime;
        private uint _productId;
        private int _count;
        private int _eventCode;

        public CouponCodeMake(DBContext accountDb, int couponType, string validDateTime, uint productId, int count, int eventCode)
        {
            _accountDb = accountDb;
            _couponType = couponType;
            _validDateTime = validDateTime;
            _productId = productId;
            _count = count;
            _eventCode = eventCode;
        }

        public async Task Run()
        {
            if (_couponType == 1)
                _count = 1;

            var query = new StringBuilder();
            query.Append("INSERT INTO _coupon (couponCode, couponType, validDateTime, productId, eventCode) VALUES ");
            for (int i = 0; i < _count; i++)
            {
                if (i != 0)
                    query.Append(",");
                    
                query.Append($"('{Utility.RandomString()}', {_couponType}, '{_validDateTime}', {_productId}, {_eventCode})");
            }
            query.Append(";");

            await _accountDb.ExecuteNonQueryAsync(query.ToString());
        }
    }
}
