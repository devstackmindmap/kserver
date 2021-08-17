using AkaEnum.Battle;
using CommonProtocol;
using System.Collections.Generic;

namespace BattleLogic
{
    public interface IBattleRecorder
    {
        void SetBattle(Battle battle);

        void EnqueueBehaviorForS2CRecord(BattleSendData sendData);

        void BattleResultRecord(BattleSendData battleSendData);

        void C2SRecord();

        void StateRecord();

        CommonProtocol.ProtoBattleRecord ToBattleRecord(PlayerType winPlayer);

        void SetBattleStartInfo(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts);

        void SetPlayerDeckInfo(ProtoOnGetDeckWithDeckNum playerDeckInfo);
    }
}
