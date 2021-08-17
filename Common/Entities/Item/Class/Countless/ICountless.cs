using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public interface ICountless
    {
        Task<bool> IsHave();

        bool IsValidData();
    }
}
