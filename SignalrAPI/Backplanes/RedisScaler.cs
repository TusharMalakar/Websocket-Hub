using System;
using Newtonsoft.Json;
using SignalrAPI.Models;
using StackExchange.Redis;
using SignalrAPI.IBackplans;
using System.Threading.Tasks;

namespace SignalrAPI.Backplanes
{
    public class RedisScaler : IRedisBackplan
    {
        private string TopicName = string.Empty; // Channel
        private IConnectionMultiplexer redis = null;
        public RedisScaler(IConnectionMultiplexer _redis, Func<MessageModel, object> clientCallback, string topicName)
        {
            redis = _redis;
            TopicName = topicName;
            SubscribeToTopicAsync(clientCallback);
        }

        public void SubscribeToTopicAsync(Func<MessageModel, object> clientCallback)
        {
            redis.GetSubscriber().SubscribeAsync(TopicName, async (channel, value) =>
            {
                clientCallback(JsonConvert.DeserializeObject<MessageModel>(value));
            });
        }

        public async Task PublishToTopic(MessageModel message)
        {
            await redis.GetSubscriber().PublishAsync(TopicName, JsonConvert.SerializeObject(message));
        }


        public async Task UnsubscribeFromTopic()
        {
            await redis.GetSubscriber().UnsubscribeAllAsync();
        }   
    }
}
