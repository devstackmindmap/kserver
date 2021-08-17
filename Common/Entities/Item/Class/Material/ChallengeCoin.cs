using AkaDB.MySql;
using AkaEnum;

namespace Common.Entities.Item
{
    public class ChallengeCoin : Material
    {
        public ChallengeCoin(uint userId, DBContext db, int count = 0) 
            : base(userId, count, ColumnName.CHALLENGE_COIN, MaterialType.ChallengeCoin, db)
        {
        }
    }
}
