using AkaData;
using AkaDB.MySql;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLogic.User.DataUpdator
{
    public class SquareObjectFriendUpdator : UserInitDataUpdator
    {
        internal SquareObjectFriendUpdator(uint userId, DBContext db ) : base(userId,db,null)
        {
        }
        
        public override async Task Run()
        {
            var maxReceiveCount = (int)(float.Epsilon + Data.GetConstant(AkaEnum.DataConstantType.SQUARE_OBJECT_MAX_RECEIVE_DONATION).Value);

            var query = new StringBuilder();
            for (int i = 0; i < maxReceiveCount; i++)
                query.Append("INSERT INTO square_object_friends(receivedUserId) VALUES(").Append(StrUserId).Append(");");

            await DB.ExecuteNonQueryAsync(query.ToString());
        }
    }
}
