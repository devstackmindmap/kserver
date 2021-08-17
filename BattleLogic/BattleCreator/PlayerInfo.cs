using System;
using System.Collections.Generic;
using System.Text;

namespace BattleLogic
{
    public class PlayerInfo
    {
        public AkaEnum.ModeType DeckModeType;
        public AkaEnum.Battle.BattleType BattleType;
        public uint Player1UserId;
        public byte Player1DeckNum;
        public bool Player1Ready;
        public uint Player2UserId;
        public byte Player2DeckNum;
        public bool Player2Ready;
        public uint StageRoundId;
        public List<uint> Player1TreasureIdList;
        public List<uint> Player2TreasureIdList;
    }
}
