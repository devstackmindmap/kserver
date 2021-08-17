using CommonProtocol;
using System.Threading.Tasks;

namespace BattleClientApp
{
    public class TryMatching : BaseController
    {
        public override async Task DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoOnTryMatching;
        }
    }
}
