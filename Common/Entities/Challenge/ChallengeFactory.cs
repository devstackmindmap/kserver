using AkaDB.MySql;
using Common.CommonType;

namespace Common.Entities.Challenge
{
    public class ChallengeFactory
    {
        public static ChallengeManager CreateChallengeManager(
            DBContext accountDb, DBContext userDb, uint userId, uint season, int day, int difficultLevel)
        {
            return new ChallengeManager(new DbConnects
            {
                AccountDb = accountDb,
                UserDb = userDb
            }, new ChallengeParam
            {
                Season = season,
                Day = day,
                DifficultLevel = difficultLevel
            }, userId);
        }

        public static EventChallengeManager CreateEventChallengeManager(
            DBContext accountDb, DBContext userDb, uint userId, uint challengeEventId, int difficultLevel)
        {
            return new EventChallengeManager(new DbConnects
            {
                AccountDb = accountDb,
                UserDb = userDb
            }, new EventChallengeParam
            {
                ChallengeEventId = challengeEventId,
                DifficultLevel = difficultLevel
            }, userId);
        }
    }
}
