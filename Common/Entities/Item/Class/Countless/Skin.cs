using AkaData;
using AkaDB.MySql;
using AkaLogger;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class Skin : Item, ICountless
    {
        private uint _skinId;
        private string _strSkinId;

        public Skin(uint userId, uint skinId, DBContext db) : base(userId, 0, db)
        {
            _skinId = skinId;
            _strSkinId = _skinId.ToString();
        }

        public override async Task Get(string logCategory)
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO skins (`userId`, `skinId`) VALUES (")
                .Append(_strUserId).Append(", ").Append(_strSkinId).Append(")" +
                " ON DUPLICATE KEY UPDATE skinId = ").Append(_strSkinId).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());

            Log.Item.EmoticonGet(_userId, _skinId, logCategory);
        }

        public async Task PutOn(uint unitId)
        {
            var strUnitId = unitId.ToString();
            var query = new StringBuilder();
            query.Append("UPDATE units SET skinId = ").Append(_strSkinId)
                .Append(" WHERE userId=").Append(_strUserId).Append(" AND id=").Append(strUnitId).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task<bool> IsHave()
        {
            var query = new StringBuilder();
            query.Append("SELECT userId FROM skins WHERE userId=").Append(_strUserId)
                .Append(" AND skinId=").Append(_strSkinId).Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                return cursor.Read();
            }
        }

        public bool IsValidData(uint unitId)
        {
            var skins = Data.GetDataSkinGroup(Data.GetUnit(unitId).SkinGroupId).SkinIdList;
            return skins.Contains(_skinId);
        }

        public bool IsValidData()
        {
            var skisn = Data.GetDataSkin(_skinId);
            return skisn != null ? true : false;
        }
    }
}
