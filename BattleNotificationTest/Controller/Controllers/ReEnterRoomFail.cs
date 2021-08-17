using AkaSerializer;
using CommonProtocol;
using System.Threading.Tasks;

namespace BattleNotificationTest
{
    public class ReEnterRoomFail : BaseController
    {
        public override async Task DoPipeline(BaseProtocol requestInfo, BattleNotificationForm form)
        {
            var req = requestInfo as ProtoEmpty;
            form.Notice("ReEnterRoomFail:" + req.MessageType.ToString());
        }
    }
}
