using AkaEnum;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public interface IPiece
    {
        Task LevelUp(int requirePieceCount, int goldCount);
        Task<ValuesRequireForCalculation> GetValuesRequireForCalculate(uint pieceId);
        int GetRequirePieceCountForLevelUp(uint pieceId, uint level);
        int GetRequireGoldForLevelUp(uint pieceId, uint level);
        Task<DbDataReader> Select();
        Task<List<PieceData>> SelectAll();
        ResultType IsEnableLevelup();

    }
}
