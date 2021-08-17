using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common;
using Common.Entities.Charger;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLogic.User.DataUpdator
{
    public class UnitUpdator : UserInitDataUpdator
    {
        internal UnitUpdator(uint userId, DBContext db, IEnumerable<uint> updateIdList ) : base(userId,db,updateIdList)
        {
        }
        
        public override async Task Run()
        {
            string targetTable;
            ICollection<uint> dataCsv = Data.GetUnitIdsByFirstType();
            string insertColumns;
            string insertValues;
            var defaultUnits = UpdateIdList.Where(dataCsv.Contains);

            var defaultCardIds = Data.GetPrimitiveDict<uint, DataCard>(DataType.data_card).Values.Where(
                data => data.UseLevel == 1
                && data.CardType == CardType.User
                && data.UnlockType == UnlockType.Level
                && defaultUnits.Contains(data.UnitId)
                )
                .Select(card => card.CardId);

            var defaultProfileIds = new List<uint>();
            foreach (var unitId in defaultUnits)
            {
                defaultProfileIds.Add(Data.GetProfileIconIdByUnitUnlock(unitId));
            }

            targetTable = TableName.PROFILE;
            insertColumns = "(userId, id)";
            insertValues = string.Join(",", defaultProfileIds.Select(id => $"({StrUserId}, {id})"));
            await ExecuteInsertQuery(targetTable, insertColumns, insertValues);

            targetTable = TableName.CARD;
            insertColumns = "(userId, level, count, id)";
            insertValues = string.Join(",", defaultCardIds.Select(id => $"({StrUserId}, {InitialValue.START_PIECE_LEVEL}, {InitialValue.START_PIECE_COUNT}, {id.ToString()})"));
            await ExecuteInsertQuery(targetTable, insertColumns, insertValues);

            targetTable = TableName.UNIT;
            insertColumns = "(userId, level, count, id, currentSeasonRankPoint, maxRankLevel, currentRankLevel)";
            insertValues = string.Join(",", defaultUnits.Select(id => $"({StrUserId}, {InitialValue.START_PIECE_LEVEL}, {InitialValue.START_PIECE_COUNT}, {id.ToString()}, {InitialValue.START_UNIT_RANK_POINT}, {InitialValue.START_UNIT_RANK_LEVEL}, {InitialValue.START_UNIT_RANK_LEVEL})"));
            await ExecuteInsertQuery(targetTable, insertColumns, insertValues);
        }

    }
}
