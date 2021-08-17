using System;
using System.Threading.Tasks;

namespace Common.Entities.Charger
{
    public interface ICharger
    {
        Task Update();
        Task UpdateChargerDataNowDateTime();
        Task UpdateChargerDataNowDateTime(DateTime newDateTime);
    }
}
