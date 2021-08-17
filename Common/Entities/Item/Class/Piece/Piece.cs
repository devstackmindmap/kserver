using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public abstract class Piece : Item, IPiece
    {
        protected string _tableName;
        protected uint _pieceId;
        protected string _levelUpStoredProcedureName;
        protected string _strPieceId;

        protected ResultType _isEnableLevelup;

        public Piece(uint userId, int count, string tableName, string levelUpStoredProcedureName, uint pieceId, DBContext db) : base(userId, count, db)
        {
            _tableName = tableName;
            _pieceId = pieceId;
            _levelUpStoredProcedureName = levelUpStoredProcedureName;
            _strPieceId = _pieceId.ToString();
        }

        public abstract int GetRequirePieceCountForLevelUp(uint pieceId, uint level);
        public abstract int GetRequireGoldForLevelUp(uint pieceId, uint level);

        public override async Task Get(string logCategory)
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO ").Append(_tableName).Append(" (`userId`, `id`, `level`, `count`) VALUES (")
                .Append(_strUserId).Append(", ").Append(_strPieceId).Append(", ").Append(InitialValue.START_PIECE_LEVEL).Append(", ")
                .Append(_strCount)
                .Append(") ON DUPLICATE KEY UPDATE count = count + ").Append(_strCount).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());

            Log.Item.PieceGet(_strUserId, _tableName, _strPieceId, _strCount, logCategory);
        }

        public virtual async Task<DbDataReader> Select()
        {
            var query = new StringBuilder();
            query.Append("SELECT `level`, `count` FROM ").Append(_tableName)
                .Append(" WHERE `userId`=").Append(_strUserId).Append(" AND `id`=").Append(_strPieceId).Append(";");
            return await _db.ExecuteReaderAsync(query.ToString());
        }

        public async Task<List<PieceData>> SelectAll()
        {
            var pieceDatas = new List<PieceData>();
            var query = new StringBuilder();
            query.Append("SELECT `id`, `level`, `count` FROM ").Append(_tableName)
                .Append(" WHERE `userId`=").Append(_strUserId).Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    pieceDatas.Add(new PieceData
                    {
                        Id = (uint)cursor["id"],
                        Level = (uint)cursor["level"],
                        Count = (int)cursor["count"]
                    });
                }
            }

            return pieceDatas;
        }

        public virtual async Task LevelUp(int requirePieceCount, int goldCount)
        {
            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$userId", _userId),
                new InputArg("$pieceId", _pieceId),
                new InputArg("$goldCount", goldCount),
                new InputArg("$pieceCount", requirePieceCount));
            await _db.CallStoredProcedureAsync(_levelUpStoredProcedureName, paramInfo);
        }

        public async Task<ValuesRequireForCalculation> GetValuesRequireForCalculate(uint pieceId)
        {
            using (var pieceCursor = await Select())
            {
                if (!pieceCursor.Read())
                    throw new Exception("DB Select Exception, Invalid id");

                _isEnableLevelup = IsEnableLevelup(pieceCursor);

                return new ValuesRequireForCalculation
                {
                    NowPieceCount = (int)pieceCursor["count"],
                    RequireGold = GetRequireGoldForLevelUp(pieceId, (uint)pieceCursor["level"]),
                    RequirePieceCountForNextLevelUp = GetRequirePieceCountForLevelUp(pieceId, (uint)pieceCursor["level"]),
                    NewLevel = (uint)pieceCursor["level"] + 1
                };
            }
        }

        public ResultType IsEnableLevelup()
        {
            return _isEnableLevelup;
        }

        protected virtual ResultType IsEnableLevelup(DbDataReader cursor)
        {
            return ResultType.Success;
        }
    }
}
