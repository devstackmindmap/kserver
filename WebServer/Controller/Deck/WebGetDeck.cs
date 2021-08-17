using AkaDB.MySql;
using Common.Entities.Deck;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Deck
{
    public class WebGetDeck : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var protoGetDeck = requestInfo as ProtoGetDeck;

            using (var db = new DBContext(protoGetDeck.UserId))
            {
                var getDeck = new DBGetDeck
                {
                    Db = db,
                    UserId = protoGetDeck.UserId,
                    ModeType = protoGetDeck.ModeType
                };

                var result = await getDeck.ExecuteAsync();
                return result;
            }
        }
    }
}
