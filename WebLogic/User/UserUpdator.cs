using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Charger;
using Common.Entities.Season;
using Common.Pass;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebLogic.User
{
    public class UserUpdator
    {
        private DBContext _db;
        private DBContext _accountDb;
        private uint _userId;
        private ProtoOnLogin _protoOnLogin;
        private ServerSeasonInfo _seasonInfo;

        public UserUpdator(uint userId, ProtoOnLogin protoOnLogin, DBContext db, DBContext accountDb, ServerSeasonInfo seasonInfo)
        {
            _db = db;
            _userId = userId;
            _protoOnLogin = protoOnLogin;
            _accountDb = accountDb;
            _seasonInfo = seasonInfo;
        }

        public async Task Run() 
        {
            var purchasedSeasons
                = await (new SeasonPassManager(_userId, _seasonInfo.CurrentSeason, _db))
                .GetBeforeAndCurrentPurchasedSeasonPassList();

            var charger
                = ChargerFactory.CreateCharger(_userId, InfusionBoxType.LeagueBox, _db, _accountDb, _seasonInfo, purchasedSeasons);
            await charger.Update();

            SeasonPassManager seasonPassManager = new SeasonPassManager(_userId, _protoOnLogin.CurrentSeasonPass, _db);
            await seasonPassManager.Update();
        }
    }
}
