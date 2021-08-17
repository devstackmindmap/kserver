using System.Collections.Generic;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using CommonProtocol;

namespace Common.Entities.ItemExtractor
{
    public class ItemExtractorUnitPieceUnlockSelect : ItemExtractorPiece
    {
        public ItemExtractorUnitPieceUnlockSelect(uint userId, PieceType pieceType, string tableName, DBContext db) : base(userId, pieceType, tableName, db)
        {
        }

        public override async Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue)
        {
            if (IsAlreadyExtracted(alreadyAcheiveItems, dataItem.ClassId) || await IsHave(dataItem.ClassId))
            {
                await PieceRandomExtract(dataItem, ItemType.UnitPieceRandom, ItemType.UnitPiece, alreadyAcheiveItems);
            }
            else
            {
                var piece = PieceFactory.CreatePiece(_pieceType, _userId, dataItem.ClassId, _db);
                using (var cursor = await piece.Select())
                {
                    if (cursor.Read() == false)
                    {
                        AddReturnItem(GetItemType(), dataItem.ClassId, 0);
                        AddLevel1Card(dataItem.ClassId);
                        AddUserProfileByUnit(dataItem.ClassId);
                        AddEmoticonByUnit(dataItem.ClassId);
                    }
                }
            }
        }

        private bool IsAlreadyExtracted(List<ProtoItemResult> alreadyAcheiveItems, uint id)
        {
            foreach (var item in alreadyAcheiveItems)
            {
                if (item.ItemType == ItemType.UnitPiece && item.ClassId == id)
                    return true;
            }
            return false;
        }
    }
}
