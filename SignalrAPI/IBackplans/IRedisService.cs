using System;
using SignalrAPI.Models;
using System.Threading.Tasks;

namespace SignalrAPI.IBackplans
{
    public interface IRedisBackplan
    {
        public void SubscribeToTopicAsync(Func<MessageModel, object> clientCallback);
        public Task PublishToTopic(MessageModel message);
        public Task UnsubscribeFromTopic();
    }
}
