
using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AkaEnum.Battle;
using System.Text;

namespace Common.Entities.Stage
{
    public class StageInfo : IContents
    {
        private DBContext _db;
        private uint _userId;
        private uint _stageLevelId;
        private BattleType _battleType;


        public StageInfo(DBContext db, uint userId, uint stageLevelId = 0, BattleType battleType = BattleType.None)
        {
            _db = db;
            _userId = userId;
            _stageLevelId = stageLevelId;
            _battleType = battleType;
        }

        public StageInfo(DBContext db, ProtoGetStageLevelRoomInfo protoStageLevelInfo)
            :this(db, protoStageLevelInfo.UserId, protoStageLevelInfo.StageLevelId, protoStageLevelInfo.BattleType) 
        {
        }

        public async Task<Dictionary<BattleType, ProtoOnGetStageLevelRoomInfo>> GetInProgressStageList()
        {
            var inprogressStageList = new Dictionary<BattleType, ProtoOnGetStageLevelRoomInfo>();
            using (var cursor = await _db.ExecuteReaderAsync(
                $"SELECT battleType, stageLevelId, clearRound, cardStatIdList, replaceCardStatIdList, treasureIdList, proposalTreasureIdList  FROM inprogress_stage WHERE userId = ({_userId.ToString()})"))
            {

                while (cursor.Read())
                {
                    var battleType = (BattleType)(int)cursor["battleType"];
                    var protoStageLevelRoomInfo = ReadCursor(cursor);
                    inprogressStageList.Add(battleType, protoStageLevelRoomInfo);
                }
            }
            return inprogressStageList;
        }

        public async Task<ProtoOnGetStageLevelRoomInfo> GetRoguelikeStageInfo()
        {
            var protoOnGetStageLevelInfo = new ProtoOnGetStageLevelRoomInfo();
            var sql = $"SELECT stageLevelId, clearRound , cardStatIdList, replaceCardStatIdList, treasureIdList, proposalTreasureIdList FROM inprogress_stage WHERE userId = {_userId.ToString()} AND battleType = {((int)_battleType).ToString()};";

            using (var cursor = await _db.ExecuteReaderAsync(sql))
            {
                while (cursor.Read())
                {
                    protoOnGetStageLevelInfo = ReadCursor(cursor);
                    break;
                }
            }

            return protoOnGetStageLevelInfo;
        }

        private ProtoOnGetStageLevelRoomInfo ReadCursor(System.Data.Common.DbDataReader cursor)
        {
            var cardStatIdList = (string)cursor["cardStatIdList"];
            var replaceCardStatIdList = (string)cursor["replaceCardStatIdList"];
            var treasureIdList = (string)cursor["treasureIdList"];
            var proposalTreasureIdList = (string)cursor["proposalTreasureIdList"];

            return new ProtoOnGetStageLevelRoomInfo
            {
                StageLevelId = (uint)cursor["stageLevelId"],
                ClearRound = (uint)cursor["clearRound"],
                CardStatIdList = cardStatIdList.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries).Select(uint.Parse).ToList(),
                ReplaceCardStatIdList = replaceCardStatIdList.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries).Select(uint.Parse).ToList(),
                TreasureIdList = treasureIdList.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries).Select(uint.Parse).ToList(),
                ProposalTreasureIdList = proposalTreasureIdList.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries).Select(uint.Parse).ToList()
            };
        }

        public async Task<ProtoOnGetStageLevelRoomInfo> GetStageInfo()
        {
            var stageLevel = AkaData.Data.GetStageLevel(_stageLevelId);
            var protoOnGetStageLevelInfo = new ProtoOnGetStageLevelRoomInfo();

            if (stageLevel != null)
            {
                bool result = stageLevel.IsAutoOpen;
                if (false == result)
                {
                    //thirdly check other pve stage
                    var sql = $"SELECT COUNT(seq) AS result FROM stage_levels WHERE userId = {_userId.ToString()} and stageLevelId = {_stageLevelId.ToString()};";

                    using (var cursor = await _db.ExecuteReaderAsync(sql))
                    {
                        while (cursor.Read())
                        {
                            result = 1 == (int)(long)cursor["result"];
                            break;
                        }
                    }
                }

                if (result)
                {
                    protoOnGetStageLevelInfo.StageLevelId = _stageLevelId;
                    protoOnGetStageLevelInfo.ClearRound = 0;
                }
            }
            return protoOnGetStageLevelInfo;
        }

        public async Task ResetInprogressStage()
        {
            if (BattleType.None != _battleType)
            {
                var query = new StringBuilder();
                query.Append("UPDATE inprogress_stage SET stageLevelId = 0, clearRound = 0, cardStatIdList = '', replaceCardStatIdList = '', treasureIdList = '', proposalTreasureIdList = '' WHERE userId =")
                    .Append(_userId.ToString()).Append(" AND battleType=").Append(((int)_battleType).ToString()).Append(";");
                await _db.ExecuteNonQueryAsync(query.ToString());
            }
        }

        public List<uint> OpenedStageLevelList()
        {
            if (AkaData.Data.IsExistNextStageLevel(_stageLevelId))
            {
                return AkaData.Data.GetOpenStageIdList(_stageLevelId).ToList();
            }
            return new List<uint>();
        }



        public async Task<List<ProtoItemResult>> GetFirstRewardsAndOpenStage()
        {
            var rewardId = AkaData.Data.GetStageLevel(_stageLevelId).RewardId;
            var rewardResult = await Reward.Reward.GetRewards(_db, _userId, rewardId, "StageClear");
            await UnlockContents(_userId);
            return rewardResult;
        }
        

        public async Task<bool> UnlockContents(uint userId)
        {
            var newStageLevelList = OpenedStageLevelList();
            if (newStageLevelList.Any())
            {
                var insertStageList = string.Join(",", newStageLevelList.Select(stageLevelId => $"({userId.ToString()},{stageLevelId.ToString()},0)"));
                var query = new StringBuilder();
                query.Append("INSERT IGNORE INTO `stage_levels` (`userId`, `stageLevelId`, `clearCount`) VALUES ").Append(insertStageList).Append(";");
                await _db.ExecuteNonQueryAsync(query.ToString());
            }
            return false;
        }
    }
}
