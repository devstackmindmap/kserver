using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using CommonProtocol;

namespace WebServer.Controller.Stage
{
    public class WebSetStageLevelRoomInfo : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var protoStageLevel = requestInfo as ProtoSetStageLevelRoomInfo;
            if (protoStageLevel.ProposalTreasureIdList == null)
                protoStageLevel.ProposalTreasureIdList = new List<uint>();
            if (protoStageLevel.TreasureIdList == null)
                protoStageLevel.TreasureIdList = new List<uint>();


            if (protoStageLevel.StageLevelId == 0)
            {
                var emptyList = new List<uint>();
                protoStageLevel.ClearRound = 0;
                protoStageLevel.CardStatIdList = emptyList;
                protoStageLevel.ReplaceCardStatIdList = emptyList;
                protoStageLevel.ProposalTreasureIdList = emptyList;
                protoStageLevel.TreasureIdList = emptyList;
            }
            else if (false == CheckValication(protoStageLevel))
                return new ProtoOnSetStageLevelRoomInfo { Result = false };

            var userId = protoStageLevel.UserId.ToString();
            var battleType = ((int)protoStageLevel.BattleType).ToString();
            var stageLevelId = protoStageLevel.StageLevelId.ToString();
            var clearRound = protoStageLevel.ClearRound.ToString();
            var updateCardStatIdList = string.Join("/", protoStageLevel.CardStatIdList);
            var updateReplaceCardStatIdList = string.Join("/", protoStageLevel.ReplaceCardStatIdList);
            var updateTreasureIdList = string.Join("/", protoStageLevel.TreasureIdList);
            var updateProposalTreasureIdList = string.Join("/", protoStageLevel.ProposalTreasureIdList);

            using (var db = new DBContext(protoStageLevel.UserId))
            {
                var sql = "INSERT INTO inprogress_stage (userId, battleType, stageLevelId, clearRound, cardStatIdList, replaceCardStatIdList, treasureIdList, proposalTreasureIdList ) "
                         + $" VALUES({userId}, {battleType}, {stageLevelId}, {clearRound}, '{updateCardStatIdList}', '{updateReplaceCardStatIdList}', '{updateTreasureIdList}', '{updateProposalTreasureIdList}' ) ON DUPLICATE KEY UPDATE "
                         + $" stageLevelId = {stageLevelId}, clearRound = {clearRound}, cardStatIdList = '{updateCardStatIdList}', replaceCardStatIdList = '{updateReplaceCardStatIdList}', treasureIdList = '{updateTreasureIdList}', proposalTreasureIdList = '{updateProposalTreasureIdList}' ;";

                await db.ExecuteNonQueryAsync(sql);
            }
            return new ProtoOnSetStageLevelRoomInfo { Result = true };
        }
        
        private bool CheckValication(ProtoSetStageLevelRoomInfo protoStageLevel)
        {
            //데이터 검증
            if ((protoStageLevel.CardStatIdList?.Count ?? 0) == 0 )
                return false;

            //컨텐츠 검증 
            var deckModeType = Data.GetContentsConstant(protoStageLevel.BattleType).DeckModeType;
            if (deckModeType != AkaEnum.ModeType.SaveDeck)
                return false;

            //레벨 검증
            var stageLevel = Data.GetStageLevel(protoStageLevel.StageLevelId);
            if ((stageLevel?.RoguelikeSaveDeckId ?? 0) == 0 )
                return false;

            //라운드 검증
            var stageRoundCount = Data.GetStageRoundList(stageLevel.StageLevelId)?.Count ?? 0;
            if (protoStageLevel.ClearRound >= stageRoundCount)
                return false;

            //덱 검증
            var roguelikeInfo = Data.GetRoguelikeSaveDeck(stageLevel.RoguelikeSaveDeckId);
            var cardStatIdList = roguelikeInfo.ProposalCardStatList.Take((int)protoStageLevel.ClearRound + 1)
                                                                   .SelectMany(proposalCardList => proposalCardList)
                                                                   .Union(roguelikeInfo.CardStatIdList);
            if (false == protoStageLevel.CardStatIdList.All(cardStatIdList.Contains))
                return false;

            var nextCardList = roguelikeInfo.ProposalCardStatList.ElementAtOrDefault((int)protoStageLevel.ClearRound);
            if ( (nextCardList?.Any() ?? false) == false )
            {
                if (true == (protoStageLevel.ReplaceCardStatIdList?.Any() ?? false))
                    return false;
            }
            else if (protoStageLevel.ReplaceCardStatIdList == null || protoStageLevel.ReplaceCardStatIdList.Any() == false || false == protoStageLevel.ReplaceCardStatIdList.All(nextCardList.Contains))
            {
                return false;
            }


            //유물 검증
            if (roguelikeInfo.ProposalTreasureId != 0)
            {
                var proposalTreasure = Data.GetProposalTreasure(roguelikeInfo.ProposalTreasureId);
                var treasureIdList = proposalTreasure?.TreasureIdList.Take((int)protoStageLevel.ClearRound + 1)
                                                                     .SelectMany(treasureList => treasureList);
                if (false == protoStageLevel.TreasureIdList.All(treasureIdList.Contains))
                    return false;
            }            

            return true;
        }
    }
   
}
