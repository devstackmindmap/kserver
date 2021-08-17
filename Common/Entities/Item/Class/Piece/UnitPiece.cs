using AkaDB.MySql;
using AkaEnum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class UnitPiece : Piece
    {
        public UnitPiece(uint userId, uint pieceId, DBContext db, int count = 0) : base(userId, count, TableName.UNIT, StoredProcedure.UNIT_LEVEL_UP, pieceId, db)
        {
        }

        public override int GetRequirePieceCountForLevelUp(uint pieceId, uint level)
        {
            return AkaData.Data.GetRequireUnitPieceCountForLevelUp(pieceId, level);
        }

        public override int GetRequireGoldForLevelUp(uint pieceId, uint level)
        {
            return AkaData.Data.GetRequireUnitGoldForLevelUp(pieceId, level);
        }

        public override async Task LevelUp(int requirePieceCount, int goldCount)
        {
            await base.LevelUp(requirePieceCount, goldCount);

            new Quest.QuestIO(_userId).UpdateQuest(
                new Dictionary<QuestProcessType, int>() {
                    { QuestProcessType.UnitLevelUp , 1 }
                }, 0 , null);
        }
    }
}
