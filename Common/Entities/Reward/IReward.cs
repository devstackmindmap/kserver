using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Box
{
    public interface IReward
    {
        Task<List<ProtoItemResult>> GetReward();
        Task SetReward(List<ProtoItemResult> itemResults, string logCategory);
    }
}
