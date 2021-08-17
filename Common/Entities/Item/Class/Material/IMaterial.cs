using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public interface IMaterial
    {
        Task<int> GetRemainCount(int useCount);
        Task<bool> IsEnoughCount();

        Task Use(string logCategory);
    }
}
