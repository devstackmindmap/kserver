using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using BattleLogic;
using Common.Entities.Item;
using Common.Entities.Season;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebLogic.User;

namespace TestHelper
{
    public class UserController : IDisposable
    {
        private uint _player1UserId = 0;
        private uint _player2UserId = 0;
        private PlayerInfo _playerInfo;

        public uint Player1UserId => _player1UserId;
        public uint Player2UserId => _player2UserId;
        public PlayerInfo PlayerInfo => _playerInfo;

        public UserInventoryChecker UserInventoryChecker;

        public ProtoOnLogin LoginResult { get; private set; }

        private readonly StackExchange.Redis.IDatabase _redis = AkaRedis.AkaRedis.GetDatabase();


        public UserController()
        {
        }

        public async Task MakeTwoUserData()
        {
            _player1UserId = await Login();
            _player2UserId = await Login();
            SetPlayerInfo();

            UserInventoryChecker = new UserInventoryChecker(_player1UserId);
        }

        public async Task MakeOneUserData()
        {
            _player1UserId = await Login();
            UserInventoryChecker = new UserInventoryChecker(_player1UserId);

        }

        private void SetPlayerInfo()
        {
            _playerInfo = new PlayerInfo();
            _playerInfo.BattleType = BattleType.LeagueBattle;
            _playerInfo.DeckModeType = AkaEnum.ModeType.PVP;
            _playerInfo.Player1UserId = _player1UserId;
            _playerInfo.Player2UserId = _player2UserId;
            _playerInfo.Player1Ready = true;
            _playerInfo.Player2Ready = true;
            _playerInfo.Player1TreasureIdList = new List<uint>();
            _playerInfo.Player2TreasureIdList = new List<uint>();
        }

        private async Task<uint> Login()
        {
            AccountInfo accountInfo = null;

            var protoOnLogin = new ProtoOnLogin();
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                ServerSeason seasonManager = new ServerSeason(accountDb);
                var seasonInfo = await seasonManager.GetKnightLeagueSeasonInfo();

                var nick = MakeRandomNickname();
                var account = new Account(accountDb, nick, nick);
                accountInfo = await account.GetAccountInfo(seasonInfo.CurrentSeason, "KR");

                using (var db = new DBContext(accountInfo.UserId))
                {
                    var login = new UserLoginInfo(accountDb, db, _redis, accountInfo, protoOnLogin);
                    await login.GetProtoOnLogin(PlatformType.Google, "KR");
                    LoginResult = protoOnLogin;
                    return protoOnLogin.UserId;
                }
            }
        }

        private string MakeRandomNickname()
        {
            var guid = System.Guid.NewGuid();
            return guid.ToString().Substring(0, 8);
        }

        public async Task LeftOnlyOneUnit()
        {
            using (var db = new DBContext(_player1UserId))
            {
                var piece = PieceFactory.CreatePiece(PieceType.Unit, _player1UserId, 0, db);
                var units = await piece.SelectAll();
                for (int i=0; i < units.Count; i++)
                {
                    if (i != 0)
                        await db.ExecuteNonQueryAsync($"DELETE FROM units WHERE userId={_player1UserId} AND id={units[i].Id};");
                }
            }
        }

        public async Task LeftOnlyOneCard()
        {
            using (var db = new DBContext(_player1UserId))
            {
                var piece = PieceFactory.CreatePiece(PieceType.Card, _player1UserId, 0, db);
                var cards = await piece.SelectAll();
                for (int i = 0; i < cards.Count; i++)
                {
                    if (i != 0)
                        await db.ExecuteNonQueryAsync($"DELETE FROM cards WHERE userId={_player1UserId} AND id={cards[i].Id};");
                }
            }
        }

        public async Task SetPiece(ItemType itemType, uint id, uint level, int count)
        {
            switch (itemType)
            {
                case ItemType.UnitPiece:
                    await SetUnit(id, level, count);
                    break;
                case ItemType.CardPiece:
                    await SetCard(id, level, count);
                    break;
                case ItemType.WeaponPiece:
                    await SetWeapon(id, level, count);
                    break;
                default:
                    throw new Exception();
            }
        }
               
        public async Task SetUnit(uint id, uint level, int count)
        {
            using (var db = new DBContext(_player1UserId))
            {
                await db.ExecuteNonQueryAsync($"UPDATE units SET level={level}, count={count} " +
                    $"WHERE userId={_player1UserId} AND id={id};");
            }
        }

        public async Task SetCard(uint id, uint level, int count)
        {
            using (var db = new DBContext(_player1UserId))
            {
                await db.ExecuteNonQueryAsync($"UPDATE cards SET level={level}, count={count} " +
                    $"WHERE userId={_player1UserId} AND id={id};");
            }
        }

        public async Task SetWeapon(uint id, uint level, int count)
        {
            using (var db = new DBContext(_player1UserId))
            {
                await db.ExecuteNonQueryAsync($"UPDATE weapons SET level={level}, count={count} " +
                    $"WHERE userId={_player1UserId} AND id={id};");
            }
        }

        public uint GetOneUnitId()
        {
            foreach(var unit in UserInventoryChecker._beforeInventory.Units)
            {
                return unit.Value.id;
            }
            throw new Exception();
        }

        public uint GetOneCardId()
        {
            foreach (var card in UserInventoryChecker._beforeInventory.Cards)
            {
                return card.Value.id;
            }
            throw new Exception();
        }

        private async Task DeleteAccountDB(uint userId)
        {
            if (userId == 0)
                return;

            using (var db = new DBContext("AccountDBSetting"))
            {
                await db.ExecuteNonQueryAsync($"DELETE FROM accounts WHERE userId={userId}");
            }
        }

        private async Task DeleteUserDB(uint userId)
        {
            if (userId == 0)
                return;

            DBParamInfo paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$userId", userId)
            );

            using (var db = new DBContext(userId))
            {
                await db.CallStoredProcedureAsync("p_deleteUserDatas", paramInfo);
            }
        }

        public async Task<ProtoOnGetDeckWithDeckNum> GetDeckInfo(AkaEnum.ModeType modeType)
        {
            var userId = _playerInfo.Player1UserId;
            byte deckNum = _playerInfo.Player1DeckNum;
            var webServerIp = 0;
            var battleType = _playerInfo.BattleType;

            /*
            if (playerType == PlayerType.Player2)
            {
                userId = _playerInfo.Player2UserId;
                deckNum = _playerInfo.Player2DeckNum;
                webServerIp = 0;
            }
            */

            using (var db = new DBContext(userId))
            {
                var deck = new WebLogic.Deck.Deck(db, userId);
                return new ProtoOnGetDeckWithDeckNum
                {
                    UserAndDecks = new Dictionary<uint, ProtoDeckWithDeckNum>() { { userId, await deck.GetDeckWithDeckNum(deckNum, modeType, battleType) } }
                };
            }
        }

        public async Task<uint> AddRandomPiece(ItemType itemType, uint level, int count)
        {
            switch(itemType)
            {
                case ItemType.UnitPiece:
                    return await AddRandomUnit(level, count);
                case ItemType.CardPiece:
                    return await AddRandomCard(level, count);
                case ItemType.WeaponPiece:
                    return await AddRandomWeapon(level, count);
                default:
                    throw new Exception();
            }
        }

        public async Task<uint> AddRandomUnit(uint level, int count)
        {
            var unit = Data.GetPrimitiveDict<uint, DataUnit>(DataType.data_unit).Values.First();
            using (var db = new DBContext(_player1UserId))
            {
                await db.ExecuteNonQueryAsync(
                    $"INSERT INTO units (userId, id, level, count) " +
                    $"VALUES({_player1UserId}, {unit.UnitId}, {level}, {count}) " +
                    $"ON DUPLICATE KEY UPDATE level = {level}, count = {count};");
            }

            return unit.UnitId;
        }
        public async Task<uint> AddRandomCard(uint level, int count)
        {
            var card = Data.GetPrimitiveDict<uint, DataCard>(DataType.data_card).Values.First();
            using (var db = new DBContext(_player1UserId))
            {
                await db.ExecuteNonQueryAsync(
                    $"INSERT INTO cards (userId, id, level, count) " +
                    $"VALUES({_player1UserId}, {card.CardId}, {level}, {count}) " +
                    $"ON DUPLICATE KEY UPDATE level = {level}, count = {count};");
            }

            return card.CardId;
        }

        public async Task<List<ProtoEmoticonInfo>> GetEmoticons()
        {
            List<ProtoEmoticonInfo> emoticons = new List<ProtoEmoticonInfo>(); 
            using (var db = new DBContext(_player1UserId))
            {
                var query = new System.Text.StringBuilder();
                query.Append("SELECT id, orderNum, unitId FROM emoticons WHERE userId=").Append(_player1UserId);
                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    while (cursor.Read())
                        emoticons.Add(new ProtoEmoticonInfo
                        {
                            EmoticonId = (uint)cursor["id"],
                            UnitId = (uint)cursor["unitId"],
                            OrderNum = (int)cursor["orderNum"]
                        });
                }

            }
            return emoticons;
        }

        public async Task AddEmoticon(uint emoticonId)
        {
            using (var db = new DBContext(_player1UserId))
            {
                await ItemFactory.CreateItem(ItemType.Emoticon, _player1UserId, emoticonId, db).Get("Test");
            }
        }

        public async Task UpdateEmoticon(uint emoticonId, uint unitid, int order)
        {
            using (var db = new DBContext(_player1UserId))
            {
                await ItemFactory.CreateEmoticon(ItemType.Emoticon, _player1UserId, emoticonId, db, order,unitid).SetOrder();
            }
        }

        public async Task<uint> AddRandomWeapon(uint level, int count)
        {
            var weapon = Data.GetPrimitiveDict<uint, DataWeapon>(DataType.data_weapon).Values.First();
            using (var db = new DBContext(_player1UserId))
            {
                await db.ExecuteNonQueryAsync(
                    $"INSERT INTO weapons (userId, id, level, count) " +
                    $"VALUES({_player1UserId}, {weapon.WeaponId}, {level}, {count}) " +
                    $"ON DUPLICATE KEY UPDATE level = {level}, count = {count};");
            }

            return weapon.WeaponId;
        }

        public bool IsValidBeforeAfterInventoryStatus(ItemType itemType, uint id, uint level, int increaseCount)
        {
            var sumMaxRequirePieceCount = GetSumMaxRequirePieceCount(itemType, id, level);
            return IsValidBeforeAfterInventoryCountStatus(itemType, id, increaseCount, sumMaxRequirePieceCount);
        }
        public bool IsValidBeforeAfterInventoryCountStatus(ItemType itemType, uint id, int increaseCount, int sumMaxRequirePieceCount)
        {
            if (increaseCount > sumMaxRequirePieceCount)
            {
                var increasedGold
                    = (increaseCount - sumMaxRequirePieceCount)
                    * GetMaxPieceToGold(itemType);

                return UserInventoryChecker._beforeInventory.Gold + increasedGold
                    == UserInventoryChecker._afterInventory.Gold
                    && GetBeforePieceCount(itemType, id) + sumMaxRequirePieceCount
                    == GetAfterPieceCount(itemType, id);
            }
            else
            {
                return GetBeforePieceCount(itemType, id) + increaseCount
                    == GetAfterPieceCount(itemType, id);
            }
        }

        private int GetBeforePieceCount(ItemType itemType, uint id)
        {
            switch (itemType)
            {
                case ItemType.UnitPiece:
                    return UserInventoryChecker._beforeInventory.Units[id].count;
                case ItemType.CardPiece:
                    return UserInventoryChecker._beforeInventory.Cards[id].count;
                case ItemType.WeaponPiece:
                    return UserInventoryChecker._beforeInventory.Weapons[id].count;
                default:
                    throw new Exception();
            }
        }

        private int GetAfterPieceCount(ItemType itemType, uint id)
        {
            switch (itemType)
            {
                case ItemType.UnitPiece:
                    return UserInventoryChecker._afterInventory.Units[id].count;
                case ItemType.CardPiece:
                    return UserInventoryChecker._afterInventory.Cards[id].count;
                case ItemType.WeaponPiece:
                    return UserInventoryChecker._afterInventory.Weapons[id].count;
                default:
                    throw new Exception();
            }
        }

        public int GetSumMaxRequirePieceCount(ItemType itemType, uint id, uint level)
        {
            switch(itemType)
            {
                case ItemType.UnitPiece:
                    return GetSumMaxRequireUnitPieceCount(id, level);
                case ItemType.CardPiece:
                    return GetSumMaxRequireCardPieceCount(id, level);
                case ItemType.WeaponPiece:
                    return GetSumMaxRequireWeaponPieceCount(id, level);
                default:
                    throw new Exception();
            }
        }

        public int GetSumMaxRequireUnitPieceCount(uint id, uint level)
        {
            return Math.Max(Data.GetPrimitiveDict<uint, DataUnitStat>(DataType.data_unit_stat_all).
                Values.Where(datas => datas.UnitId == id && datas.Level >= level).Sum(datas => datas.RequirePieceCountForNextLevelUp)
                - UserInventoryChecker._beforeInventory.Units[id].count, 0);
        }

        public int GetSumMaxRequireCardPieceCount(uint id, uint level)
        {
            return Math.Max(Data.GetPrimitiveDict<uint, DataCardStat>(DataType.data_card_stat_all).
                Values.Where(datas => datas.CardId == id && datas.Level >= level).Sum(datas => datas.RequirePieceCountForNextLevelUp)
                - UserInventoryChecker._beforeInventory.Cards[id].count, 0);
        }

        public int GetSumMaxRequireWeaponPieceCount(uint id, uint level)
        {
            return Math.Max(Data.GetPrimitiveDict<uint, DataWeaponStat>(DataType.data_weapon_stat_all).
                Values.Where(datas => datas.WeaponId == id && datas.Level >= level).Sum(datas => datas.RequirePieceCountForNextLevelUp)
                - UserInventoryChecker._beforeInventory.Weapons[id].count, 0);
        }

        private int GetMaxPieceToGold(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.UnitPiece:
                    return (int)(Data.GetConstant(DataConstantType.MAX_UNIT_PIECE_TO_GOLD).Value);
                case ItemType.CardPiece:
                    return (int)(Data.GetConstant(DataConstantType.MAX_CARD_PIECE_TO_GOLD).Value);
                case ItemType.WeaponPiece:
                    return (int)(Data.GetConstant(DataConstantType.MAX_WEAPON_PIECE_TO_GOLD).Value);
                default:
                    throw new Exception();
            }
        }

        public uint GetMaxLevel(ItemType itemType, uint id)
        {
            switch (itemType)
            {
                case ItemType.UnitPiece:
                    return Data.GetUnitAdditionalData(id).MaxLevel;
                case ItemType.CardPiece:
                    return Data.GetCardAdditionalData(id).MaxLevel;
                case ItemType.WeaponPiece:
                    return Data.GetWeaponAdditionalData(id).MaxLevel;
                default:
                    throw new Exception();
            }
        }

        public async Task SetInfusionBox(string columnName, int value)
        {
            using (var db = new DBContext(_player1UserId))
            {
                await db.ExecuteNonQueryAsync($"UPDATE infusion_boxes SET {columnName}={value} WHERE userId={_player1UserId};");
            }
        }

        public void Dispose()
        {
            DeleteAccountDB(_player1UserId);
            DeleteAccountDB(_player2UserId);
            DeleteUserDB(_player1UserId);
            DeleteUserDB(_player2UserId);
        }
    }
}

