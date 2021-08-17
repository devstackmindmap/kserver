using AkaDB.MySql;
using AkaEnum;

namespace Common.Entities.Item
{
    public class StarCoin : Material
    {
        public StarCoin(uint userId, DBContext db, int count = 0) : base(userId, count, ColumnName.STAR_COIN, MaterialType.StarCoin, db)
        {
        }
    }
}
