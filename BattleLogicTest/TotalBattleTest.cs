using AkaEnum;
using AkaEnum.Battle;
using AkaLogger;
using NUnit.Framework;
using System.Threading.Tasks;
using AkaUtility;
using TestHelper.BattleData;

namespace BattleLogicTest
{
    class TotalBattleTest
    {
        [SetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task NormalAttack()
        {

            using (BattleTestHelper helper = new BattleTestHelper())
            {
                await helper.MakeBattle();

                helper.SetOneVsOne();
                helper.SetHp(PlayerType.Player1, 0, 110);
                helper.SetAtk(PlayerType.Player2, 0, 100);
                helper.SetCriRate(PlayerType.Player2, 0, 0);
                helper.DoAttack(PlayerType.Player2, 0);
                Assert.AreEqual(helper.GetHp(PlayerType.Player1, 0), 10);
            }
        }

        [Test]
        public async Task NormalAttackToShield()
        {
            using (BattleTestHelper helper = new BattleTestHelper())
            {
                await helper.MakeBattle();

                helper.SetOneVsOne();
                helper.SetHp(PlayerType.Player1, 0, 110);
                helper.SetShield(PlayerType.Player1, 0, 50);
                helper.SetAtk(PlayerType.Player2, 0, 100);
                helper.SetCriRate(PlayerType.Player2, 0, 0);
                helper.SetAttackDelay(PlayerType.Player2, 0, 0);

                helper.DoAttack(PlayerType.Player2, 0);
                var hp = helper.GetHp(PlayerType.Player1, 0);
                var shield = helper.GetShield(PlayerType.Player1, 0);

                Logger.Instance().Info($"HP:{hp}, SHIELD:{shield}");

                Assert.True(
                    helper.GetHp(PlayerType.Player1, 0) == 60 &&
                    helper.GetShield(PlayerType.Player1, 0) == 0
                    );
            }
        }

        [Test]
        public async Task NormalAttackToShieldWith_BUFF_NEXT_ATTACK_SHIELD_IGNORE()
        {
            using (BattleTestHelper helper = new BattleTestHelper())
            {
                await helper.MakeBattle();

                helper.SetOneVsOne();
                helper.SetHp(PlayerType.Player1, 0, 110);
                helper.SetShield(PlayerType.Player1, 0, 50);
                helper.SetAtk(PlayerType.Player2, 0, 100);
                helper.SetCriRate(PlayerType.Player2, 0, 0);
                helper.SetAttackDelay(PlayerType.Player2, 0, 0);

                helper.DoSkill(PlayerType.Player2, 0, PlayerType.Player2, 0, 5039, 1);
                helper.DoAttack(PlayerType.Player2, 0);
                var hp = helper.GetHp(PlayerType.Player1, 0);
                var shield = helper.GetShield(PlayerType.Player1, 0);

                Logger.Instance().Info($"HP:{hp}, SHIELD:{shield}");

                Assert.True(
                    helper.GetHp(PlayerType.Player1, 0) == 10 &&
                    helper.GetShield(PlayerType.Player1, 0) == 50
                    );
            }
        }

        [Test]
        public async Task BuffSkillElectricShock()
        {
            using (BattleTestHelper helper = new BattleTestHelper())
            {
                await helper.MakeBattle();

                helper.SetOneVsOne();
                helper.SetHp(PlayerType.Player2, 0, 200);
                helper.SetShield(PlayerType.Player2, 0, 50);
                helper.SetAtk(PlayerType.Player2, 0, 100);

                helper.DoSkill(PlayerType.Player1, 0, PlayerType.Player2, 0, 5068, 1);
                helper.DoSkill(PlayerType.Player1, 0, PlayerType.Player2, 0, 5068, 1);

                helper.ProgressTimerStart();

                await Task.Delay(20);

                Assert.True(helper.GetHp(PlayerType.Player2, 0) == 50);
            }
        }
    }
}
