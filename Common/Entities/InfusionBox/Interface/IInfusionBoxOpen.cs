using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.InfusionBox
{
    public interface IInfusionBoxOpen
    {
        Task<bool> IsEnoughEnergy();
        Task<InfusionBoxOpenInfo> GetInfusionBoxOpenInfo();
        Task SetInfusionBoxOpenInfo(InfusionBoxOpenInfo infusionBoxOpenInfo);
    }
}
