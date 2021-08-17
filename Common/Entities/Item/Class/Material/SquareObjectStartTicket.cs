using AkaDB.MySql;
using AkaEnum;

namespace Common.Entities.Item
{
    public class SquareObjectStartTicket : Material
    {
        public SquareObjectStartTicket(uint userId, DBContext db, int count = 0) 
            : base(userId, count, ColumnName.SQUARE_OBJECT_START_TICKET, MaterialType.SquareObjectStartTicket, db)
        {
        }
    }
}
