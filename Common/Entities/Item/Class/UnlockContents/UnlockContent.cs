using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class UnlockContent : Item, IUnlockContents
    {
        private uint _contentsType;

        public UnlockContent(uint userId, uint classId, DBContext db) : base(userId, 0, db)
        {
            _contentsType = classId;
        }

        public override async Task Get(string logCategory)
        {
            await Update(logCategory);
        }

        private IContents GetContentUnlocker()
        {
            if (Enum.TryParse<ContentsType>(_contentsType.ToString(), out var contentsType))
            {
                if (contentsType == ContentsType.SquareObject)
                {
                    return new SquareObject.SquareObjectIO(_db, null);
                }
            }
            return null;
        }

        private async Task<int> Update(string logCategory)
        {
            var contentsUnlocker = GetContentUnlocker();
            if (await contentsUnlocker.UnlockContents(_userId) )
            {
                Log.Item.ContentGet(_userId, _contentsType, logCategory);

                var query = new StringBuilder();
                query.Append("UPDATE user_info SET unlockContents = CONCAT(unlockContents,'")
                    .Append(_contentsType)
                    .Append("/') WHERE userId = ")
                    .Append(_strUserId)
                    .Append(";");
                return await _db.ExecuteNonQueryAsync(query.ToString());
            }
            return 0;
        }

        public async Task<bool> IsHave()
        {
            var query = new StringBuilder();
            query.Append("SELECT unlockContents FROM user_info WHERE userId =")
                 .Append(_strUserId)
                 .Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                return cursor.Read() && ((string)cursor["unlockContents"]).IndexOf(_contentsType.ToString()) > 0;
            }
        }

        public bool IsValidData()
        {
            return Enum.TryParse<ContentsType>(_contentsType.ToString(), out var result);
        }
    }
}
