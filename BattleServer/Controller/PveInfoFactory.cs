using AkaEnum;
using AkaEnum.Battle;
using System.Threading.Tasks;

namespace BattleServer.Controller
{
    interface IPveInfoHelper
    {
        Task<ResultType> SetBattleInfo(IBattleInfo battleInfo);
    }

    public class PveInfoFactory
    {
        public static async Task<ResultType> SetBattleInfo(IBattleInfo battleInfo)
        {
            IPveInfoHelper stageInfoHelper = null;
            switch (battleInfo.BattleType)
            {
                case BattleType.LeagueBattleAi:
                    stageInfoHelper = new LeagueInfoHelper();
                    break;
                case BattleType.Tutorial:
                case BattleType.AkasicRecode_RogueLike:
                case BattleType.AkasicRecode_UserDeck:
                    stageInfoHelper = new RoguelikeInfoHelper();
                    break;
                case BattleType.PracticeBattle:
                    stageInfoHelper = new PracticeInfoHelper();
                    break;
                case BattleType.VirtualLeagueBattle:
                    stageInfoHelper = new VirtualInfoHelper();
                    break;
                case BattleType.Challenge:
                    stageInfoHelper = new ChallengeInfoHelper();
                    break;
                case BattleType.EventChallenge:
                    stageInfoHelper = new EventChallengeInfoHelper();
                    break;
            }

            if (stageInfoHelper != null)
                return await stageInfoHelper.SetBattleInfo(battleInfo);
            return ResultType.Fail;
        }
    }
}
