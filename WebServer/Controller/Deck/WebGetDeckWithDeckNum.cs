using System.Collections.Generic;
using System.Threading.Tasks;
using AkaDB.MySql;
using Common.Entities.User;
using CommonProtocol;

namespace WebServer.Controller.Deck
{
    public class WebGetDeckWithDeckNum : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoGetDeckWithDeckNum;

            var res = new ProtoOnGetDeckWithDeckNum
            {
                UserAndDecks = new Dictionary<uint, ProtoDeckWithDeckNum>()
            };

            foreach (var userIdAndDeckNum in req.UserIdAndDeckNums)
            {
                var userId = userIdAndDeckNum.Key;
                var deckNum = userIdAndDeckNum.Value;

                ProtoDeckWithDeckNum deckInfo;
                using (var db = new DBContext(userId))
                {
                    var deck = new WebLogic.Deck.Deck(db, userId);
                    deckInfo = await deck.GetDeckWithDeckNum( deckNum, req.ModeType, req.BattleType) as ProtoDeckWithDeckNum;
                }

                if (deckInfo != null)
                {
                    using (var db = new DBContext("AccountDBSetting"))
                    {
                        var userInfo = await GetNickName(db, userId);
                        deckInfo.Nickname = userInfo.nickName;
                        deckInfo.ProfileIconId = userInfo.profileIconId;
                    }
                    res.UserAndDecks.Add(userId, deckInfo);
                }
            }

            return res;
        }

        private async Task<(string nickName, uint profileIconId)> GetNickName(DBContext db, uint userId)
        {
            var dbNickname = new DBGetUserNickname
            {
                StrUserId = userId.ToString()
            };

            return await dbNickname.ExecuteAsync(db);
        }
    }
}
