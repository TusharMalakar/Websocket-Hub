using SignalrAPI.Models;
using System.Threading.Tasks;

namespace SignalrAPI.IServices
{
    public interface IHubEmailClient
    {
        Task BroadcastEmail(EmailModel payload);
    }
}
