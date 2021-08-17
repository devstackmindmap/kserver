using AkaData;
using AkaEnum;
using AkaUtility;
using CommonProtocol;

namespace BattleLogic
{
    public class DeckCreator
    {
        private uint _userId;
        private ModeType _modeType;
        private byte _deckNum;
        private ProtoDeckWithDeckNum _protoOnGetDeckForBattle;

        public DeckCreator(uint userId, ModeType modeType, byte deckNum, ProtoDeckWithDeckNum protoOnGetDeckForBattle)
        {
            _userId = userId;
            _modeType = modeType;
            _deckNum = deckNum;
            _protoOnGetDeckForBattle = protoOnGetDeckForBattle;
        }

        public Deck GetDeckByDeckInfo()
        {
            var deck = new Deck();
            
            foreach (var deckElement in _protoOnGetDeckForBattle.Deck.DeckElements)
            {
                if (deckElement.SlotType == SlotType.Unit)
                    deck.Units.Add(deckElement.OrderNum, GetUnit(deckElement, _protoOnGetDeckForBattle));

                if (deckElement.SlotType == SlotType.Card)
                    deck.Cards.Enqueue(GetCard(deckElement, _protoOnGetDeckForBattle));
            }

            AkaRandom.Random.Shuffle(deck.Cards);
            return deck;
        }

        private Unit GetUnit(ProtoDeckElement deckElement, ProtoDeckWithDeckNum deckInfo)
        {
            var unit = new Unit(deckElement.ClassId,
                    deckInfo.UnitsInfo[deckElement.OrderNum].Level,
                    deckInfo.UnitsInfo[deckElement.OrderNum].SkinId,
                    deckElement.OrderNum);
            unit.DataWeaponStat = ApplyWeaponStat(unit.UnitData.UnitStatus, deckInfo.UnitsInfo[deckElement.OrderNum].WeaponInfo);
            return unit;
        }

        private DataWeaponStat ApplyWeaponStat(UnitStatus unitStat, ProtoWeaponInfoForBattle weaponInfo)
        {
            var weaponStatInfo = weaponInfo == null ? null : Data.GetWeaponStat(weaponInfo.Id, weaponInfo.Level);
            if (weaponStatInfo == null)
                return null;

            unitStat.Atk += weaponStatInfo.Atk;
            unitStat.AttackSpeed += weaponStatInfo.AtkSpd;
            unitStat.CriticalRate += weaponStatInfo.CriRate;
            unitStat.Aggro += weaponStatInfo.Aggro;
            unitStat.Hp += weaponStatInfo.Hp;
            unitStat.MaxHp += weaponStatInfo.Hp;
            unitStat.CriticalDamageRate += weaponStatInfo.CriDmgRate;
            unitStat.ShieldDamageRate += weaponStatInfo.ShieldDamageRate;
            return weaponStatInfo;
        }

        private Card GetCard(ProtoDeckElement deckElement, ProtoDeckWithDeckNum deckInfo)
        {
            var dataCard = Data.GetCard(deckElement.ClassId);
            var dataCardStat = Data.GetCardStat(dataCard.CardId, deckInfo.CardsLevel[deckElement.OrderNum]);
            return new Card(dataCard, dataCardStat);
        }
    }
}
