namespace AkaLogger.Users
{
    public sealed class LogUser
    {
        public readonly LogJoin Join = new LogJoin();
        public readonly LogLogin Login = new LogLogin();
        public readonly LogDeck Deck = new LogDeck();
        public readonly LogUserRank UserRank = new LogUserRank();
        public readonly LogUserVirtualRank UserVirtualRank = new LogUserVirtualRank();
        public readonly LogUserLevel UserLevel = new LogUserLevel();
        public readonly LogUnitRank UnitRank = new LogUnitRank();
        public readonly LogInfusionBox InfusionBox = new LogInfusionBox();
        public readonly LogPieceLevel PieceLevel = new LogPieceLevel();
        public readonly LogEmoticon Emoticon = new LogEmoticon();
        public readonly LogSquareObject SquareObject = new LogSquareObject();
        public readonly LogNicknameChange NicknameChange = new LogNicknameChange();

    }
}
