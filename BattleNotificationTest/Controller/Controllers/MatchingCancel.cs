using AkaSerializer;
using CommonProtocol;
using System.Threading.Tasks;

namespace BattleNotificationTest
{
    public class MatchingCancel : BaseController
    {
        public override async Task DoPipeline(BaseProtocol requestInfo, BattleNotificationForm form)
        {
            var req = requestInfo as ProtoEmpty;
            form.Notice("매칭취소결과:" + req.MessageType.ToString());
        }
    }
}
