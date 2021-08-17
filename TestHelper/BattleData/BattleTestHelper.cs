using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using BattleLogic;
using CommonProtocol;
using System;
using System.Threading.Tasks;

namespace TestHelper.BattleData
{
    public class BattleTestHelper : IDisposable
    {
        private Battle _battle;
        private UserController _userController;

        public async Task MakeBattle()
        {
            _userController = new UserController();
            await _userController.MakeTwoUserData();

            var battleCreator = new BattleCreator(_userController.PlayerInfo, _userController.GetDeckInfo);
            _battle = (await battleCreator.GetBattle(0, _userController.PlayerInfo.BattleType, BattleEnd, null)).battle;
        }

        public void SetOneVsOne()
        {
            _battle.Players[PlayerType.Player1].Units[1].DecreaseHp(100000, DamageReasonType.NormalAttack, 0, false);
            _battle.Players[PlayerType.Player1].Units[2].DecreaseHp(100000, DamageReasonType.NormalAttack, 0, false);
            _battle.Players[PlayerType.Player2].Units[1].DecreaseHp(100000, DamageReasonType.NormalAttack, 0, false);
            _battle.Players[PlayerType.Player2].Units[2].DecreaseHp(100000, DamageReasonType.NormalAttack, 0, false);
        }

        public void DoAttack(PlayerType playerType, int unitPositionIndex)
        {
            _battle.Players[playerType].Units[unitPositionIndex].DoAttack();
        }

        public void DoSkill(PlayerType playerType, int unitPositionIndex,
            PlayerType targetPlayerType, int targetUnitPositionIndex,
            uint cardId, uint cardLevel)
        {
            var dataCard = Data.GetCard(cardId);
            var dataCardStat = Data.GetCardStat(dataCard.CardId, cardLevel);
            var card = new Card(dataCard, dataCardStat);

            _battle.Players[playerType].Units[unitPositionIndex]
                .DoSkill(new CardUseActionData
                {
                    UseCard = card,
                    Target = new ProtoTarget
                    {
                        PlayerType = targetPlayerType,
                        UnitPositionIndex = targetUnitPositionIndex
                    },
                    ReplaceCardInfo = new ReplaceCardInfo
                    {
                        NextCardStatId = null,
                        ReplacedCard = null,
                        ReplacedHandIndex = 0
                    }
                });
        }

        private void BattleEnd(PlayerType winPlayer, bool isRetreat)
        {
        }

        public void SetHp(PlayerType playerType, int unitPositionIndex, int hp)
        {
            _battle.Players[playerType].Units[unitPositionIndex].UnitData.UnitStatus.Hp = hp;
        }

        public void SetAtk(PlayerType playerType, int unitPositionIndex, int atk)
        {
            _battle.Players[playerType].Units[unitPositionIndex].UnitData.UnitStatus.Atk = atk;
        }

        public void SetCriRate(PlayerType playerType, int unitPositionIndex, float criticalRate)
        {
            _battle.Players[playerType].Units[unitPositionIndex].UnitData.UnitStatus.CriticalRate = criticalRate;
        }

        public int GetHp(PlayerType playerType, int unitPositionIndex)
        {
            return _battle.Players[playerType].Units[unitPositionIndex].UnitData.UnitStatus.Hp;
        }

        public int GetShield(PlayerType playerType, int unitPositionIndex)
        {
            return _battle.Players[playerType].Units[unitPositionIndex].Shield;
        }

        public void SetShield(PlayerType playerType, int unitPositionIndex, int shield)
        {
            //_battle.Players[playerType].Units[unitPositionIndex].AddShield(shield);
        }

        public void SetAttackDelay(PlayerType playerType, int unitPositionIndex, int delay)
        {
            _battle.Players[playerType].Units[unitPositionIndex].UnitData.UnitStatus.AttackDelay = 0;
        }

        public void ProgressTimerStart()
        {
            _battle.BattleHelper.BattleProgress.TimerStart();
        }

        public void Dispose()
        {
            _userController.Dispose();
        }
    }
}
