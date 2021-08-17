using CommonProtocol;
using System.Threading.Tasks;

namespace Common.Entities.InfusionBox
{
    public interface IInfusionBoxInfusion
    {
        Task<ProtoNewInfusionBox> GetNewInfusionBox(bool isWin, int addtionalInfusionEnergy, bool isDoubleEnergy);
        Task SetNewInfusionBox(ProtoNewInfusionBox newInfusionBox);
    }
}
