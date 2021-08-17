using System;
using System.IO;
using System.Threading.Tasks;
using CommonProtocol;

namespace WebServer.Controller.Battle
{
    public class WebGetBattleRecord : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoGetBattleRecord;

            throw new NotImplementedException();

            var dirPath = "";
            var recordPath = Path.Combine(dirPath, req.RecordKey);
            if (File.Exists(recordPath))
            {
                using (var ms = new MemoryStream())
                {
                    using (var fs = File.OpenRead(recordPath))
                    {
                        fs.CopyTo(ms);
                    }

                    return new ProtoOnGetBattleRecord
                    {
                        ArchivedData = ms.ToArray()
                    };                
                }
            }

            return new ProtoOnGetBattleRecord();
        }
    }
}
