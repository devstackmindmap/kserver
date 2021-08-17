using AkaDB.MySql;
using AkaUtility;
using Common.Entities.Charger;
using CommonProtocol;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.InfusionBox
{
    public abstract  class InfusionBox
    {
        protected uint _userId;
        protected byte _type;
        protected DBContext _db;
        protected ICharger _energy;

        public InfusionBox(uint userId, byte type, ICharger energy, DBContext db)
        {
            _userId = userId;
            _type = type;
            _db = db;
            _energy = energy;
        }

        protected async Task<ProtoInfusionBox> GetInfusionBox()
        {
            var strUserId = _userId.ToString();
            var strType = _type.ToString();

            var query = new StringBuilder();
            query.Append("SELECT id, ").Append(ColumnName.BOX_ENERGY).Append(", ").Append(ColumnName.USER_ENERGY)
                .Append(", ").Append(ColumnName.USER_BONUS_ENERGY).Append(", ").Append(ColumnName.USER_ENERGY_RECENT_UPDATE_DATETIME)
                .Append(" FROM ").Append(TableName.INFUSION_BOX)
                .Append(" WHERE userId = ").Append(strUserId).Append(" AND `type`=").Append(strType).Append(";");

            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                cursor.Read();
                var id = (uint)cursor["id"];

                return new ProtoInfusionBox
                {
                    Id = id,
                    BoxEnergy = (int)cursor[ColumnName.BOX_ENERGY],
                    UserEnergy = (int)cursor[ColumnName.USER_ENERGY],
                    UserBonusEnergy = (int)cursor[ColumnName.USER_BONUS_ENERGY],
                    UserEnergyRecentUpdateDatetime = ((DateTime)cursor[ColumnName.USER_ENERGY_RECENT_UPDATE_DATETIME]).Ticks
                };
            }
        }

        protected async Task SetInfusionBox(ProtoNewInfusionBox userInfusion)
        {
            var strUserId = _userId.ToString();
            var strType = _type.ToString();
            var strId = userInfusion.Id.ToString();

            var query = new StringBuilder();
            query.Append("UPDATE infusion_boxes SET id = ").Append(strId).Append(", userEnergy = userEnergy - ").Append(userInfusion.UseUserEnergy)
                .Append(", userBonusEnergy = userBonusEnergy - ").Append(userInfusion.UseUserBonusEnergy)
                .Append(", boxEnergy = ").Append(userInfusion.NewTotalBoxEnergy)
                .Append(", userEnergyRecentUpdateDatetime = '").Append(new DateTime(userInfusion.NewUserEnergyRecentUpdateDatetime).ToTimeString())
                .Append("' WHERE userId = ").Append(strUserId).Append(" AND type = ").Append(strType).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());
        }
    }
}
