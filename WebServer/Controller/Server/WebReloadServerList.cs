using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using Common.Entities.SquareObject;
using AkaSerializer;

namespace WebServer.Controller.SquareObject
{
    public class WebReloadServerList : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
             return new ProtoEmpty();
        }

    }
}
