using AkaDB.MySql;
using System.Data.Common;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public abstract class Item : IItem
    {
        protected uint _userId;
        protected int _count;
        protected DBContext _db;
        protected string _strUserId;
        protected string _strCount;

        public abstract Task Get(string logCategory);

        public Item(uint userId, int count, DBContext db)
        {
            _userId = userId;
            _count = count;
            _db = db;
            _strUserId = _userId.ToString();
            _strCount = _count.ToString();
        }
    }
}
