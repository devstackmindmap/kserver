
using System.Data.Common;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public interface IItem
    {
        Task Get(string logCategory);
    }
}
