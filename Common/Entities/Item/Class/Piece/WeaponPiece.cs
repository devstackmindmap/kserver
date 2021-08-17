using AkaDB.MySql;

namespace Common.Entities.Item
{
    public class WeaponPiece : Equipment
    {
        public WeaponPiece(uint userId, uint pieceId, DBContext db, int count = 0) : base(userId, pieceId, TableName.WEAPON, StoredProcedure.WEAPON_LEVEL_UP, db, count)
        {
        }

        public override int GetRequirePieceCountForLevelUp(uint pieceId, uint level)
        {
            return AkaData.Data.GetRequireWeaponPieceCountForLevelUp(pieceId, level);
        }

        public override int GetRequireGoldForLevelUp(uint pieceId, uint level)
        {
            return AkaData.Data.GetRequireWeaponGoldForLevelUp(pieceId, level);
        }
    }
}
