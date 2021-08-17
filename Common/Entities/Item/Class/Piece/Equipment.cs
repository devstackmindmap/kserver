using AkaDB.MySql;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public abstract class Equipment : Piece
    {
        public Equipment(uint userId, uint pieceId, string tableName, string levelUpStoredProcedureName, DBContext db, int count = 0) : base(userId, count, tableName, levelUpStoredProcedureName, pieceId, db)
        {
        }
    }
}
