using CommonProtocol;
using System.Threading.Tasks;

namespace BattleNotificationTest
{
    public class BeforeBattleStart : BaseController
    {
        public override async Task DoPipeline(BaseProtocol requestInfo, BattleNotificationForm form)
        {
            form.Notice("배틀시작");

            //PlayerInfo.Instance.PlayerType = req.MyPlayerType;
            //PlayerInfo.Instance.PlayerType = req.MyPlayerType == AkaEnum.Battle.PlayerType.Player1 ? AkaEnum.Battle.PlayerType.Player2 : AkaEnum.Battle.PlayerType.Player1;
            
        }
    }
}
