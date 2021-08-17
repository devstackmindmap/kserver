using System;
using System.Collections.Generic;

namespace Common.Entities.Item
{
    public class PieceDatas : ICloneable
    {
        public List<PieceData> PieceDataList;

        public PieceDatas()
        {
            PieceDataList = new List<PieceData>();
        }

        public PieceDatas(List<PieceData> pieceDatas)
        {
            PieceDataList = pieceDatas;
        }

        public object Clone()
        {
            PieceDatas pieceDatas = new PieceDatas();
            foreach (var pieceData in PieceDataList)
            {
                pieceDatas.PieceDataList.Add(new PieceData
                {
                    Id = pieceData.Id,
                    Level = pieceData.Level,
                    Count = pieceData.Count
                });
            }
            return pieceDatas;
        }
    }

    public class PieceData
    {
        public uint Id;
        public uint Level;
        public int Count;
    }
}