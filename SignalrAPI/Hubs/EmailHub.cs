using SignalrAPI.Models;
using SignalrAPI.IServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;

namespace SignalrAPI.Hubs
{
    public class EmailHub : Hub<IHubEmailClient>
    {
        private static Dictionary<int, string> connectionIds = new Dictionary<int, string>();
        public EmailHub() { }
        
        public void RegisterConId(int firmId)
        {
            if (connectionIds.ContainsKey(firmId))
                connectionIds[firmId] = Context.ConnectionId;
            else
                connectionIds.Add(firmId, Context.ConnectionId);
        }

        public async Task<ResponseModel> BroadcastEmailToUser(int userId, EmailModel email)
        {
            ResponseModel response = new ResponseModel();
            if (connectionIds.ContainsKey(userId))
            {
                await Clients.Client(connectionIds[userId]).BroadcastEmail(email);
                response.Success = true;
                response.Message = "Successfully broadcast";
            }
            return await Task.FromResult(response);
        }
    }
}
