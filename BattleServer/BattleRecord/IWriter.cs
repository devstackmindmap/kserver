using System.IO;
using System.Threading.Tasks;

namespace BattleServer.BattleRecord
{
    public interface IWriter
    {
        Task<bool> WriteAsync(MemoryStream stream, string recordKey);
    }

}