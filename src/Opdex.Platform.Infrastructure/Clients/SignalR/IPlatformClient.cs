using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.SignalR
{
    public interface IPlatformClient
    {
        Task OnTransactionBroadcast(string txHash);
        Task OnTransactionMined(string txHash);
    }
}
