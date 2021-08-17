using AkaDB.MySql;

namespace Common.Entities.Item
{
    public class GemPaid : GemCommon
    {
        public GemPaid(uint userId, DBContext db, int count = 0) : base(userId, ColumnName.USER_GEM_PAID, db, count)
        {
        }
    }
}
