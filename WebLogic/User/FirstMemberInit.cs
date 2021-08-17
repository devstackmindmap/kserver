using AkaDB.MySql;
using System.Threading.Tasks;

namespace WebLogic.User
{
    public class FirstMemberInitializer
    {
        private DBContext _db;
        private uint _userId;
        private int _cheatLevel;

        public FirstMemberInitializer(DBContext db, uint userId, int cheatLevel)
        {
            _db = db;
            _userId = userId;
            _cheatLevel = cheatLevel;
        }

        public async Task FirstMemberInit()
        {
            if (_cheatLevel == 0)
                await new UserInitializer(_userId, _db).Run();
            else
                await new UserCheatInitializer(_userId, _db, _cheatLevel).Run();
        }
    }
}
