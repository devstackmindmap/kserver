using AkaDB.MySql;
using CommonProtocol;
using System.Threading.Tasks;
using AkaEnum;
using Common.Entities.Deck;
using Common.Entities.Unit;
using Common.Entities.Card;
using AkaData;
using AkaEnum.Battle;
using AkaUtility;
using AkaLogger;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace WebLogic.Deck
{
    public class Deck
    {
        private DBContext _db;
        private uint _userId;

        public Deck(DBContext db , uint userId)
        {
            _db = db;
            _userId = userId;
        }

        public async Task<ProtoDeckWithDeckNum> GetDeckWithDeckNum(byte deckNum, ModeType modeType, BattleType battleType)
        {
            var onProto = new ProtoDeckWithDeckNum
            {
                DeckNum = deckNum
            };
            onProto.Deck = await GetDeck(_userId, modeType, deckNum);
            
            var isFriendlyBattleType = IsMaxLevelBattleType(battleType);
            var weaponIds = new Dictionary<int, uint>();
            foreach (var deckElement in onProto.Deck.DeckElements)
            {
                if (deckElement.SlotType == SlotType.Unit)
                    onProto.UnitsInfo.Add(deckElement.OrderNum, await GetUnitsInfo(isFriendlyBattleType, deckElement));

                if (deckElement.SlotType == SlotType.Card)
                    onProto.CardsLevel.Add(deckElement.OrderNum, await GetCardsInfo(isFriendlyBattleType, deckElement));

                if (deckElement.SlotType == SlotType.Weapon)
                    weaponIds.Add(deckElement.OrderNum, deckElement.ClassId);
            }

            await SetWeaponInUnit(onProto, weaponIds);
            await SaveRecentDeck(battleType, onProto.Deck, deckNum, modeType, weaponIds);
            return onProto;
        }

        private async Task<ProtoOnGetDeck> GetDeck(uint userId, ModeType modeType, byte deckNum)
        {
            var getDeck = new DBGetDeck
            {
                Db = _db,
                UserId = userId,
                ModeType = modeType,
                DeckNum = deckNum,
                IsAllDeckNum = false
            };

            return await getDeck.ExecuteAsync();
        }

        private bool IsMaxLevelBattleType(BattleType battleType)
        {
            return battleType.In(BattleType.FriendlyBattle);
        }

        private async Task<ProtoUnitInfoForBattle> GetUnitsInfo(bool isFriendlyBattleType, ProtoDeckElement deckElement)
        {
            return isFriendlyBattleType ? await GetFriendlyBattleTypeUnitInfo(deckElement) : await GetUnitsInfo(deckElement);
        }

        private async Task<ProtoUnitInfoForBattle> GetUnitsInfo(ProtoDeckElement deckElement)
        {
            return new ProtoUnitInfoForBattle
            {
                Level = await (new DBGetUnitInfo { StrUserId = _userId.ToString(), StrUnitId = deckElement.ClassId.ToString() }.GetLevel(_db)),
                SkinId = await (new DBGetUnitSkinId { StrUserId = _userId.ToString(), StrUnitId = deckElement.ClassId.ToString() }.ExecuteAsync(_db))
            };
        }

        private async Task<ProtoUnitInfoForBattle> GetFriendlyBattleTypeUnitInfo(ProtoDeckElement deckElement)
        {            
            return new ProtoUnitInfoForBattle
            {
                Level = Data.GetUnitAdditionalData(deckElement.ClassId).MaxLevel,
                SkinId = await (new DBGetUnitSkinId { StrUserId = _userId.ToString(), StrUnitId = deckElement.ClassId.ToString() }.ExecuteAsync(_db)),
                WeaponInfo = null
            };
        }

        private async Task SetWeaponInUnit(ProtoDeckWithDeckNum returnDeckInfo, Dictionary<int, uint> weaponIds)
        {
            foreach (var deckElement in returnDeckInfo.Deck.DeckElements)
            {
                if (deckElement.SlotType == SlotType.Unit)
                {
                    if (weaponIds.ContainsKey(deckElement.OrderNum))
                    {
                        returnDeckInfo.UnitsInfo[deckElement.OrderNum].WeaponInfo
                            = await GetWeaponInfo(weaponIds[deckElement.OrderNum]);
                    }
                }
            }
        }

        private async Task SetWeaponInUnitRecentDeck(ProtoOnGetRecentDeck returnDeckInfo, Dictionary<int, uint> weaponIds)
        {
            foreach (var deckElement in returnDeckInfo.UnitsInfo)
            {
                
                if (weaponIds.ContainsKey(deckElement.Key))
                {
                    returnDeckInfo.UnitsInfo[deckElement.Key].WeaponInfo
                        = await GetWeaponInfo(weaponIds[deckElement.Key]);
                }
            }
        }

        public async Task<ProtoOnGetRecentDeck> GetRecentDeck(ModeType modeType, byte deckNum)
        {
            var deckInfo = await GetDeck(_userId, modeType, deckNum);
            return await GetRecentDeck(deckInfo);
        }

        public async Task<ProtoOnGetRecentDeck> GetRecentDeck(ProtoOnGetDeck deckInfo)
        {
            var onProto = new ProtoOnGetRecentDeck();

            var weaponIds = new Dictionary<int, uint>();
            foreach (var deckElement in deckInfo.DeckElements)
            {
                if (deckElement.SlotType == SlotType.Unit)
                    onProto.UnitsInfo.Add(deckElement.OrderNum, await GetUnitsInfoForRecentDeck(deckElement));

                if (deckElement.SlotType == SlotType.Card)
                    onProto.CardsInfo.Add(deckElement.OrderNum, await GetCardsInfoForRecentDeck(deckElement));

                if (deckElement.SlotType == SlotType.Weapon)
                    weaponIds.Add(deckElement.OrderNum, deckElement.ClassId);
            }

            await SetWeaponInUnitRecentDeck(onProto, weaponIds);

            return onProto;
        }


        private async Task<ProtoUnitInfoForRecentDeck> GetUnitsInfoForRecentDeck(ProtoDeckElement deckElement)
        {
            var deckUnitInfo = new ProtoUnitInfoForRecentDeck();
            await (new DBGetUnitInfo { StrUserId = _userId.ToString(), StrUnitId = deckElement.ClassId.ToString() }.GetInfos(_db, deckUnitInfo));
            return deckUnitInfo;
        }

        private async Task<uint> GetCardsInfo(bool isMaxLevelBattleType, ProtoDeckElement deckElement)
        {
            return isMaxLevelBattleType ? GetMaxCardLevel(deckElement) : await GetCardLevel(deckElement);
        }

        private async Task<ProtoCardInfoExceptCount> GetCardsInfoForRecentDeck(ProtoDeckElement deckElement)
        {
            return await (new DBGetCardInfo
            {
                StrUserId = _userId.ToString(),
                StrCardId = deckElement.ClassId.ToString()
            }).GetInfos(_db);
        }

        private async Task<uint> GetCardLevel(ProtoDeckElement deckElement)
        {
            return await (new DBGetCardInfo
            {
                StrUserId = _userId.ToString(),
                StrCardId = deckElement.ClassId.ToString()
            }).GetLevel(_db);
        }

        private uint GetMaxCardLevel(ProtoDeckElement deckElement)
        {
            return Data.GetCardAdditionalData(deckElement.ClassId).MaxLevel;
        }

        private async Task<ProtoWeaponInfoForBattle> GetWeaponInfo(uint classId)
        {
            using (var cursor = await _db.ExecuteReaderAsync($"SELECT level FROM weapons WHERE userId = {_userId} AND id = {classId};"))
            {
                if (cursor.Read())
                {
                    return new ProtoWeaponInfoForBattle
                    {
                        Id = classId,
                        Level = (uint)cursor["level"]
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<ResultType> SetDeck(List<ProtoDeckElement> updateDecks, bool IsRecentSaveDeck = false)
        {
            if (updateDecks == null)
                return ResultType.Fail;

            var resultType = DeckHelper.CheckRequestParameter(await IsAddDeck(), updateDecks);

            if (resultType != ResultType.Success)
                return resultType;

            foreach (var deckElement in updateDecks)
            {
                await DBSetDeck((byte)(
                    IsRecentSaveDeck ? ModeType.RecentKnightLeagueDeck : deckElement.ModeType),
                    (byte)deckElement.SlotType,
                    IsRecentSaveDeck ? (byte)0 : deckElement.DeckNum,
                    deckElement.OrderNum,
                    deckElement.ClassId);
            }

            SetDeckLog(updateDecks, IsRecentSaveDeck);
            return ResultType.Success;
        }

        private async Task<bool> IsAddDeck()
        {
            var query = new StringBuilder();
            query.Append("SELECT addDeck FROM user_info WHERE userId = ").Append(_userId).Append(";");

            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read() || (int)cursor["addDeck"] == 0)
                    return false;
                return true;
            }
        }

        private async Task DBSetDeck(byte modeType, byte slotType, byte deckNum, byte orderNum, uint classId)
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO decks(userId, modeType, slotType, deckNum, orderNum, classId)")
                .Append("VALUES(").Append(_userId).Append(",").Append(modeType).Append(",")
                .Append(slotType).Append(",").Append(deckNum).Append(",").Append(orderNum)
                .Append(",").Append(classId).Append(") ON DUPLICATE KEY UPDATE classId = ")
                .Append(classId).Append(";");

            await _db.ExecuteNonQueryAsync(query.ToString());
        }

        private void SetDeckLog(List<ProtoDeckElement> updateDecks, bool IsRecentSaveDeck)
        {
            try
            {
                if (IsRecentSaveDeck == false && updateDecks.Count > 0)
                {
                    var modeType = updateDecks.First().ModeType;

                    var decksForNum = updateDecks.OrderBy(deckElement => deckElement.OrderNum)
                                                 .GroupBy(deckElement => deckElement.DeckNum);

                    foreach (var deck in decksForNum)
                    {

                        var deckList = deck.GroupBy(deckElement => (int)deckElement.SlotType)
                                .ToDictionary(deckElementGroup => deckElementGroup.Key, deckElementGroup => string.Join(",", deckElementGroup.Select(deckElement => deckElement.OrderNum + "-" + deckElement.ClassId)));

                        Log.User.Deck.Log(_userId, (byte)modeType, deck.Key, deckList);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug.Exception("WriteSetDeckLog:" + _userId, e);
            }
        }

        private async Task SaveRecentDeck(BattleType battleType, ProtoOnGetDeck deck, byte deckNum, ModeType modeType, Dictionary<int, uint> weaponIds)
        {
            if (false == battleType.In(BattleType.LeagueBattle, BattleType.LeagueBattleAi))
                return;

            await SetDeck(deck.DeckElements, true);
            await SetEmptyWeapons(deckNum, modeType, weaponIds);
        }

        private async Task SetEmptyWeapons(byte deckNum, ModeType modeType, Dictionary<int, uint> weaponIds)
        {
            List<ProtoDeckElement> setEmptyWeapons = new List<ProtoDeckElement>();
            for (byte i=0; i < 3; i++)
            {
                if (false == weaponIds.ContainsKey(i))
                {
                    setEmptyWeapons.Add(new ProtoDeckElement
                    {
                        ClassId = 0,
                        OrderNum = i,
                        DeckNum = deckNum,
                        SlotType = SlotType.Weapon,
                        ModeType = modeType
                    });
                }
            }

            if (setEmptyWeapons.Count > 0)
                await SetDeck(setEmptyWeapons, true);
        }
    }
}
