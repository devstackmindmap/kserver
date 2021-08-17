using AkaData;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.ItemExtractor
{
    public interface IItemExtractor
    {
        Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue);
        Task Extract(List<DataItem> dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue);
        List<ProtoItemResult> GetResultItems();
    }
}
