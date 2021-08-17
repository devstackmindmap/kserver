using CommonProtocol;
using System.Threading.Tasks;

namespace BattleNotificationTest
{
    public class BattleResult : BaseController
    {
        public override async Task DoPipeline(BaseProtocol requestInfo, BattleNotificationForm form)
        {
            //var req = requestInfo as ProtoOnBattleResult;
            //form.Notice("배틀결과");
            //foreach(var item in req.UnitsRankData)
            //{
            //    form.Notice($"[{req.BattleResultType.ToString()}" +
            //    $":Unit[{item.Key}] Level:{item.Value.MaxRankLevel}" +
            //    $" Point:{item.Value.CurrentSeasonRankPoint}");
            //}

            //form.Notice($":User Level:{req.UserRankData.MaxRankLevel}" +
            //    $" Point:{req.UserRankData.CurrentSeasonRankPoint}");
        }
    }
}
