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
    public class ItemExtractorPiece : ItemExtractor
    {
        protected PieceType _pieceType;

        public ItemExtractorPiece(uint userId, PieceType pieceType, string tableName, DBContext db) : base(userId, tableName, db)
        {
            _pieceType = pieceType;
        }

        public override async Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue = 0)
        {
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

        protected void ExtractPiece(PieceData pieceData, DataItem dataItem)
        {
            var addCount = AkaRandom.Random.Next(dataItem.MinNumber, dataItem.MaxNumber + 1);

            var needPieceCountToMaxLevel = GetNeedPieceCountToMaxLevel(pieceData);
            var updatePieceCount = needPieceCountToMaxLevel;
            if (needPieceCountToMaxLevel < 0)
                needPieceCountToMaxLevel = 0;

            if (IsNeedToChangePieceToGold(addCount, needPieceCountToMaxLevel))
            {
                //if (IsPieceSelectType(dataItem.ItemType) && updatePieceCount == 0)
                //    return;

                AddReturnItem(GetPieceGoldType(), pieceData.Id, (addCount - needPieceCountToMaxLevel) * GetMaxPieceToGold());
                if (updatePieceCount != 0)
                    AddReturnItem(GetItemType(), pieceData.Id, updatePieceCount);
            }
            else
            {
                AddReturnItem(GetItemType(), pieceData.Id, addCount);
            }
        }

        protected ItemType GetItemType()
        {
            switch (_pieceType)
            {
                case PieceType.Unit:
                    return ItemType.UnitPiece;
                case PieceType.Card:
                    return ItemType.CardPiece;
                case PieceType.Weapon:
                    return ItemType.WeaponPiece;
                default:
                    throw new System.Exception();
            }
        }

        protected int GetNeedPieceCountToMaxLevel(PieceData pickPiece)
        {
            var maxLevel = GetMaxLevel(pickPiece.Id);
            if (maxLevel <= pickPiece.Level)
                return 0;
            
            int needPieceCountToMaxLevel = 0;
            for (uint level = pickPiece.Level; level <= maxLevel; level++)
            {
                needPieceCountToMaxLevel += GetNeedPieceCountToMaxLevel(pickPiece.Id, level);
            }
            return needPieceCountToMaxLevel - pickPiece.Count;
        }

        private uint GetMaxLevel(uint id)
        {
            switch (_pieceType)
            {
                case PieceType.Unit:
                    return Data.GetUnitAdditionalData(id).MaxLevel;
                case PieceType.Card:
                    return Data.GetCardAdditionalData(id).MaxLevel;
                case PieceType.Weapon:
                    return Data.GetWeaponAdditionalData(id).MaxLevel;
                default:
                    throw new Exception();
            }
        }

        private int GetNeedPieceCountToMaxLevel(uint id, uint level)
        {
            switch (_pieceType)
            {
                case PieceType.Unit:
                    return Data.GetUnitStat(id, level).RequirePieceCountForNextLevelUp;
                case PieceType.Card:
                    return Data.GetCardStat(id, level).RequirePieceCountForNextLevelUp;
                case PieceType.Weapon:
                    return Data.GetWeaponStat(id, level).RequirePieceCountForNextLevelUp;
                default:
                    throw new Exception();
            }
        }

        private ItemType GetPieceGoldType()
        {
            switch (_pieceType)
            {
                case PieceType.Unit:
                case PieceType.Card:
                case PieceType.Weapon:
                    return ItemType.Gold;
                default:
                    throw new System.Exception();
            }
        }

        private int GetMaxPieceToGold()
        {
            switch (_pieceType)
            {
                case PieceType.Unit:
                    return (int)Data.GetConstant(DataConstantType.MAX_UNIT_PIECE_TO_GOLD).Value;
                case PieceType.Card:
                    return (int)Data.GetConstant(DataConstantType.MAX_CARD_PIECE_TO_GOLD).Value;
                case PieceType.Weapon:
                    return (int)Data.GetConstant(DataConstantType.MAX_WEAPON_PIECE_TO_GOLD).Value;
                default:
                    throw new System.Exception();
            }
        }

        private bool IsPieceSelectType(ItemType itemType)
        {
            return itemType.In(ItemType.UnitPiece, ItemType.CardPiece, ItemType.WeaponPiece);
        }
    }
}
