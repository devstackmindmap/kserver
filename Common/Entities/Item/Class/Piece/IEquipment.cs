
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public interface IEquipment
    {
        Task<bool> IsAnotherUnitEquiped();
        Task PutOn(uint unitId);
        Task PutOff();
    }
}
