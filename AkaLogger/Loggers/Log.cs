namespace AkaLogger
{
    public sealed class Log
    {
        public readonly static Users.LogUser User = new Users.LogUser();
        public readonly static Common.LogCommon Common = new Common.LogCommon();
        public readonly static Battle.LogBattle Battle = new Battle.LogBattle();
        public readonly static Matching.LogMatching Matching = new Matching.LogMatching();
        public readonly static Item.LogItem Item = new Item.LogItem();
        public readonly static Clan.LogClan Clan = new Clan.LogClan();
        public readonly static Friend.LogFriend Friend = new Friend.LogFriend();
        public readonly static Debug.LogDebug Debug = new Debug.LogDebug();
    }
}
