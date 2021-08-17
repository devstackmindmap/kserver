using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using CommonProtocol;

namespace WebServer.Controller.Emoticon
{
    public class WebSetEmoticon : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoSetEmoticons;

            using (var db = new DBContext(req.UserId))
            {
                var sqlBuilder = new StringBuilder();
                var emoticonItems = req.Emoticons.Select(emoticon => ItemFactory.CreateEmoticon(ItemType.Emoticon, req.UserId, emoticon.EmoticonId, db, emoticon.OrderNum, emoticon.UnitId));
                
                bool result = true;
                foreach (var emoticonItem in emoticonItems)
                    result &= await emoticonItem.SetOrder();

                return new ProtoResult
                {
                    ResultType = result ? ResultType.Success : ResultType.Fail
                };
            }
        }
    }
}
