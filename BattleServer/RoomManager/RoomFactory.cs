using AkaEnum.Battle;

namespace BattleServer
{
    public class RoomFactory
    {
        public static Room CreateRoom(BattleType battleType, string roomId, IBattleInfo battleInfo)
        {
            switch (battleType)
            {
                case BattleType.LeagueBattle:
                    return new LeagueRoom(roomId, battleInfo);
                case BattleType.LeagueBattleAi:
                    return new LeagueRoomAi(roomId, battleInfo);
                case BattleType.FriendlyBattle:
                    return new FriendlyRoom(roomId, battleInfo);
                case BattleType.PracticeBattle:
                    return new PracticeRoom(roomId, battleInfo);
                case BattleType.VirtualLeagueBattle:
                    return new VirtualLeagueRoom(roomId, battleInfo);
                case BattleType.Challenge:
                    return new ChallengeRoom(roomId, battleInfo);
                case BattleType.EventChallenge:
                    return new EventChallengeRoom(roomId, battleInfo);
                case BattleType.AkasicRecode_RogueLike:
                case BattleType.AkasicRecode_UserDeck:
                    return new RoguelikeRoom(roomId, battleInfo);
                default:
                    throw new System.Exception("Wrong BattleType");
            }
        }
    }
}
