using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs
{
    public sealed partial class VOProvider
    {
        internal async Task Login(UserInfo userInfo)
        {
            if (userInfo.users.userId == 0)
                return;

            var onLogin = await Net.API.Login(userInfo.accounts.socialAccount);
            if (onLogin?.UserId != 0)
            {
                userInfo.LoginInfo = onLogin;

                Net.Pubsub.IamLogin(userInfo);
            }
        }

        internal async Task Join(string name)
        {
            name = name.Replace(" ", string.Empty);

            var onLogin = await Net.API.Join(name);
            if (onLogin?.UserId != 0)
            {
                await ReloadUserAccount(onLogin.UserId);

                var user = UserInfos.FirstOrDefault(userInfo => userInfo.accounts.userId == onLogin.UserId);
                user.LoginInfo = onLogin;

                var allQuests = Data.GetQuestWithProcessType(QuestProcessType.Completed);
                await Net.API.SetQuestForComplete(user, allQuests);

                await GetDeck(user, ModeType.PVP);
                if (user.UnitsWithPvPDeck.Count == 0)
                {
                    await SetDeckWithRandom(user, 0);
                    await GetDeck(user, ModeType.PVP);
                }
            }
        }

        internal async Task GetDeck(UserInfo userInfo, ModeType deckModeType)
        {

            var deck = await Net.API.GetDeck(userInfo, deckModeType);

            if (deck.DeckElements.Count > 0)
            {
                userInfo.UnitsWithPvPDeck.Clear();
                userInfo.CardsWithPvPDeck.Clear();
            }

            foreach (var deckElement in deck.DeckElements)
            {
                var targetContainer = deckElement.SlotType == SlotType.Unit ? userInfo.UnitsWithPvPDeck : userInfo.CardsWithPvPDeck;
                if (false == targetContainer.TryGetValue( deckElement.DeckNum, out var slotList))
                {
                    slotList = new List<uint>();
                    targetContainer.Add(deckElement.DeckNum, slotList);
                }

                if (slotList.Count <= deckElement.OrderNum)
                {
                    slotList.AddRange(Enumerable.Repeat((uint)0, deckElement.OrderNum + 1 - slotList.Count));
                }
                slotList[deckElement.OrderNum] = deckElement.ClassId;
            }
        }

        internal async Task SetDeckWithRandom(UserInfo userInfo, int deckNum)
        {
            var selectedUnits = AkaRandom.Random.ChooseIndexRanddomlyInSourceByCount(userInfo.LoginInfo.UnitInfoList, 3).ToList();
            var enableCards = userInfo.LoginInfo.CardInfoList.Select(card => Data.GetCard(card.Id))
                                                             .Where(card => ((int)card.CardId).In(5041,5042,5059,5068,5070,5071) == false && selectedUnits.Any(unit => unit.Id == card.UnitId));

            var selectedCards = new List<uint>();
            var selectedSpecials = new List<uint>();
            uint selectedUltimateCard = 0;
            while (selectedCards.Count < 8)
            {
                var card = AkaRandom.Random.ChooseElementRandomlyInCount(enableCards);
                if (card.CardRarity == CardRarity.ULTIMATE)
                {
                    if (selectedUltimateCard != 0)
                        continue;
                    selectedUltimateCard = card.CardId;
                }
                else if(card.CardRarity == CardRarity.SPECIAL)
                {
                    if (selectedSpecials.Any( cardId => cardId == card.CardId))
                        continue;
                    selectedSpecials.Add(card.CardId);
                }
                selectedCards.Add(card.CardId);
            }
            List<ProtoDeckElement> updateDeck = new List<ProtoDeckElement>();
            for( int i= 0; i < selectedUnits.Count; i++)
            {
                updateDeck.Add(new ProtoDeckElement
                {
                    ModeType = ModeType.PVP,
                    SlotType = SlotType.Unit,
                    DeckNum = (byte)deckNum,
                    OrderNum = (byte)i,
                    ClassId = selectedUnits[i].Id
                });
            }
            for (int i = 0; i < selectedCards.Count; i++)
            {
                updateDeck.Add(new ProtoDeckElement
                {
                    ModeType = ModeType.PVP,
                    SlotType = SlotType.Card,
                    DeckNum = (byte)deckNum,
                    OrderNum = (byte)i,
                    ClassId = selectedCards[i]
                });
            }

            await Net.API.SetDeck(userInfo, updateDeck);

        }
    }
}
