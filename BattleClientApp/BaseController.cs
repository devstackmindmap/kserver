using CommonProtocol;
using System.Threading.Tasks;

namespace BattleClientApp
{
    public abstract class BaseController
    {
        public abstract  Task DoPipeline(BaseProtocol requestInfo);
    }
}
