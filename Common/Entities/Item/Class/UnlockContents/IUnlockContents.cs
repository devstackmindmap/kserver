using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public interface IUnlockContents
    {
        Task<bool> IsHave();

        bool IsValidData();
    }
}
