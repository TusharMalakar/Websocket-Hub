using System;
using System.Linq;
using SignalrAPI.Models;
using StackExchange.Redis;
using SignalrAPI.IServices;
using SignalrAPI.Backplanes;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Alachisoft.NCache.Runtime.Caching;


namespace SignalrAPI.Hubs
{
    public class ChatHub : Hub<IHubChatClient>, IChatService
    {
        private RedisScaler redis = null;
        private NCacheScaler nCache = null;
        private AppSettings appSettings = null;
        private static Dictionary<int, string> connectionIds = new Dictionary<int, string>();

        public ChatHub(AppSettings _appSettings, IConnectionMultiplexer _redis)
        {
            appSettings = _appSettings;
            if (appSettings.IsNCacheEnabled)
            {
                nCache = new NCacheScaler(appSettings, MessageReceived);
            }
            if (appSettings.IsRedisEnabled)
            {
                redis = new RedisScaler(_redis, MessageReceivedFromRedis);
            }
        }

        // this function will be invoked from client
        public void RegisterConId(int UserId)
        {
            if (connectionIds.ContainsKey(UserId))
                connectionIds[UserId] = Context.ConnectionId;
            else
                connectionIds.Add(UserId, Context.ConnectionId);
        }
        // this function will be invoked from client
        public void UpdateMessageStatus(MessageModel message)
        {
            // update db message-status 
            switch (message.MessageStatusId)
            {
                case (int)MessageStatusEnum.Delivered:
                    break;
                case (int)MessageStatusEnum.Unread:
                    break;
                case (int)MessageStatusEnum.Read:
                    break;
            }

        }

        public async Task<ResponseModel> BroadcastToUser(MessageModel message)
        {
            ResponseModel response = new ResponseModel();
            if (message.UserId == 0)
            {
                response.Data = "UserId is required";
                return await Task.FromResult(response);
            }
            if (connectionIds.ContainsKey(message.UserId))
            {
                // await Clients.Client(connectionIds[message.UserId]).BroadcastMessage(message);
                if (appSettings.IsNCacheEnabled)
                {
                    nCache.PublishToMessageTopic(message);
                }

                if (appSettings.IsRedisEnabled)
                {
                    await redis.PublishToMessageTopic(message);
                }

                response.Success = true;
                response.Message = "Successfully broadcast";
            }
            return await Task.FromResult(response);
        }

        public async Task<ResponseModel> BroadcastToUsers(MessageModel message)
        {
            ResponseModel response = new ResponseModel();
            if (!message.UserIds.Any())
            {
                response.Data = "List of UserId is required";
                return await Task.FromResult(response);
            }
            // await Clients.Clients(GetConnectionIds(message.UserIds)).BroadcastMessage(message);
            if (appSettings.IsNCacheEnabled)
            {
                nCache.PublishToMessageTopic(message);
            }

            if (appSettings.IsRedisEnabled)
            {
                await redis.PublishToMessageTopic(message);
            }

            response.Success = true;
            response.Message = "Successfully broadcast to users";
            return await Task.FromResult(response);
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var userId = connectionIds.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId > 0)
                connectionIds.Remove(userId);

            if (appSettings.IsNCacheEnabled && nCache != null && nCache.msgSubscriber != null)
            {
                nCache.UnsubscribeToMessageTopic();
            }

            if (appSettings.IsRedisEnabled)
            {
                await redis.UnsubscribeToMessageTopic();
            }

            await base.OnDisconnectedAsync(ex);
        }

        private List<string> GetConnectionIds(List<int> UserIds)
        {
            return connectionIds.Where(pair => UserIds.Contains(pair.Key)).Select(_pair => _pair.Value).ToList();
        }

        //------------- MessageReceivedCallback --------------//
        // Private function to broadcast message within node recived from NCache
        private async void MessageReceived(object sender, MessageEventArgs args)
        {
            if (args.Message.Payload is MessageModel message)
            {
                await InternalBroadCast(message);
            }
            else
            {
                // Message failed to receive
            }
        }

        //------------- MessageReceivedCallback --------------//
        // Private function to broadcast message within node recived from Redis
        private async Task<object> MessageReceivedFromRedis(MessageModel message)
        {
            if (message != null)
            {
               return await InternalBroadCast(message);
            }
            return 0;
        }

        private async Task<object> InternalBroadCast(MessageModel message)
        {
            try
            {
                if (message.UserId > 0 && !string.IsNullOrEmpty(connectionIds[message.UserId]))
                {
                    await Clients.Client(connectionIds[message.UserId]).BroadcastMessage(message);
                }
                else if (message.UserIds.Any() && GetConnectionIds(message.UserIds).Any())
                {
                    await Clients.Clients(GetConnectionIds(message.UserIds)).BroadcastMessage(message);
                }
                return 1;
            }
            catch
            {
                return 0;
            }
        }
    }
}
