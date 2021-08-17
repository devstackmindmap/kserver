using AkaSerializer;
using CommonProtocol;
using System.Threading.Tasks;

namespace BattleNotificationTest
{
    public class Test : BaseController
    {
        private string _message;
        public Test(string message = "")
        {
            _message = message;
        }

        public override async Task DoPipeline(BaseProtocol requestInfo, BattleNotificationForm form)
        {
            AkaLogger.Logger.Instance().Info(requestInfo.MessageType.ToString());
            form.Notice(requestInfo.MessageType.ToString());
        }
    }
}
