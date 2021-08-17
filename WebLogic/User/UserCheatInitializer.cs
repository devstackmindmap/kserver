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
    public class UserCheatInitializer
    {
        private DBContext _db;
        private string _strUserId;
        private int _cheatLevel;
        private uint _userId;

        public UserCheatInitializer(uint userId, DBContext db, int cheatLevel)
        {
            _db = db;
            _strUserId = userId.ToString();
            _userId = userId;
            _cheatLevel = cheatLevel;
        }

        public async Task Run()
        {
            await GiveDefaultUsers();
            await GiveDefaultBox();
            var selectedUnitIds = await GiveDefaultUnits();
            await GiveDefaultCards(selectedUnitIds);
            await GiveDefaultDeckSetting();
            await GiveDefaultWeapons();
            //await GiveDefaultArmors();
            //await GiveDefaultRank();
            await GiveDefaultSquareObject();
        }

        private async Task GiveDefaultUsers()
        {
            var startValue = 50000;

            var materialColumnValue = Data.GetAllMaterials()
                                .Select(data => Common.Entities.Item.MaterialFactory.CreateMaterial(data.MaterialType, _userId, startValue, _db)?.GetColumnWithValue())
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

        private async Task GiveDefaultWeapons()
        {
            var dataWeapons = Data.GetPrimitiveValues<uint, DataWeapon>(DataType.data_weapon);


            foreach (var dataWeapon in dataWeapons)
            {
                var pieceLevel = _cheatLevel == 1 ? InitialValue.START_WEAPON_LEVEL : Data.GetWeaponAdditionalData(dataWeapon.WeaponId).MaxLevel.ToString();

                var query = new StringBuilder();
                var strWeaponId = dataWeapon.WeaponId;
                query.Append("INSERT INTO weapons(userId, id, level, count) VALUES(")
                    .Append(_strUserId).Append(", ")
                    .Append(strWeaponId).Append(", ")
                    .Append(pieceLevel).Append(", ")
                    .Append("100000").Append(");");

                await _db.ExecuteNonQueryAsync(query.ToString());
                query.Clear();
            }
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

        private async Task<List<uint>> GiveDefaultUnits()
        {
            var unitTable = Data.GetPrimitiveDict<uint, DataUnit>(DataType.data_unit);
            var selectedUnits = unitTable.Values.Where(data => data.UserType == UserType.User );
            List<uint> unitIds = new List<uint>();

            List<string> addUnits = new List<string>();
            List<string> skins = new List<string>();

            foreach (var unitInfo in selectedUnits)
            {
                var pieceLevel = InitialValue.START_PIECE_LEVEL;
                if (_cheatLevel > 1)
                {
                    if (_cheatLevel > Data.GetUnitAdditionalData(unitInfo.UnitId).MaxLevel)
                    {
                        pieceLevel = Data.GetUnitAdditionalData(unitInfo.UnitId).MaxLevel.ToString();
                    }
                    else
                        pieceLevel = _cheatLevel.ToString();
                }
                unitIds.Add(unitInfo.UnitId);
                var strUnitId = unitInfo.UnitId;

                var skinList = Data.GetDataSkinGroup(unitInfo.SkinGroupId).SkinIdList;
                var skinId = AkaRandom.Random.ChooseElementRandomlyInCount(skinList);
                skins.AddRange(skinList.Select(skin => _strUserId + "," + skin));

                var addunit = string.Join(",",_strUserId, pieceLevel, "100000", strUnitId.ToString(),
                            InitialValue.START_UNIT_RANK_POINT, InitialValue.START_UNIT_RANK_LEVEL, InitialValue.START_UNIT_RANK_LEVEL,
                            skinId);
                addUnits.Add(addunit);
            }

            var query = "INSERT INTO skins (userId, skinId) VALUES ("
                        + string.Join("),(", skins)
                        + ");";
            await _db.ExecuteNonQueryAsync(query);
            query = "INSERT INTO units (userId, level, count, id, currentSeasonRankPoint, maxRankLevel, currentRankLevel, skinId) VALUES ("
                    + string.Join("),(", addUnits)
                    + ");";
            await _db.ExecuteNonQueryAsync(query);

            return unitIds;
        }

        private async Task GiveDefaultCards(List<uint> unitIds)
        {
            var cardsTable = Data.GetPrimitiveDict<uint, DataCard>(DataType.data_card);
            var selectedCard = cardsTable.Values.Where((data) =>
            {
                if (data.CardType == CardType.User 
            //    && data.UseLevel == 1 
                && unitIds.Contains(data.UnitId)
                && data.UnlockType == UnlockType.Level)
                {
                    return true;
                }

                return false;
            });

            foreach (var cardInfo in selectedCard)
            {
                var pieceLevel = InitialValue.START_PIECE_LEVEL;
                if (_cheatLevel > 1)
                {
                    if (_cheatLevel > Data.GetCardAdditionalData(cardInfo.CardId).MaxLevel)
                    {
                        pieceLevel = Data.GetCardAdditionalData(cardInfo.CardId).MaxLevel.ToString();
                    }
                    else
                        pieceLevel = _cheatLevel.ToString();
                }

                var strCardId = cardInfo.CardId;
                var query = new StringBuilder();
                query.Append("INSERT INTO cards (userId, level, count, id) VALUES (")
                    .Append(_strUserId).Append(", ")
                    .Append(pieceLevel).Append(", ")
                    .Append("100000").Append(", ")
                    .Append(strCardId).Append(");");

                await _db.ExecuteNonQueryAsync(query.ToString());
                query.Clear();
            }
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

        private async Task GiveDefaultRank()
        {
            var strRankType = ((int)RankType.AllUnitRankPoint).ToString();
            var query = new StringBuilder();
            query.Append("INSERT INTO ranks (userId, rankType, rankLevel, rankPoint) VALUES (")
                .Append(_strUserId).Append(" , ")
                .Append(strRankType).Append(", 1, 0);");
            await _db.ExecuteNonQueryAsync(query.ToString());
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


    }
}
