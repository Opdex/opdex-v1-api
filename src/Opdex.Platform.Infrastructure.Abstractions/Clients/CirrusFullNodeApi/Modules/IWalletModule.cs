using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules
{
    public interface IWalletModule
    {
        /// <summary>
        /// Verifies a message was signed using a private key of a wallet address.
        /// </summary>
        /// <param name="request">The verify message request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>true if the signature could be verified; otherwise, false.</returns>
         Task<bool> VerifyMessage(VerifyMessageRequestDto request, CancellationToken cancellationToken);
    }
}