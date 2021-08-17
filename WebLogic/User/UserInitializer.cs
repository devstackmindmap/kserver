using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLogic.User
{
    public class UserInitializer
    {
        private DBContext _db;
        private string _strUserId;
        private uint _userId;

        public UserInitializer(uint userId, DBContext db)
        {
            _db = db;
            _strUserId = userId.ToString();
            _userId = userId;
        }

        public async Task Run()
        {
            await GiveDefaultUsers();
            await GiveDefaultBox();
            await GiveDefaultDeckSetting();
            await GiveDefaultSquareObject();
            await GiveDefaultUserInfo();
        }

        private async Task GiveDefaultUsers()
        {
            var materialColumnValue = Data.GetAllMaterials()
                                .Select(data => Common.Entities.Item.MaterialFactory.CreateMaterial(data.MaterialType, _userId, data.StartValue, _db)?.GetColumnWithValue())
                                .Where(value => value.HasValue);

            var materualColumns = string.Join(",", materialColumnValue.Select(value => value.Value.columnName));
            var materialValues = string.Join(",", materialColumnValue.Select(value => value.Value.defaultValue.ToString()));
            
            var query = new StringBuilder();
            query.Append("INSERT INTO users (userId, ")
                .Append(materualColumns)
                .Append(", level, exp) VALUES (")
                .Append(_strUserId).Append(", ")
                .Append(materialValues)
                .Append(", 1, 0);");

            await _db.ExecuteNonQueryAsync(query.ToString());
        }

        private async Task GiveDefaultBox()
        {
            var strBoxType = ((byte)InfusionBoxType.LeagueBox).ToString();
            var strFirstLeagueBox = ((int)Data.GetConstant(DataConstantType.FIRST_LEAGUE_BOX).Value).ToString();
            var strStartEnergy = Data.GetMaterial(MaterialType.Energy).StartValue.ToString();
            var strStartBonusEnergy = Data.GetMaterial(MaterialType.BonusEnergy).StartValue.ToString();
            var strNow = DateTime.UtcNow.ToTimeString();

            var query = new StringBuilder();
            query.Append("INSERT INTO ").Append(TableName.INFUSION_BOX).Append(" (userId, type, id, ")
                .Append(ColumnName.BOX_ENERGY).Append(", ")
                .Append(ColumnName.USER_ENERGY).Append(", ")
                .Append(ColumnName.USER_BONUS_ENERGY).Append(", ")
                .Append(ColumnName.USER_ENERGY_RECENT_UPDATE_DATETIME).Append(") VALUES (")
                .Append(_strUserId).Append(", ")
                .Append(strBoxType).Append(", ")
                .Append(strFirstLeagueBox).Append(", 0, ")
                .Append(strStartEnergy).Append(", ")
                .Append(strStartBonusEnergy).Append(", '")
                .Append(strNow).Append("');");

            await _db.ExecuteNonQueryAsync(query.ToString());
        }
        
        private async Task GiveDefaultDeckSetting()
        {
            var defaultDeckOfPvp = Data.GetDefaultDeckSet(ModeType.PVP);
            var defaultDeckOfPve = Data.GetDefaultDeckSet(ModeType.PVE);

            ValidateUnitId(defaultDeckOfPvp.UnitIdList);
            ValidateUnitId(defaultDeckOfPve.UnitIdList);

            ValidateCardId(defaultDeckOfPvp.CardIdList);
            ValidateCardId(defaultDeckOfPve.CardIdList);

            await InsertDeckInfo(ModeType.PVP, 0, SlotType.Unit, defaultDeckOfPvp.UnitIdList);
            await InsertDeckInfo(ModeType.PVP, 0, SlotType.Card, defaultDeckOfPvp.CardIdList);

            await InsertDeckInfo(ModeType.PVE, 0, SlotType.Unit, defaultDeckOfPve.UnitIdList);
            await InsertDeckInfo(ModeType.PVE, 0, SlotType.Card, defaultDeckOfPve.CardIdList);
        }

        private async Task InsertDeckInfo(ModeType modeType, byte deckNum, SlotType slotType, List<uint> classIdList)
        {
            var strModeType = ((int)modeType).ToString();
            var strDeckNum = deckNum.ToString();
            var strSlotType = ((int)slotType).ToString();
            for (int orderNum = 0; orderNum < classIdList.Count; orderNum++)
            {
                var strClassId = classIdList[orderNum].ToString();
                var strOrderNum = orderNum.ToString();
                var query = new StringBuilder();
                query.Append("INSERT INTO decks(userId, modeType, deckNum, slotType, orderNum, classId) VALUES(")
                    .Append(_strUserId).Append(", ")
                    .Append(strModeType).Append(", ")
                    .Append(strDeckNum).Append(", ")
                    .Append(strSlotType).Append(", ")
                    .Append(strOrderNum).Append(",")
                    .Append(strClassId).Append(");");
                await _db.ExecuteNonQueryAsync(query.ToString());
            }
        }

        private void ValidateUnitId(List<uint> unitIdList)
        {
            foreach (var unitId in unitIdList)
            {
                Data.GetUnit(unitId);
            }
        }

        private void ValidateCardId(List<uint> cardIdList)
        {
            foreach (var cardId in cardIdList)
            {
                Data.GetCard(cardId);
            }
        }

        private async Task GiveDefaultSquareObject()
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO square_object_schedule (userId) VALUES (").Append(_strUserId).Append(");");
            await _db.ExecuteNonQueryAsync(query.ToString());
        }

        private async Task GiveDefaultUserInfo()
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO user_info (userId, lastRefreshDate) VALUES (").Append(_strUserId)
                .Append(", '").Append(DateTime.UtcNow.ToTimeString())
                .Append("') ON DUPLICATE KEY UPDATE lastRefreshDate = '").Append(DateTime.UtcNow.ToTimeString())
                .Append("';");
            await _db.ExecuteNonQueryAsync(query.ToString());
        }


    }
}
