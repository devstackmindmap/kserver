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
    public abstract class ItemExtractorPieceUnlockRandom : ItemExtractor
    {
        public ItemExtractorPieceUnlockRandom(uint userId, string tableName, DBContext db) : base(userId, tableName, db)
        {
        }

        protected void AddHaveItemByAlreadyAcheive(List<uint> haveItems, List<ProtoItemResult> alreadyAcheiveItems, ItemType itemType)
        {
            foreach (var item in alreadyAcheiveItems)
            {
                if (item.ItemType == itemType)
                {
                    haveItems.Add(item.ClassId);
                }
            }
        }
    }
}
