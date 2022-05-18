using Alachisoft.NCache.Runtime.Caching;
using SignalrAPI.Models;
using StackExchange.Redis;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SignalrAPI.Backplanes
{
    public class RedisScaler //: IScalerService
    {
        private IConnectionMultiplexer redis = null;
        public RedisScaler(IConnectionMultiplexer _redis, Func<MessageModel, object> clientCallback)
        {
            redis = _redis;
            SubscribeToRedisMessageTopicAsync(clientCallback);
        }

        public void SubscribeToRedisMessageTopicAsync(Func<MessageModel, object> clientCallback)
        {
            redis.GetSubscriber().SubscribeAsync("chatTopic", async (channel, value) =>
            {
                clientCallback(JsonConvert.DeserializeObject<MessageModel>(value));
            });
        }

        public async Task PublishToMessageTopic(MessageModel message)
        {
            await redis.GetSubscriber().PublishAsync("chatTopic", JsonConvert.SerializeObject(message));
        }


        public async Task UnsubscribeToMessageTopic()
        {
            await redis.GetSubscriber().UnsubscribeAllAsync();
        }   
    }
}
