using AkaData;
using AkaEnum.Battle;
using AkaRedis;
using AkaSerializer;
using BattleLogic;
using CommonProtocol;
using Network;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using AkaUtility;
using AkaEnum;

namespace BattleServer
{
    public class Stage
    {
        internal readonly string KeyValue;
        internal readonly uint StageLevelId;
        internal readonly DateTime CreatedTime = DateTime.UtcNow;

        internal uint CurrentRound => _currentRound;

        private uint _currentRound;
        private Dictionary<uint, uint> _stageRoundIdMap;


        private List<uint> _roguelikeCardStatIdList;
        private List<uint> _roguelikeUnitIdList;

        private IList<IList<uint>> _roguelikeProposalCardStatList;
        private IList<IList<uint>> _proposalTreasureList;


        public List<uint> SaveDeckCardStatIdList => _roguelikeCardStatIdList;

        //public Stage(BattleInfoRoguelike battleInfo)
        public Stage(BattleInfo battleInfo)
        {
            KeyValue = battleInfo.UserId.ToString();
            StageLevelId = battleInfo.StageLevelId;

            if (battleInfo.BattleType.In(BattleType.LeagueBattleAi, BattleType.PracticeBattle, BattleType.VirtualLeagueBattle))
            {
                _stageRoundIdMap = new Dictionary<uint, uint> { { _currentRound, battleInfo.StageRoundId } };
                _currentRound = 0;
            }
            else
            {
                var stageRoundList = Data.GetStageRoundList(StageLevelId);
                _currentRound = stageRoundList.Find(roundInfo => roundInfo.StageRoundId == battleInfo.StageRoundId).Round;
                _stageRoundIdMap = stageRoundList.ToDictionary(stageRound => stageRound.Round, stageRound => stageRound.StageRoundId);
            }

            if (ModeType.SaveDeck == BattleEnviroment.GetDeckModeType(battleInfo.BattleType))
            {
                var stage = Data.GetStageLevel(StageLevelId);
                var roguelikeSaveDeck = Data.GetRoguelikeSaveDeck(stage.RoguelikeSaveDeckId);
                var proposalTreasure = Data.GetProposalTreasure(roguelikeSaveDeck.ProposalTreasureId);

                _roguelikeUnitIdList = new List<uint>(roguelikeSaveDeck.UnitIdList);
                _roguelikeProposalCardStatList = roguelikeSaveDeck.ProposalCardStatList;
                _proposalTreasureList = proposalTreasure?.TreasureIdList;

                if (_currentRound == 0)
                {
                    _roguelikeCardStatIdList = new List<uint>(roguelikeSaveDeck.CardStatIdList);
                }
                else
                {
                    //_roguelikeCardStatIdList = new List<uint>(battleInfo.SaveDeckCardStatIdList);
                }
            }
        }

        public bool RoundClearAndGoNextRound(ProtoStageRoundResult clearInfo)
        {
            _currentRound++;
            if (0 != _stageRoundIdMap.SafeGet(_currentRound))
            {
                //only savedeck contents
                if (_roguelikeProposalCardStatList != null)
                {
                    var stageRoundCardList = _roguelikeProposalCardStatList.ElementAtOrDefault((int)_currentRound - 1);
                    int resultCount = (int)Data.GetConstant(DataConstantType.SUGGEST_CARD_NUM).Value;
                    clearInfo.NextCardStatIdList = AkaRandom.Random.ChooseIndexRanddomlyInSourceByCount(stageRoundCardList, resultCount).ToList();
                }

                if (_proposalTreasureList != null)
                {
                    var treasureList = _proposalTreasureList.ElementAtOrDefault((int)_currentRound - 1);
                    int resultCount = (int)Data.GetConstant(DataConstantType.SUGGEST_TREASURE_NUM).Value;
                    clearInfo.ProposalTreasureIdList = AkaRandom.Random.ChooseIndexRanddomlyInSourceByCount(treasureList, resultCount).ToList();
                }
                return true;
            }
            return false;
        }

        public uint CurrentStageRoundId()
        {
            return _stageRoundIdMap.SafeGet(_currentRound);
        }

        internal ProtoDeckWithDeckNum GetCurrentRoundSaveDeck()
        {
            var deck = new ProtoDeckWithDeckNum();
            deck.UnitsInfo = new Dictionary<int, ProtoUnitInfoForBattle>();
            deck.CardsLevel = new Dictionary<int, uint>();
            deck.Nickname = "";

            ProtoUnitInfoForBattle protoUnitInfoForBattle = new ProtoUnitInfoForBattle
            {
                Level = 1
            };


            var unitElements = _roguelikeUnitIdList.Select((unitId, orderNum) => {
                deck.UnitsInfo.Add(orderNum, protoUnitInfoForBattle);
                return new ProtoDeckElement
                {
                    ClassId = unitId,
                    OrderNum = (byte)orderNum,
                    SlotType = SlotType.Unit
                };
            });

            var cardElements = _roguelikeCardStatIdList.Select((cardStatId, orderNum) => {
                var cardStat = Data.GetCardStat(cardStatId);
                deck.CardsLevel.Add(orderNum, cardStat.Level);

                return new ProtoDeckElement
                {
                    ClassId = cardStat.CardId,
                    OrderNum = (byte)orderNum,
                    SlotType = SlotType.Card
                };
            });


            var proto = new ProtoOnGetDeck
            {
                DeckElements = unitElements.Union(cardElements).ToList()
            };

            deck.Deck = proto;
            return deck;
        }
    }
}
