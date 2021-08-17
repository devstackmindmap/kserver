using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;

namespace Common.Entities.Deck
{
    public static class DeckHelper
    {
        public static ResultType CheckRequestParameter(bool isAddDeck, List<ProtoDeckElement> deckElements)
        {
            foreach (var deckElement in deckElements)
            {
                var resultType = CheckRequestParameter(isAddDeck, deckElement);
                if (resultType != ResultType.Success)
                    return resultType;
            }

            return ResultType.Success;
        }

        public static ResultType CheckRequestParameter(bool isAddDeck, ProtoDeckElement deckElement)
        {
            if (IsInvalidDeckNum(isAddDeck, deckElement.DeckNum))
                return ResultType.InvalidDeckNum;

            if (deckElement.SlotType == SlotType.Unit)
            {
                if (IsInvalidUnitOrderNum(deckElement.OrderNum))
                    return ResultType.InvalidUnitOrderNum;
            }

            if (deckElement.SlotType == SlotType.Card)
            {
                if (IsInvalidCardOrderNum(deckElement.OrderNum))
                    return ResultType.InvalidCardOrderNum;
            }

            if (deckElement.SlotType == SlotType.Weapon)
            {
                if (IsInvalidWeaponOrderNum(deckElement.OrderNum))
                    return ResultType.InvalidWeaponOrderNum;
            }

            return ResultType.Success;
        }

        public static bool IsInvalidDeckNum(bool isAddDeck, uint deckNum)
        {
            var deckMax = isAddDeck ? (int)AddDeckMinMax.Max : (int)DeckMinMax.Max;
            return deckNum < (int)DeckMinMax.Min || deckNum > deckMax;
        }

        public static bool IsInvalidUnitOrderNum(byte characterOrderNum)
        {
            return characterOrderNum < (byte)UnitOrderMinMax.Min || characterOrderNum > (byte)UnitOrderMinMax.Max;
        }

        public static bool IsInvalidCardOrderNum(byte cardOrderNum)
        {
            return cardOrderNum < (byte)CardOrderMinMax.Min || cardOrderNum > (byte)CardOrderMinMax.Max;
        }

        public static bool IsInvalidWeaponOrderNum(byte weaponOrderNum)
        {
            return weaponOrderNum < (byte)WeaponOrderMinMax.Min || weaponOrderNum > (byte)WeaponOrderMinMax.Max;
        }
    }
}
