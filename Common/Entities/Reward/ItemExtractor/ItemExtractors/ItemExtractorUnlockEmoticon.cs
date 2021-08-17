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
    public class ItemExtractorUnlockEmoticon : ItemExtractor
    {
        public ItemExtractorUnlockEmoticon(uint userId, DBContext db) : base(userId, TableName.EMOTICON, db)
        {
        }

        public override async Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue)
        {
            var haveEmoticons = await GetItems();
            var allEmoticons = Data.GetAllEmoticons().Where(emoticon => emoticon.IsFirstEmoticon == false).Select(emoticon => emoticon.EmoticonId);
            var notHaveEmoticons = allEmoticons.Except(haveEmoticons);
            var candidateCount = notHaveEmoticons.Count();

            if (candidateCount > 0)
            {
                var selectedIndex = AkaRandom.Random.Next(0, candidateCount);
                AddReturnItem(ItemType.Emoticon, notHaveEmoticons.ElementAt(selectedIndex), 0);
            }
        }
    }
}
