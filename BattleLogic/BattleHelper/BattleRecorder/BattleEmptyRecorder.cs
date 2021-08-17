using AkaEnum.Battle;
using CommonProtocol;
using System.Collections.Generic;

namespace BattleLogic
{
    public class BattleEmptyRecorder : IBattleRecorder
    {
        public BattleEmptyRecorder()
        {
        }

        public void SetBattle(Battle battle)
        {
        }

        public void EnqueueBehaviorForS2CRecord(BattleSendData sendData)
        {
        }

        public void BattleResultRecord(BattleSendData battleSendData)
        {
        }

        public void C2SRecord()
        {
        }

        public void StateRecord()
        {
        }

        public CommonProtocol.ProtoBattleRecord ToBattleRecord(PlayerType winPlayer)
        {
            return null;
        }

        public void SetBattleStartInfo(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts)
        {

        }

        public void SetPlayerDeckInfo(ProtoOnGetDeckWithDeckNum playerDeckInfo)
        {
        }
    }
}
