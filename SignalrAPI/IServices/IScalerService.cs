using Alachisoft.NCache.Runtime.Caching;
using SignalrAPI.Models;

namespace SignalrAPI.IServices
{
    public interface IScalerService
    {
        public void SubscribeToNCacheMessageTopic(MessageReceivedCallback callBackFunction);
        public void SubscribeToRedisMessageTopic();
        public void PublishToMessageTopic(MessageModel message);
        public void UnsubscribeToMessageTopic();
    }
}
