using System.Threading.Tasks;
using AkaDB.MySql;
using AkaUtility;
using Common.Entities.Stage;
using CommonProtocol;

namespace WebServer.Controller.Stage
{
    public class WebStageLevelRoomInfo : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var protoStageLevel = requestInfo as ProtoGetStageLevelRoomInfo;
            var protoOnGetStageLevelInfo = new ProtoOnGetStageLevelRoomInfo();

            using (var db = new DBContext(protoStageLevel.UserId))
            {
                var stageInfo = new StageInfo(db, protoStageLevel);
                //firstly check roguelike stage
                protoOnGetStageLevelInfo = await stageInfo.GetRoguelikeStageInfo();

                //진행중인 로그라이크 스테이지 요청 or 진행중인 로그라이크 스테이지 일치 체크 
                if (true == protoStageLevel.StageLevelId.In((uint)0, protoOnGetStageLevelInfo.StageLevelId))
                    return protoOnGetStageLevelInfo;

                //secondary check other pve forced Open stage
                if (protoStageLevel.StageLevelId != 0 && protoOnGetStageLevelInfo.StageLevelId == 0)
                    protoOnGetStageLevelInfo = await stageInfo.GetStageInfo();
                else
                    protoOnGetStageLevelInfo.StageLevelId = 0;

            }
            return protoOnGetStageLevelInfo;
        }
    }
}
