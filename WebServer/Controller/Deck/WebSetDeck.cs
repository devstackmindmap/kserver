using System.Threading.Tasks;
using AkaDB.MySql;
using CommonProtocol;

namespace WebServer.Controller.Deck
{
    public class WebSetDeck : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoSetDeck;
            using (var db = new DBContext(req.UserId))
            {
                var deck = new WebLogic.Deck.Deck(db, req.UserId);
                return new ProtoResult { ResultType = await deck.SetDeck(req.UpdateDecks) };
            }
        }
    }
}
