using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaSerializer;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class SquareObjectAgencyPiece : Piece
    {
        public SquareObjectAgencyPiece(uint userId, DBContext db, int count = 0) : base(userId, count, "DontImplemnted", StoredProcedure.SQUAREOBJECT_AGENCY_LEVEL_UP, 0, db)
        {
        }

        public override async Task<DbDataReader> Select()
        {
            var query = new StringBuilder();
            query.Append("SELECT isActivated, nextInvasionTime, agencyLevel as `level` , agencyExp as `count` FROM square_object_schedule  WHERE `userId`= ")
                 .Append(_strUserId).Append(";");
            return await _db.ExecuteReaderAsync(query.ToString());
        }

        public override int GetRequirePieceCountForLevelUp(uint pieceId, uint level)
        {
            return AkaData.Data.GetSquareObjectPlanetAgency(level).NeedExpForNextLevelUp;
        }

        public override int GetRequireGoldForLevelUp(uint pieceId, uint level)
        {
            return AkaData.Data.GetSquareObjectPlanetAgency(level).NeedGoldForNextLevelUp;
        }

        public override async Task LevelUp(int requirePieceCount, int goldCount)
        {
            await base.LevelUp(requirePieceCount, goldCount);

            var protoSquareObjectLevelup = new ProtoSquareObjectLevelUp
            {
                UserId = _userId,
                UsedExp = requirePieceCount
            };
        }

        protected override ResultType IsEnableLevelup(DbDataReader cursor)
        {
            var invasionTime = (DateTime)cursor["nextInvasionTime"];
            var isActivated = 0 != (uint)cursor["isActivated"];

            if (isActivated && DateTime.UtcNow >= invasionTime.AddSeconds(-invasionTime.Second)) 
            {
                return ResultType.Invading;
            }
            return ResultType.Success;
        }
    }
}
