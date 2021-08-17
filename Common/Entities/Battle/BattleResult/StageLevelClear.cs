using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using AkaEnum.Battle;
using Common.Entities.Stage;
using AkaUtility;

namespace Common.Entities.Battle
{
    public class StageLevelClear : BattleResult
    {
        private DBContext _db;
        private uint _userId;
        private string _strUserId;
        private uint _stageLevelId;
        private BattleType _battleType;
        private ProtoOnBattleResultRoguelike _protoOnBattleResult;

        private StageInfo _stageInfo;

        public StageLevelClear(DBContext db, uint userId, uint stageLevelId, ProtoOnBattleResultRoguelike protoOnBattleResult, BattleType battleType = BattleType.None)
        {
            _db = db;
            _userId = userId;
            _stageLevelId = stageLevelId;
            _battleType = battleType;
            _strUserId = userId.ToString();
            _protoOnBattleResult = protoOnBattleResult;
            _stageInfo = new StageInfo(db, userId, stageLevelId, battleType);
        }

        private async Task<uint> ExecuteAsync()
        {
            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(new InputArg("$userId", _strUserId)
                , new InputArg("$stageLevelId", _stageLevelId.ToString()));

            var cursor = await _db.CallStoredProcedureAsync(StoredProcedure.UPDATE_STAGECLEAR_COUNT, paramInfo);

            uint clearCount = 0;
            while (cursor.Read())
            {
                clearCount = (uint)cursor["clearCount"];
                break;
            }

            return clearCount;
        }

        protected override async Task Win()
        {
            ProtoOnStageLevelExit protoOnStageLevelExit = new ProtoOnStageLevelExit();
            protoOnStageLevelExit.ClearCount = await ExecuteAsync();

            await _stageInfo.ResetInprogressStage();

            if (protoOnStageLevelExit.ClearCount > 0)
            {
                protoOnStageLevelExit.StageLevelId = _stageLevelId;
                protoOnStageLevelExit.IsWin = true;

                protoOnStageLevelExit.OpenedStageLevelIdList = _stageInfo.OpenedStageLevelList();

                //최초 획득
                if (protoOnStageLevelExit.ClearCount == 1)
                {
                    if (null == _protoOnBattleResult.ItemResults)
                        _protoOnBattleResult.ItemResults = new Dictionary<RewardCategoryType, List<ProtoItemResult>>(RewardCategoryTypeComparer.Comparer);

                    _protoOnBattleResult.ItemResults.Add(RewardCategoryType.StageClear,
                        await _stageInfo.GetFirstRewardsAndOpenStage());
                }
            }
            else
            {
                protoOnStageLevelExit.OpenedStageLevelIdList = new List<uint>();
            }

            _protoOnBattleResult.BattleResultType = BattleResultType.Win;
            _protoOnBattleResult.StageClearInfo = protoOnStageLevelExit;



            new Quest.QuestIO(_userId).UpdateQuest(
                new Dictionary<QuestProcessType, int>() {
                    { QuestProcessType.StageClear , 1 }
                }
                , _stageLevelId, null);


        }

        protected override async Task Lose()
        {
            await _stageInfo.ResetInprogressStage();
            ProtoOnStageLevelExit protoOnStageLevelExit = new ProtoOnStageLevelExit();
            protoOnStageLevelExit.StageLevelId = _stageLevelId;
            protoOnStageLevelExit.IsWin = false;
            protoOnStageLevelExit.OpenedStageLevelIdList = new List<uint>();

            _protoOnBattleResult.BattleResultType = BattleResultType.Lose;
            _protoOnBattleResult.StageClearInfo = protoOnStageLevelExit;
        }

        protected override async Task Draw()
        {
            await Lose();
            _protoOnBattleResult.BattleResultType = BattleResultType.Draw;
        }


        public override bool HasRedisJob()
        {
            return false;
        }


        public override async Task<bool> RedisJob(uint serverCurrentSeason, uint clanId, string clanCountryCode)
        {
            //Nothing
            return true;
        }

        public override async Task<uint> SeasonJob()
        {
            return 0;
        }

        public override string GetCountryCode()
        {
            return "";
        }

        public override async Task<ProtoNewInfusionBox> InfusionBoxJob(BattleResultType battleResultType)
        {
            return null;
        }
    }
}
