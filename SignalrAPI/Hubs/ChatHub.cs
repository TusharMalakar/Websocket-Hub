using System.Linq;
using SignalrAPI.Models;
using SignalrAPI.IServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalrAPI.Hubs
{
    public class ChatHub : Hub<IHubChatClient>, IChatService
    {
        private static Dictionary<int, string> connectionIds = new Dictionary<int, string>();
        public ChatHub(){}

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
                await Clients.Client(connectionIds[message.UserId]).BroadcastMessage(message);
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
            await Clients.Clients(GetConnectionIds(message.UserIds)).BroadcastMessage(message);
            response.Success = true;
            response.Message = "Successfully broadcast";
            return await Task.FromResult(response);
        }

        private List<string> GetConnectionIds(List<int> UserIds)
        {
            return connectionIds.Where(pair => UserIds.Contains(pair.Key)).Select(_pair => _pair.Value).ToList();
        }
    }
}
