using AkaDB.MySql;
using Common.Entities.Challenge;
using Common.Entities.Season;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Challenge
{
    public class WebGetEventChallengeFirstClearUser : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoEventChallengeParam;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var manager
                    = ChallengeFactory.CreateEventChallengeManager
                    (accountDb, null, 0, req.ChallengeEventId, req.DifficultLevel);

                var userId = await manager.GetFirstClearUserId();

                using (var userDb = new DBContext(userId.Value))
                {
                    var deckElements = await manager.GetFirstClearUser(userDb, userId.Value);
                    var deck = new WebLogic.Deck.Deck(userDb, req.UserId);
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
