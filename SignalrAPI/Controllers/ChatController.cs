using SignalrAPI.Hubs;
using SignalrAPI.Models;
using SignalrAPI.Common;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace SignalrAPI.Controllers
{
    [Authenticate]
    [ApiController]
    [Route("chat/")]
    public class ChatController : Controller
    {
        private ChatHub chatHub;

        public ChatController(AppSettings _appSettings, IConnectionMultiplexer _redis)
        {
            chatHub = new ChatHub(_appSettings, _redis);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ResponseModel> Broadcast([FromBody] MessageModel message)
        {
            if (!ModelState.IsValid)
                return await Task.FromResult(new ResponseModel() { Data = "Invalid request" });

            if (message.UserId > 0)
                return await chatHub.BroadcastToUser(message);
            return await chatHub.BroadcastToUsers(message);
        }
    }
}
