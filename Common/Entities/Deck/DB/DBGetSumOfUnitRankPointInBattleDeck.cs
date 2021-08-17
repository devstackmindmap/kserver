using AkaDB;
using AkaDB.MySql;
using AkaEnum;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Common.Entities.Deck
{
    public class DBGetSumOfUnitRankPointInBattleDeck
    {
        public DBContext Db;
        public ModeType ModeType { get; set; }
        public uint UserId { get; set; }
        public byte DeckNum { get; set; }

        public async Task<int> ExecuteAsync()
        {
            int sumOfRankPoint = 0;
           
            var maxDeckUnitCount = (int)UnitOrderMinMax.Max + 1;
            var deckUnits = new uint[maxDeckUnitCount];

            int index = 0;

            var strUserId = UserId.ToString();
            var strModeType = ((byte)ModeType).ToString();
            var strSlotType = ((byte)SlotType.Unit).ToString();
            var strDeckNum = DeckNum.ToString();

            using (var cursor = await Db.ExecuteReaderAsync(
                $"SELECT classId FROM decks " +
                $"WHERE userId = {strUserId} and modeType = {strModeType} and slotType = {strSlotType} and deckNum = {strDeckNum}"))
            {
                while (cursor.Read())
                {
                    var classId = (uint)cursor["classId"];
                    deckUnits[index++] = classId;
                }

                Debug.Assert(index == maxDeckUnitCount);
            }

            foreach (var unitId in deckUnits)
            {
                var strUnitId = unitId.ToString();
                using (var cursor = await Db.ExecuteReaderAsync(
                    $"SELECT currentSeasonRankPoint FROM units WHERE userId = {strUserId} and id = {strUnitId}"))
                {
                    cursor.Read();
                    sumOfRankPoint += (int)cursor["currentSeasonRankPoint"];
                }
            }
            return sumOfRankPoint;
        }
    }
}
