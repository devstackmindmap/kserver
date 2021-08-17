using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Common.Entities.Deck
{
    public class DBGetDeck
    {
        public DBContext Db;
        public uint UserId { get; set; }
        public ModeType ModeType { get; set; }
        public bool IsAllDeckNum { get; set; } = true;
        public byte DeckNum { get; set; }

        public async Task<ProtoOnGetDeck> ExecuteAsync()
        {
            return await GetDeckInfo(Db);
        }

        private async Task<ProtoOnGetDeck> GetDeckInfo(DBContext context)
        {
            using (var cursor = await GetCursorOfDeckInfos(context))
            {
                var proto = new ProtoOnGetDeck();
                SetDeckInfos(cursor, proto);

                return proto;
            }
        }

        private void SetDeckInfos(DbDataReader cursor, ProtoOnGetDeck proto)
        {
            while (cursor.Read())
            {
                proto.DeckElements.Add(new ProtoDeckElement
                {
                    ClassId = (uint)cursor["classId"],
                    DeckNum = (byte)cursor["deckNum"],
                    ModeType = (ModeType)(byte)cursor["modeType"],
                    OrderNum = (byte)cursor["orderNum"],
                    SlotType = (SlotType)(byte)cursor["slotType"]
                });
            }
        }

        private async Task<DbDataReader> GetCursorOfDeckInfos(DBContext db)
        {
            DbDataReader cursor = null;

            var strUserId = UserId.ToString();
            var strModeType = ((byte)ModeType).ToString();

            if (IsAllDeckNum)
            {
                cursor = await db.ExecuteReaderAsync(
                     $"SELECT modeType, slotType, deckNum, orderNum, classId FROM decks " +
                     $"WHERE userId = {strUserId} and modeType = {strModeType} and classId != 0 ORDER BY modeType, slotType, deckNum, orderNum;");
            }
            else
            {
                var strDeckNum = DeckNum.ToString();
                cursor = await db.ExecuteReaderAsync(
                    $"SELECT modeType, slotType, deckNum, orderNum, classId FROM decks " +
                    $"WHERE userId = {strUserId} and modeType = {strModeType} and deckNum = {strDeckNum} and classId != 0 ORDER BY modeType, slotType, deckNum, orderNum;");
            }

            return cursor;
        }
    }
}
