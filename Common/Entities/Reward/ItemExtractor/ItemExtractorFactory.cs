using AkaDB.MySql;
using AkaEnum;
using System;
using System.Collections.Generic;

namespace Common.Entities.ItemExtractor
{
    public static class ItemExtractorFactory
    {
        public static IItemExtractor CreateItemExtractor(uint userId, ItemType type, DBContext db)
        {
            switch (type)
            {
                case ItemType.UnitPieceUnlockRandom:
                    return new ItemExtractorUnitPieceUnlockRandom(userId, db);
                case ItemType.UnitPieceRandom:
                    return new ItemExtractorPieceRandom(userId, PieceType.Unit, TableName.UNIT, db);
                case ItemType.UnitPieceUnlockSelect:
                    return new ItemExtractorUnitPieceUnlockSelect(userId, PieceType.Unit, TableName.UNIT, db);
                case ItemType.CardPieceUnlockSelect:
                    return new ItemExtractorCardPieceUnlockSelect(userId, PieceType.Card, TableName.CARD, db);
                case ItemType.UnitPiece:
                    return new ItemExtractorPiece(userId, PieceType.Unit, TableName.UNIT, db);
                case ItemType.UnitSpecialPiece:
                    return new ItemExtractorSpecialPiece(userId, PieceType.Unit, TableName.UNIT, db);

                case ItemType.CardPieceUnlockRandom:
                    return new ItemExtractorCardPieceUnlockRandom(userId, db);
                case ItemType.CardPieceRandom:
                    return new ItemExtractorPieceRandom(userId, PieceType.Card, TableName.CARD, db);
                case ItemType.CardPiece:
                    return new ItemExtractorPiece(userId, PieceType.Card, TableName.CARD, db);
                case ItemType.CardSpecialPiece:
                    return new ItemExtractorSpecialPiece(userId, PieceType.Card, TableName.CARD, db);

                case ItemType.WeaponPieceRandom:
                    return new ItemExtractorPieceRandom(userId, PieceType.Weapon, TableName.WEAPON, db);
                case ItemType.WeaponPieceUnlockSelect:
                    return new ItemExtractorPieceUnlockSelect(userId, PieceType.Weapon, TableName.WEAPON, db);
                case ItemType.WeaponPiece:
                    return new ItemExtractorPiece(userId, PieceType.Weapon, TableName.WEAPON, db);
                case ItemType.WeaponPieceUnlockRandom:
                    return new ItemExtractorWeaponPieceUnlockRandom(userId, db);
                case ItemType.WeaponSpecialPiece:
                    return new ItemExtractorSpecialPiece(userId, PieceType.Weapon, TableName.WEAPON, db);

                case ItemType.EmoticonUnlockRandom:
                    return new ItemExtractorUnlockEmoticon(userId, db);

                default:
                    throw new Exception("CreateItemExtractor, Non existent type");
            }
        }

        public static IItemExtractor CreateSelectableExtractor(uint userId, ItemType itemType,  DBContext db)
        {
            switch (itemType)
            {
                case ItemType.DynamicQuest:
                    return new ItemExtractorDynamicQuest(userId, db);
                default:
                    throw new Exception("Non existent type");
            }
        }


        public static bool IsPieceType(ItemType type)
        {
            switch (type)
            {
                case ItemType.UnitPieceUnlockRandom:
                case ItemType.UnitPieceUnlockSelect:
                case ItemType.UnitPieceRandom:
                case ItemType.UnitPiece:
                case ItemType.UnitSpecialPiece:
                case ItemType.CardPieceUnlockRandom:
                case ItemType.CardPieceUnlockSelect:
                case ItemType.CardPieceRandom:
                case ItemType.CardPiece:
                case ItemType.CardSpecialPiece:
                case ItemType.WeaponPieceUnlockRandom:
                case ItemType.WeaponPieceUnlockSelect:
                case ItemType.WeaponPieceRandom:
                case ItemType.WeaponPiece:
                case ItemType.WeaponSpecialPiece:
                case ItemType.EmoticonUnlockRandom:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSelectableType(ItemType type)
        {
            switch (type)
            {
                case ItemType.DynamicQuest:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsCountlessType(ItemType type)
        {
            switch (type)
            {
                case ItemType.Emoticon:
                case ItemType.Skin:
                case ItemType.UserProfile:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsUnlockContentsType(ItemType type)
        {
            switch (type)
            {
                case ItemType.UnlockContents:
                case ItemType.UnlockStageLevel:
                case ItemType.UnlockQuest:
                case ItemType.SeasonPass:
                default:
                    return false;
            }
        }
    }
}
