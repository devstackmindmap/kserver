using AkaDB.MySql;
using Common.Entities.Challenge;
using Common.Entities.Season;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Challenge
{
    public class WebGetChallengeFirstClearUser : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoChallengeParam;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var manager 
                    = ChallengeFactory.CreateChallengeManager
                    (accountDb, null, 0, req.Season, req.Day, req.DifficultLevel);

                var userId = await manager.GetFirstClearUserId();
                if (false == userId.HasValue)
                    return null;

                using (var userDb = new DBContext(userId.Value))
                {
                    var deckElements = await manager.GetFirstClearUser(userDb, userId.Value);
                    var deck = new WebLogic.Deck.Deck(userDb, userId.Value);
                    return new ProtoOnGetDeckWithNickname
                    {
                        Nickname = await GetNickname(accountDb, userId.Value),
                        DeckElements = await deck.GetRecentDeck(deckElements)
                    };
                }
            }
        }

        public async Task<string> GetNickname(DBContext accountDb, uint userId)
        {
            using (var cursor = 
                await accountDb.ExecuteReaderAsync("SELECT nickName FROM accounts WHERE userId = " + userId + ";"))
            {
                if (false == cursor.Read())
                    return "";
                return (string)cursor["nickName"];
            }
        }
    }
}
