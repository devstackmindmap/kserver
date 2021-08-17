using System;
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
    public class ItemExtractorCardPieceUnlockRandom : ItemExtractorPieceUnlockRandom
    {
        public ItemExtractorCardPieceUnlockRandom(uint userId, DBContext db) : base(userId, TableName.CARD, db)
        {
        }

        public override async Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue)
        {
            var haveUnitsInfo = await GetHaveUnitsInfo();
            var haveCards = await GetItems();
            AddHaveItemByAlreadyAcheive(haveCards, alreadyAcheiveItems, ItemType.CardPiece);
            List<uint> haveUnitsUnlockTypeNormalCardsCheckedLevel = new List<uint>();
            foreach (var unitInfo in haveUnitsInfo)
            {
                var unitId = unitInfo.Key;
                var unitLevel = unitInfo.Value;
                var unlockTypeNormalCheckedLevelCards 
                    = Data.GetCardUnlockTypeNormal(unitId)
                    ?.Where(data => data.UseLevel <= unitLevel)
                    .Select(data => data.Id);

                if (unlockTypeNormalCheckedLevelCards?.Any() ?? false)
                    haveUnitsUnlockTypeNormalCardsCheckedLevel.AddRange(unlockTypeNormalCheckedLevelCards);
            }

            var notHaveCards = haveUnitsUnlockTypeNormalCardsCheckedLevel.Except(haveCards);

            var candidateCount = notHaveCards.Count();

            if (candidateCount > 0)
            {
                var selectedIndex = AkaRandom.Random.Next(0, candidateCount);
                var newCardId = notHaveCards.ElementAt(selectedIndex);
                AddReturnItem(ItemType.CardPiece, newCardId, 0);
                await AddUserProductByUnlockCard(newCardId);
            }
            else
            {
                await PieceRandomExtract(dataItem, ItemType.CardPieceRandom, ItemType.CardPiece, alreadyAcheiveItems);
            }
        }

        private async Task<Dictionary<uint, uint>> GetHaveUnitsInfo()
        {
            Dictionary<uint, uint> haveUnitsInfo = new Dictionary<uint, uint>();

            var strUserId = _userId.ToString();
            var query = $"SELECT id, level FROM units WHERE userId={strUserId};";
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                while(cursor.Read())
                {
                    var unitId = (uint)cursor["id"];
                    var level = (uint)cursor["level"];
                    haveUnitsInfo.Add(unitId, level);
                }
            }

            return haveUnitsInfo;            
        }
    }
}
