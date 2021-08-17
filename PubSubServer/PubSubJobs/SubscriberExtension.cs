using AkaThreading;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PubSubServer
{
    static class SubscriberExtension
    {
        internal static long AkaPublish(this ISubscriber subscriber, RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            using (var balancer = SemaphoreManager.Lock(SemaphoreType.PubsubServer2GameRedisServerBalancer))
            {
                return subscriber.Publish(channel, message, flags);
            }
        }

        internal static void AkaSubscribe(this ISubscriber subscriber, RedisChannel channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None)
        {
            using (var balancer = SemaphoreManager.Lock(SemaphoreType.PubsubServer2GameRedisServerBalancer))
            {
                subscriber.Subscribe(channel, handler, flags);
            }
        }

        internal static void AkaSubscribe(this ISubscriber subscriber, MessageHandlerType messageHandlerType, 
            NetworkSession session, RedisChannel channel, Action<ChannelMessage> handler, CommandFlags flags = CommandFlags.None)
        {
            using (var balancer = SemaphoreManager.Lock(SemaphoreType.PubsubServer2GameRedisServerBalancer))
            {
                var msgQ = subscriber.Subscribe(channel);
                msgQ.OnMessage(handler);
                AddHandlerInMessageQueue(session, messageHandlerType, msgQ);
            }
        }

        internal static void AddHandlerInMessageQueue(NetworkSession session, MessageHandlerType msgHandlerType, ChannelMessageQueue msgQ)
        {
            if (msgHandlerType == MessageHandlerType.Clan)
                session.ClanMessageHandlers.Add(msgQ);
            else
                session.ChannelHandlers.Add(msgQ);
        }

        internal static void AkaUnsubscribe(this ISubscriber subscriber, RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
        {
            using (var balancer = SemaphoreManager.Lock(SemaphoreType.PubsubServer2GameRedisServerBalancer))
            {
                subscriber.Unsubscribe(channel, handler, flags);
            }
        }

        internal static async Task AkaUnsubscribes(this ISubscriber subscriber, params RedisChannel[] channels)
        {
            using (var balancer = SemaphoreManager.Lock(SemaphoreType.PubsubServer2GameRedisServerBalancer))
            {
                await Task.WhenAll( channels.Select( channel => subscriber.UnsubscribeAsync(channel)));
            }
        }
    }
}
