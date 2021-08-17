using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer
{
    public abstract class BaseController
    {
        public abstract Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo) ;
    }
}
