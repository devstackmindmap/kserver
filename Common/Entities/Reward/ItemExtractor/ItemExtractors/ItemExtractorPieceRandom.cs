using System.Collections.Generic;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Item;
using CommonProtocol;

namespace Common.Entities.ItemExtractor
{
    public class ItemExtractorPieceRandom : ItemExtractorPiece
    {
        public ItemExtractorPieceRandom(uint userId, PieceType pieceType, string tableName, DBContext db) : base(userId, pieceType, tableName, db)
        {
        }

        public override async Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue)
        {
            var piece = PieceFactory.CreatePiece(_pieceType, _userId, 0, _db);
            var havePieces = new PieceDatas(await piece.SelectAll());
            if (havePieces.PieceDataList.Count == 0)
            {
                await UlockExtract(dataItem, alreadyAcheiveItems);
                return;
            }

            var havePieceExcludedAlreadyMax = ExcludeAlreadyMax(havePieces.PieceDataList, out var maxLevelPieceData);
            var havePieceExcludedAlreadyAcheiveItem 
                = ExcludeAlreadyAcheiveItem(havePieceExcludedAlreadyMax, alreadyAcheiveItems);

            PieceData pickPiece;
            if (havePieceExcludedAlreadyAcheiveItem.Count > 0)
                pickPiece = havePieceExcludedAlreadyAcheiveItem[AkaRandom.Random.Next(havePieceExcludedAlreadyAcheiveItem.Count)];
            else
                pickPiece = maxLevelPieceData[AkaRandom.Random.Next(maxLevelPieceData.Count)];

            ExtractPiece(pickPiece, dataItem);
        }

        private List<PieceData> ExcludeAlreadyMax(List<PieceData> pieceDatas, out List<PieceData> maxLevelPieceData)
        {
            maxLevelPieceData = new List<PieceData>();
            for (int i = pieceDatas.Count - 1; i > -1; i--)
            {
                if (GetNeedPieceCountToMaxLevel(pieceDatas[i]) == 0)
                {
                    maxLevelPieceData.Add(pieceDatas[i]);
                    pieceDatas.RemoveAt(i);
                }
            }
            return pieceDatas;
        }

        private List<PieceData> ExcludeAlreadyAcheiveItem(List<PieceData> pieceDatas, List<ProtoItemResult> alreadyAcheiveItems)
        {
            for (int i = pieceDatas.Count - 1 ; i > -1 ; i--)
            {
                foreach (var alreadyAcheiveItem in alreadyAcheiveItems)
                {
                    if (alreadyAcheiveItem.ClassId == pieceDatas[i].Id 
                        && alreadyAcheiveItem.Count != 0)
                    {
                        pieceDatas.RemoveAt(i);
                        break;
                    }
                }
            }
            return pieceDatas;
        }

        private async Task UlockExtract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems)
        {
            var newItemType = GetPieceUnlockRandomByPieceRandom(dataItem.ItemType);
            var extractor = ItemExtractorFactory.CreateItemExtractor(_userId, newItemType, _db);
            await extractor.Extract(dataItem, alreadyAcheiveItems, 0);
            var items = extractor.GetResultItems();

            foreach (var item in items)
            {
                AddReturnItem(item.ItemType, item.ClassId, item.Count);
            }
        }
    }
}
