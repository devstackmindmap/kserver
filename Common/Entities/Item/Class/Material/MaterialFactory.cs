using AkaDB.MySql;
using AkaEnum;

namespace Common.Entities.Item
{
    public static class MaterialFactory
    {
        public static Material CreateMaterial(MaterialType materialType, uint userId, int cost, DBContext db)
        {
            switch (materialType)
            {
                case MaterialType.Gold:
                    return new Gold(userId, db, cost);
                case MaterialType.Gem:
                    return new Gem(userId, db, cost);
                case MaterialType.GemPaid:
                    return new GemPaid(userId, db, cost);
                case MaterialType.StarCoin:
                    return new StarCoin(userId, db, cost);
                case MaterialType.SquareObjectStartTicket:
                    return new SquareObjectStartTicket(userId, db, cost);
                case MaterialType.ChallengeCoin:
                    return new ChallengeCoin(userId, db, cost);
            }

            return null;
        }

        public static TermMaterial CreateTermMaterial(MaterialType termMaterialType, uint userId, int cost, 
            DBContext accountDb, DBContext userDb)
        {
            switch (termMaterialType)
            {
                case MaterialType.EventCoin:
                    return new EventCoin(userId, accountDb, userDb, cost);
                case MaterialType.EventBoxEnergy:
                    return new EventBoxEnergy(userId, accountDb, userDb, cost);
            }
            return null;
        }
    }
}
