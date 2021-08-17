using CommonProtocol;
using System.Threading.Tasks;

namespace BattleNotificationTest
{
    public abstract class BaseController
    {
        public abstract Task DoPipeline(BaseProtocol requestInfo, BattleNotificationForm form);
    }
}
