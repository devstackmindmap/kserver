using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using Common.Entities.SquareObject;
using AkaSerializer;
using System;

namespace WebServer.Controller.SquareObject
{
    public class WebSyncTime : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            return new ProtoOnSyncTime { ServerTime = DateTime.UtcNow.Ticks };
        }

    }
}
