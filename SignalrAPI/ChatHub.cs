using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalrAPI
{
    public class ChatHub : Hub<HubClient>
    {
        public void BroadcastMessage(string message)
        {
            Clients.All.BroadcastMessage(message);
        }
    }
    public interface HubClient
    {
        Task BroadcastMessage(string msg);
    }
}
