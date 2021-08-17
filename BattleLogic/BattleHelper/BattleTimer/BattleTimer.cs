namespace BattleLogic
{
    public class BattleTimer
    {
        private Battle _battle;

        public void SetBattle(Battle battle)
        {
            _battle = battle;
        }

        public void Start()
        {
            _battle.BattleTimer.Start();
            _battle.BoosterStartTimer.Start();

            foreach (var player in _battle.Players)
            {
                player.Value.Start(_battle.Status.StartDateTime);

                foreach (var unit in player.Value.Units)
                {
                    unit.Value.AttackTimer.Start();
                }
            }
        }

        public void Pause()
        {
            _battle.BattleTimer.Pause();
            _battle.BoosterStartTimer.Pause();

            foreach (var player in _battle.Players)
            {
                player.Value.Pause();
                foreach (var unit in player.Value.Units)
                {
                    unit.Value.Pause();
                }
            }
        }

        public void Restart(int bulletTime)
        {
            _battle.BattleTimer.Restart(bulletTime);
            _battle.BoosterStartTimer.Restart(bulletTime);

            foreach (var player in _battle.Players)
            {
                player.Value.Restart(bulletTime);
                foreach (var unit in player.Value.Units)
                {
                    unit.Value.Restart(bulletTime);
                }
            }
        }
    }
}
