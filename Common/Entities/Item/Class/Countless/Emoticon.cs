using AkaData;
using AkaDB.MySql;
using AkaLogger;
using AkaUtility;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class Emoticon : Item, ICountless
    {
        private uint _emoticonId;
        private int _orderNum;
        private uint _unitId;

        public Emoticon(uint userId, uint emoticonId, DBContext db, int orderNum = -1, uint unitId = 0) : base(userId, 0, db)
        {
            _emoticonId = emoticonId;
            _orderNum = orderNum;
            _unitId = unitId;
        }

        public override async Task Get(string logCategory)
        {
            var emoticon = Data.GetEmoticon(_emoticonId);
            if (emoticon != null)
            {
                await Upsert(emoticon.UnitId);
                Log.Item.EmoticonGet(_userId, _emoticonId, logCategory);
            }
        }

        private async Task<int> Upsert(uint unitId)
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO emoticons (`userId`, `id`, `unitId`, `orderNum`) VALUES (")
                .Append(_strUserId).Append(", ")
                .Append(_emoticonId.ToString()).Append(", ")
                .Append(unitId.ToString()).Append(", ")
                .Append(_orderNum.ToString()).Append(") ")
                .Append("ON DUPLICATE KEY UPDATE unitId = ").Append(unitId.ToString())
                .Append(", orderNum = ").Append(_orderNum.ToString()).Append(";");
            return await _db.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task<bool> SetOrder()
        {
            if ((false == IsValidData(_unitId) || false == await IsHave() ))
                return false;

            AkaLogger.Log.User.Emoticon.Log(_userId, _unitId, _emoticonId, _orderNum);
            return 0 < await Upsert(_unitId);
        }

        public async Task<bool> IsHave()
        {
            if (Data.GetEmoticon(_emoticonId).IsFirstEmoticon)
                return true;

            var query = new StringBuilder();
            query.Append("SELECT userId FROM emoticons WHERE userId=").Append(_strUserId)
                .Append(" AND id = ").Append(_emoticonId.ToString()).Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                return cursor.Read();
            }
        }

        public bool IsValidData(uint unitId)
        {
            var emoticon = Data.GetEmoticon(_emoticonId);
            return emoticon != null && true == emoticon.UnitId.In(unitId, (uint)0);
        }

        public bool IsValidData()
        {
            return Data.GetEmoticon(_emoticonId) != null ? true : false;
        }
    }
}
