using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Item;
using CommonProtocol;

namespace Common.Entities.ItemExtractor
{
    public class ItemExtractorSpecialPiece : ItemExtractorPiece
    {
        public ItemExtractorSpecialPiece(uint userId, PieceType pieceType, string tableName, DBContext db) : base(userId, pieceType, tableName, db)
        {
            _pieceType = pieceType;
        }

        public override async Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue = 0)
        {
            if (false == IsSpecialPiece(dataItem.ItemType))
                return;
            
            dataItem.ItemType = GetPieceBySpecialPiece(dataItem.ItemType);

            if (IsInvalidId(dataItem.ItemType, itemValue))
                return;

            dataItem.ClassId = itemValue;

            var piece = PieceFactory.CreatePiece(_pieceType, _userId, dataItem.ClassId, _db);
            using (var unit = await piece.Select())
            {
                if (unit.Read() == false)
                    throw new Exception();

                ExtractPiece(new PieceData
                {
                    Id = dataItem.ClassId,
                    Level = (uint)unit["level"],
                    Count = (int)unit["count"]
                }, dataItem);
            }
        }

        private bool IsSpecialPiece(ItemType itemType)
        {
            return itemType.In(ItemType.UnitSpecialPiece, ItemType.CardSpecialPiece, ItemType.WeaponSpecialPiece);
        }

        private ItemType GetPieceBySpecialPiece(ItemType itemType)
        {
            if (itemType == ItemType.UnitSpecialPiece)
                return ItemType.UnitPiece;
            else if (itemType == ItemType.CardSpecialPiece)
                return ItemType.CardPiece;
            else if (itemType == ItemType.WeaponSpecialPiece)
                return ItemType.WeaponPiece;
            else
                throw new Exception("Invalid ItemType");
        }

        private bool IsInvalidId(ItemType itemType, uint itemValue)
        {
            if (itemType == ItemType.UnitPiece)
                return Data.GetUnit(itemValue) == null;
            else if (itemType == ItemType.CardPiece)
                return Data.GetCard(itemValue) == null;
            else if (itemType == ItemType.WeaponPiece)
                return Data.GetWeapon(itemValue) == null;
            else
                throw new Exception("Invalid ItemType");

        }
    }
}
