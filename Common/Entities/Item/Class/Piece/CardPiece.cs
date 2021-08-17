using AkaDB.MySql;
using AkaEnum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class CardPiece : Piece
    {
        public CardPiece(uint userId, uint pieceId, DBContext db, int count = 0) : base(userId, count, TableName.CARD, StoredProcedure.CARD_LEVEL_UP, pieceId, db)
        {
        }

        public override int GetRequirePieceCountForLevelUp(uint pieceId, uint level)
        {
            return AkaData.Data.GetRequireCardPieceCountForLevelUp(pieceId, level);
        }

        public override int GetRequireGoldForLevelUp(uint pieceId, uint level)
        {
            return AkaData.Data.GetRequireCardGoldForLevelUp(pieceId, level);
        }

        public override async Task LevelUp(int requirePieceCount, int goldCount)
        {
            await base.LevelUp(requirePieceCount, goldCount);

            new Quest.QuestIO(_userId).UpdateQuest(
                new Dictionary<QuestProcessType, int>() {
                    { QuestProcessType.SkillLevelUp , 1 }
                }, 0 ,null);
        }
    }
}
