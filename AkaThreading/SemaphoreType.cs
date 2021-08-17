
namespace AkaThreading
{
    public enum SemaphoreType
    {
        MatchServer2MatchingRedisServerBalancer,
        MatchServer2GameServerRequestBalancer,
        BattleServer2GameRedisServerBalancer,
        BattleServer2GameServerBalancer,
        GameServer2GameRedisServerBalancer,
        PubsubServer2GameRedisServerBalancer,
        PubsubServer2ClientSocketBalancer
    }
}
