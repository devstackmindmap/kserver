using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;

namespace Common.Entities.ItemExtractor
{
    public class ItemExtractorWeaponPieceUnlockRandom : ItemExtractorPieceUnlockRandom
    {
        public ItemExtractorWeaponPieceUnlockRandom(uint userId, DBContext db) : base(userId, TableName.WEAPON, db)
        {
        }

        public override async Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue)
        {
            var haveWeapon = await GetItems();
            AddHaveItemByAlreadyAcheive(haveWeapon, alreadyAcheiveItems, ItemType.WeaponPiece);
            var allWeapon = Data.GetAllWeaponIds();
            var notHaveWeapon = allWeapon.Except(haveWeapon);
            var candidateCount = notHaveWeapon.Count();

            if (candidateCount > 0)
            {
                var selectedIndex = AkaRandom.Random.Next(0, candidateCount);
                var newId = notHaveWeapon.ElementAt(selectedIndex);
                AddReturnItem(ItemType.WeaponPiece, newId, 0);
            }
            else
            {
                await PieceRandomExtract(dataItem, ItemType.WeaponPieceRandom, ItemType.WeaponPiece, alreadyAcheiveItems);
            }
        }
    }
}
