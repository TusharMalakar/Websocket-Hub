using System;
using SignalrAPI.Models;
using SignalrAPI.IServices;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Exceptions;

namespace SignalrAPI.Backplanes
{
    public class NCacheScaler : IScalerService
    {
        private ICache cache = null;
        private AppSettings appSettings = null;
        public ITopicSubscription msgSubscriber = null;

        public NCacheScaler(AppSettings _appSettings, MessageReceivedCallback callBackFunction)
        {
            appSettings = _appSettings;
            cache = CacheManager.GetCache(appSettings.NCacheName);
            SubscribeToNCacheMessageTopic(callBackFunction);
        }

        public void SubscribeToNCacheMessageTopic(MessageReceivedCallback callBackFunction)
        {
            try
            {
                ITopic msgTopic = GetMessageTopic();
                if (msgTopic != null)
                {
                    // Create and register subscribers for Order topic
                    // MessageReceived callback is specified
                    // DeliveryMode is set to async
                    msgSubscriber = msgTopic.CreateSubscription(callBackFunction);

                    // Topics can also be unsubscribed
                    // msgSubscriber.UnSubscribe();
                }
                else
                {
                    // No topic exists
                }
            }
            catch (OperationFailedException ex)
            {
                if (ex.ErrorCode == NCacheErrorCodes.TOPIC_DISPOSED)
                {
                    // Specified topic has been disposed
                }
                if (ex.ErrorCode == NCacheErrorCodes.SUBSCRIPTION_EXISTS)
                {
                    // Active subscription with this name already exists
                    // Specific to Exclusive subscription
                }
                if (ex.ErrorCode == NCacheErrorCodes.DEFAULT_TOPICS)
                {
                    // Operation cannot be performed on default topics,
                    // Get user-defined topics instead
                }
                else
                {
                    // Exception can occur due to:
                    // Connection Failures
                    // Operation Timeout
                    // Operation performed during state transfer
                }
            }
            catch (Exception ex)
            {
                // Any other generic exception like ArgumentNullException or ArgumentException
                // Topic name is null/empty
            }
        }


        public void PublishToMessageTopic(MessageModel message)
        {

            try
            {
                ITopic msgTopic = GetMessageTopic();
                if (msgTopic != null)
                {
                    // Create the new message with the object order
                    var orderMessage = new Message(message);
                    // Set the expiration time of the message
                    orderMessage.ExpirationTime = TimeSpan.FromSeconds(5000);
                    // Publish the order with delivery option set as all
                    // and register message delivery failure
                    msgTopic.Publish(orderMessage, DeliveryOption.All, true);
                }
                else
                {
                    // No topic exists
                }

            }
            catch (OperationFailedException ex)
            {
                if (ex.ErrorCode == NCacheErrorCodes.MESSAGE_ID_ALREADY_EXISTS)
                {
                    // Message ID already exists, specify a new ID
                }
                if (ex.ErrorCode == NCacheErrorCodes.TOPIC_DISPOSED)
                {
                    // Specified topic has been disposed
                }
                if (ex.ErrorCode == NCacheErrorCodes.PATTERN_BASED_PUBLISHING_NOT_ALLOWED)
                {
                    // Message publishing on pattern based topic is not allowed
                    // Get non-pattern based topic
                }
                else
                {
                    // Exception can occur due to:
                    // Connection Failures
                    // Operation Timeout
                    // Operation performed during state transfer
                }
            }
            catch (Exception ex)
            {
                // Any other generic exception like ArgumentNullException or ArgumentException
                // Topic name is null/empty
            }
        }


        public void UnsubscribeToMessageTopic()
        {
            if (msgSubscriber != null)
            {
                msgSubscriber.UnSubscribe();
            }
        }

        private ITopic GetMessageTopic()
        {
            ITopic msgTopic = cache.MessagingService.GetTopic("chatTopic");
            if (msgTopic != null)
                return msgTopic;
            return cache.MessagingService.CreateTopic("chatTopic");
        }

        // No Implementation Required as we are using NCache
        public void SubscribeToRedisMessageTopic()
        {
            throw new NotImplementedException();
        }
    }
}
