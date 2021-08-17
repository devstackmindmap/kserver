using AkaDB.MySql;

namespace Common.Entities.Item
{
    public class Gem : GemCommon
    {
        public Gem(uint userId, DBContext db, int count = 0) : base(userId, ColumnName.USER_GEM, db, count)
        {
        }
    }
}
