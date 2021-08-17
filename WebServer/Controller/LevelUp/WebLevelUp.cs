using AkaDB.MySql;
using Common.Entities.Item;
using CommonProtocol;
using System.Threading.Tasks;
using AkaEnum;
using System.Linq;
using AkaData;
using Common;
using System.Collections.Generic;
using System.Text;
using AkaLogger;

namespace WebServer.Controller.LevelUp
{
    public class WebLevelUp : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoLevelUp;
            List<uint> cardIds = new List<uint>();
            using (var db = new DBContext(req.UserId))
            {
                IPiece piece = PieceFactory.CreatePiece(req.PieceType, req.UserId, req.ClassId, db);

                ValuesRequireForCalculation forCalc = await piece.GetValuesRequireForCalculate(req.ClassId);
                var isEnableLevelup = piece.IsEnableLevelup();

                if (isEnableLevelup != ResultType.Success)
                    return new ProtoOnLevelUp { ResultType = isEnableLevelup };

                if (forCalc.RequirePieceCountForNextLevelUp == 0)
                    return new ProtoOnLevelUp { ResultType = ResultType.AlreadyMaxLevel };

                if (forCalc.NowPieceCount < forCalc.RequirePieceCountForNextLevelUp)
                    return new ProtoOnLevelUp { ResultType = ResultType.NeedPieceCount };

                var gold = new Gold(req.UserId, db, forCalc.RequireGold);
                if (!await gold.IsEnoughCount())
                    return new ProtoOnLevelUp { ResultType = ResultType.NeedGold };

                await db.BeginTransactionCallback(async () =>
                {
                    await piece.LevelUp(forCalc.RequirePieceCountForNextLevelUp, forCalc.RequireGold);

                    if (req.PieceType == PieceType.Unit)
                    {
                        cardIds = await GiveCardByUnitLevelUp(db, req.UserId, req.ClassId, forCalc.NewLevel);
                    }
                    return true;
                });

                Log.Item.PieceUse(req.UserId, req.PieceType, req.ClassId, forCalc.RequirePieceCountForNextLevelUp, "PieceLevelUp");
                Log.Item.MaterialUse(req.UserId.ToString(), MaterialType.Gold, forCalc.RequireGold, gold.NowCount - forCalc.RequireGold, "PieceLevelUp");

                Log.User.PieceLevel.Log(req.UserId, (byte)req.PieceType, req.ClassId, forCalc.NewLevel, string.Join(",", cardIds),
                    gold.NowCount, forCalc.RequireGold, forCalc.NowPieceCount, forCalc.RequirePieceCountForNextLevelUp);

                return new ProtoOnLevelUp { ResultType = ResultType.Success, Gold = await gold.GetRemainCount(), NewCardIds = cardIds };
            }
        }

        private async Task<List<uint>> GiveCardByUnitLevelUp(DBContext db, uint userId, uint unitId, uint level)
        {
            var cardsTable = Data.GetPrimitiveDict<uint, DataCard>(DataType.data_card);
            var selectedCard = cardsTable.Values.Where((data) =>
            {
                if (data.CardType == CardType.User && data.UnitId == unitId && data.UseLevel == level)
                {
                    return true;
                }

                return false;
            });

            List<uint> cardIds = new List<uint>();
            var strUserId = userId.ToString();
            foreach (var cardInfo in selectedCard)
            {
                cardIds.Add(cardInfo.CardId);
                var strCardId = cardInfo.CardId.ToString();
                var query = new StringBuilder();
                query.Append("INSERT INTO cards (userId, level, count, id) VALUES (")
                    .Append(strUserId).Append(", ")
                    .Append(InitialValue.START_PIECE_LEVEL).Append(", ")
                    .Append(InitialValue.START_PIECE_COUNT).Append(", ")
                    .Append(strCardId).Append(");");

                await db.ExecuteNonQueryAsync(query.ToString());
                query.Clear();
            }

            return cardIds;
        }
    }
}
