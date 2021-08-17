using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ChattingManager
    {
        DBContext _accountDb;
        StringBuilder _query = new StringBuilder();
        uint _userId;
        uint _target;
        string _chattingKey;
        ChattingType _chattingType;
        string _chattingMessage;

        public ChattingManager(DBContext accountDb, uint userId, uint targetId, string chattingKey, ChattingType chattingType, string chattingMessage)
        {
            _accountDb = accountDb;
            _userId = userId;
            _target = targetId;
            _chattingKey = chattingKey;
            _chattingType = chattingType;
            _chattingMessage = chattingMessage;
        }

        


        public async Task GetChattingMessage()
        {
            _query.Clear();
            _query.Append("SELECT seq, chattingKey, chattingDateTime, chattingType, chattingMessage FROM chatting_messages " +
                "WHERE chattingKey = ").Append(_chattingKey).Append(" AND chattingDateTime >= '")
                .Append(DateTime.UtcNow.AddDays(-5)).Append("' ORDER BY seq DESC  LIMIT 100");
            
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {

                }
            }
        }
        
    }
}
