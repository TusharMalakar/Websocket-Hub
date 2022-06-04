using System;
using System.Linq;
using SignalrAPI.Common;
using SignalrAPI.Models;
using StackExchange.Redis;
using SignalrAPI.IServices;
using SignalrAPI.Backplanes;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace SignalrAPI.Hubs
{
    public class ChatHub : Hub<IHubChatClient>, IChatService
    {
        private RedisScaler redis = null;
        private AppSettings appSettings = null;
        private readonly IHubContext<ChatHub, IHubChatClient> chatHub;
        private static Dictionary<int, string> connectionIds = new Dictionary<int, string>();

        public ChatHub(IHubContext<ChatHub, IHubChatClient> _chatHub, AppSettings _appSettings, IConnectionMultiplexer redisConnection=null)
        {
            chatHub = _chatHub;
            appSettings = _appSettings;
            // When Redis-Backplan is Enabled
            if (appSettings.IsRedisEnabled)
            {
                redis = new RedisScaler(redisConnection, MessageReceivedFromRedis, "ChatTopic");
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
                
                // When Redis-Backplan is Enabled
                if (appSettings.IsRedisEnabled)
                {
                    await redis.PublishToTopic(message);
                }

                // When Redis Backplanes is not enable, broadcast Internally
                else
                {
                    await InternalBroadCast(message);
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
            // When Redis-Backplan is Enabled
            if (appSettings.IsRedisEnabled)
            {
                await redis.PublishToTopic(message);
            }
            // When Redis Backplanes is not enable, broadcast Internally
            else
            {
                await InternalBroadCast(message);
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

            // When Redis-Backplan is Enabled
            if (appSettings.IsRedisEnabled && redis != null)
            {
                await redis.UnsubscribeFromTopic();
            }

            await base.OnDisconnectedAsync(ex);
        }

        private List<string> GetConnectionIds(List<int> UserIds)
        {
            return connectionIds.Where(pair => UserIds.Contains(pair.Key)).Select(_pair => _pair.Value).ToList();
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
                    await chatHub.Clients.Client(connectionIds[message.UserId]).BroadcastMessage(JsonConvert.SerializeObject(message));
                }
                else if (message.UserIds.Any() && GetConnectionIds(message.UserIds).Any())
                {
                    await chatHub.Clients.Clients(GetConnectionIds(message.UserIds)).BroadcastMessage(JsonConvert.SerializeObject(message));
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
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
    }
}
