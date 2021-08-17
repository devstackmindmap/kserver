using AkaDB.MySql;
using AkaEnum;
using System;

namespace Common.Entities.Item
{
    public static class ItemFactory
    {
        public static IItem CreateItem(ItemType itemType, uint userId, uint classId, DBContext db, int count = 0)
        {
            switch (itemType)
            {
                case ItemType.UnitPiece:
                case ItemType.UnitPieceUnlockSelect:
                    return new UnitPiece(userId, classId, db, count);
                case ItemType.CardPiece:
                case ItemType.CardPieceUnlockSelect:
                    return new CardPiece(userId, classId, db, count);
                case ItemType.WeaponPiece:
                case ItemType.WeaponPieceUnlockSelect:
                    return new WeaponPiece(userId, classId, db, count);
                case ItemType.Gold:
                    return new Gold(userId, db, count);
                case ItemType.Gem:
                    return new Gem(userId, db, count);
                case ItemType.GemPaid:
                    return new GemPaid(userId, db, count);
                case ItemType.StarCoin:
                    return new StarCoin(userId, db, count);
                case ItemType.SquareObjectStartTicket:
                    return new SquareObjectStartTicket(userId, db, count);
                case ItemType.Skin:
                    return new Skin(userId, classId, db);
                case ItemType.Emoticon:
                    return new Emoticon(userId, classId, db);
                case ItemType.UserProfile:
                    return new UserProfile(userId, classId, db);
                case ItemType.UnlockContents:
                    return new UnlockContent(userId, classId, db);
                case ItemType.UnlockStageLevel:
                    return new UnlockStageLevel(userId, classId, db);
                case ItemType.UnlockQuest:
                    return new UnlockQuest(userId, classId, db);
                case ItemType.BonusEnergy:
                    return new BonusEnergy(userId, count, db);
                case ItemType.Energy:
                    return new Energy(userId, count, db);
                case ItemType.UserProduct:
                    return new UserProduct(userId, classId, count, db);
                case ItemType.Mail:
                    return new ItemMail(userId, classId, db);
                case ItemType.SeasonPass:
                    return new UnlockSeasonPass(userId, classId, db);
                case ItemType.DynamicQuest:
                    return new ItemDynamicQuest(userId, classId, count, db);
                default:
                    throw new Exception("Non existent type");
            }
        }

        public static ICountless CreateCountless(ItemType itemType, uint userId, uint classId, DBContext db)
        {
            switch (itemType)
            {
                case ItemType.Skin:
                    return new Skin(userId, classId, db);
                case ItemType.Emoticon:
                    return new Emoticon(userId, classId, db);
                case ItemType.UserProfile:
                    return new UserProfile(userId, classId, db);
                default:
                    throw new Exception("Non existent type");
            }
        }

        public static IUnlockContents CreateUnlockContents(ItemType itemType, uint userId, uint contentType, DBContext db)
        {
            switch (itemType)
            {
                case ItemType.UnlockContents:
                    return new UnlockContent(userId, contentType, db);
                case ItemType.UnlockStageLevel:
                    return new UnlockStageLevel(userId, contentType, db);
                case ItemType.UnlockQuest:
                    return new UnlockQuest(userId, contentType, db);
                case ItemType.SeasonPass:
                    return new UnlockSeasonPass(userId, contentType, db);
                default:
                    throw new Exception("Non existent type");
            }
        }

        public static Skin CreateSkin(ItemType itemType, uint userId, uint classId, DBContext db)
        {
            switch (itemType)
            {
                case ItemType.Skin:
                    return new Skin(userId, classId, db);
                default:
                    throw new Exception("Non existent type");
            }
        }

        public static Emoticon CreateEmoticon(ItemType itemType, uint userId, uint classId, DBContext db, int orderNum = -1, uint unitId = 0)
        {
            switch (itemType)
            {
                case ItemType.Emoticon:
                    return new Emoticon(userId, classId, db, orderNum, unitId);
                default:
                    throw new Exception("Non existent type");
            }
        }

        public static ItemMail CreateItemMail(uint userId, uint mailId, DBContext db)
        {
            return new ItemMail(userId, mailId, db);
        }
    }
}
