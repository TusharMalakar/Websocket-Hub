using SignalrAPI.Models;
using System.Threading.Tasks;

namespace SignalrAPI.IServices
{
    public interface IChatService
    {
        // this function will be invoked from client
        // Register firmId in a Dictionay immediately a Firm LogIn 
        public void RegisterConId(int UserId);
        // this function will be invoked from client
        // Update message status wether it was read, unread or delivered
        public void UpdateMessageStatus(MessageModel message);
        // Broadcast to a Specific firm
        public Task<ResponseModel> BroadcastToUser(MessageModel message);
        // Broadcast to List of firm or all firms
        public Task<ResponseModel> BroadcastToUsers(MessageModel message);
    }
}
