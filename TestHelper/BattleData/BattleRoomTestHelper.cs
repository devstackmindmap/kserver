using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using BattleLogic;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestHelper.BattleData
{
    public class BattleRoomTestHelper : IDisposable
    {
        TransactionManager _transactionManager = new TransactionManager();

        public DBContext DB => _transactionManager.DB;

        public async Task SetRoguelikeStageRound(uint userId, uint stageLevelId, int round, BattleType battleType = BattleType.AkasicRecode_RogueLike)
        {
            await _transactionManager.AddUpdate("inprogress_stage",$"userid={userId.ToString()} AND battleType ={(int)battleType} ", ("stageLevelId", stageLevelId.ToString()),("clearRound", round.ToString()));
        }

        public async Task InsertRoguelikeStageRound(uint userId, uint stageLevelId, uint clearRound, BattleType battleType, IList<uint> cardStatIdList, IList<uint> replaceCardStatIdList)
        {
            //   await _transactionManager.Insert("inprogress_stage", )
            var columns = "userId, battleType, stageLevelId, clearRound, cardStatIdList, replaceCardStatIdList";
            var values = $"{userId},{(int)battleType},{stageLevelId},{clearRound}, '{string.Join("/",cardStatIdList)}', '{string.Join("/",replaceCardStatIdList)}'";
            var deleteWhere = $"userId={userId} AND battleType={(int)battleType}";
            await _transactionManager.Insert("inprogress_stage", columns, values, deleteWhere);
        }

        public void Dispose()
        {
            _transactionManager.Dispose();
        }
    }
}
