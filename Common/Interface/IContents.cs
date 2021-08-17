using System.Threading.Tasks;

namespace Common
{
    public interface IContents
    {
        Task<bool> UnlockContents(uint userId);
    }
}
