using AkaEnum.Battle;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingServer
{
    public class MatchingFactory
    {
        public static MatchCommonBattle CreateMatching(ProtoTryMatching matchingInfo, int matchingLine)
        {
            switch (matchingInfo.BattleType)
            {
                case BattleType.LeagueBattle:
                    return new MatchLeagueBattle(matchingInfo, matchingInfo.GroupCode, matchingLine);
                case BattleType.FriendlyBattle when matchingInfo is ProtoTryFvFMatching:
                    return new MatchFriendlyBattle(matchingInfo as ProtoTryFvFMatching, matchingLine);
                default:
                    return null;
            }

        }
    }
}
