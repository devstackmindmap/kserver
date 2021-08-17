using AkaData;
using AkaEnum;
using Common.CommonType;
using Common.Quest;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Challenge
{
    public abstract class CommonChallengeManager
    {
        protected DbConnects _dbCon;
        protected string _tableName;
        protected UserDbChallengeStage _challengeStage;
        protected StringBuilder _query = new StringBuilder();
        protected uint _userId;

        protected abstract Task InitChallengeStage();

        public CommonChallengeManager(string tableName, DbConnects dbCon, uint userId)
        {
            _tableName = tableName;
            _dbCon = dbCon;
            _userId = userId;
        }

        protected async Task<UserDbChallengeStage> GetChallengeStageInfo()
        {
            if (_challengeStage != null)
                return _challengeStage;
            
            using (var cursor = await _dbCon.UserDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                {
                    await InitChallengeStage();
                    _challengeStage = new UserDbChallengeStage();
                }
                else
                {
                    _challengeStage = new UserDbChallengeStage
                    {
                        ClearCount = cursor.GetInt32(0),
                        IsRewarded = cursor.GetInt32(1),
                        RewardResetCount = cursor.GetInt32(2),
                        Round = cursor.GetInt32(3)
                    };
                }
            }

            return _challengeStage;
        }

        public async Task<int> GetTodayKnightLeagueWinCount()
        {
            var questIo = new QuestIO(_userId, _dbCon.UserDb);
            var questList = await questIo.GetQuestWithType(QuestType.ChallengeConditioner);

            foreach (var quest in questList)
            {
                if (Data.GetQuest(quest.QuestGroupId).FirstOrDefault()?.QuestProcessType
                    == QuestProcessType.DailyKnightLeagueVictory)
                    return quest.PerformCount;
            }
            return 0;
        }

        protected async Task<bool> IsCanResetReward(UserDbChallengeStage stageInfo)
        {
            return await GetTodayKnightLeagueWinCount() >=
                (int)Data.GetConstant(DataConstantType.COUNT_NIGHT_LEAGUE_WINS_THAT_CAN_RESET_CHALLENGE_REWARDS).Value
                &&
                stageInfo.RewardResetCount
                < (int)Data.GetConstant(DataConstantType.CHALLENGE_MODE_REWARD_RESET_COUNT).Value;
        }
    }
}
