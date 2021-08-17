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
    public class ItemExtractorUnitPieceUnlockRandom : ItemExtractorPieceUnlockRandom
    {
        public ItemExtractorUnitPieceUnlockRandom(uint userId, DBContext db) : base(userId, TableName.UNIT, db)
        {
        }

        public override async Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue)
        {
            var haveUnits = await GetItems();
            AddHaveItemByAlreadyAcheive(haveUnits, alreadyAcheiveItems, ItemType.UnitPiece);
            var probabilityTypeUnits = Data.GetUnitIdsByRewardProbabilityType();
            var notHaveUnits = probabilityTypeUnits.Except(haveUnits);
            var candidateCount = notHaveUnits.Count();

            if (candidateCount > 0)
            {
                var selectedIndex = AkaRandom.Random.Next(0, candidateCount);
                var newUnitId = notHaveUnits.ElementAt(selectedIndex);
                AddReturnItem(ItemType.UnitPiece, newUnitId, 0);
                AddLevel1Card(newUnitId);
                AddUserProfileByUnit(newUnitId);
                AddEmoticonByUnit(newUnitId);
            }
            else
            {
                await PieceRandomExtract(dataItem, ItemType.UnitPieceRandom, ItemType.UnitPiece, alreadyAcheiveItems);
            }
        }
    }
}
