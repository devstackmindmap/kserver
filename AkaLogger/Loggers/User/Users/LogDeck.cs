using System;
using System.Collections.Generic;

namespace AkaLogger.Users
{
    public sealed class LogDeck
    {
        public void Log(uint userId, byte modeType, byte deckNum, Dictionary<int, string> deckList)
        {
            List<string> logParam = new List<string>()
            {
                "Deck",
                "UserId", userId.ToString(),
                "DeckModeType", modeType.ToString(),
                "DeckNum", deckNum.ToString()
            };

            foreach( var deck in deckList)
            {
                logParam.Add(deck.Key.ToString());
                logParam.Add(deck.Value);
            }

            Logger.Instance().Analytics("Deck", logParam.ToArray());

        }
    }
}
