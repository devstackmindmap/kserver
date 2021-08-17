using AkaSerializer;
using CommonProtocol;
using System.Threading.Tasks;

namespace BattleNotificationTest
{
    public class ReEnterRoomSuccess : BaseController
    {
        public override async Task DoPipeline(BaseProtocol requestInfo, BattleNotificationForm form)
        {
            var req = requestInfo as ProtoCurrentBattleStatus;
            form.Notice("ReEnterRoomSuccess:" + req.MessageType.ToString());
        }
    }
}
