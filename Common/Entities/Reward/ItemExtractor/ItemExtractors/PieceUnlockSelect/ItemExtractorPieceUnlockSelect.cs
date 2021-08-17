using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Item;
using CommonProtocol;

namespace Common.Entities.ItemExtractor
{
    public class ItemExtractorPieceUnlockSelect : ItemExtractorPiece
    {
        public ItemExtractorPieceUnlockSelect(uint userId, PieceType pieceType, string tableName, DBContext db) : base(userId, pieceType, tableName, db)
        {
        }

        public override async Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue)
        {
            var piece = PieceFactory.CreatePiece(_pieceType, _userId, dataItem.ClassId, _db);
            using (var cursor = await piece.Select())
            {
                if (cursor.Read() == false)
                {
                    AddReturnItem(GetItemType(), dataItem.ClassId, 0);
                }
            }
        }
    }
}
