using SignalrAPI.Models;
using System.Threading.Tasks;

namespace SignalrAPI.IServices
{
    public interface IHubChatClient
    {
        Task BroadcastMessage(string payload);
    }
}
