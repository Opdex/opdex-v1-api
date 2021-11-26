using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Auth
{
    public class CallCirrusVerifyMessageQueryHandler : IRequestHandler<CallCirrusVerifyMessageQuery, bool>
    {
        private readonly IWalletModule _walletModule;

        public CallCirrusVerifyMessageQueryHandler(IWalletModule walletModule)
        {
            _walletModule = walletModule ?? throw new ArgumentNullException(nameof(walletModule));
        }

        public async Task<bool> Handle(CallCirrusVerifyMessageQuery request, CancellationToken cancellationToken)
        {
            return await _walletModule.VerifyMessage(new VerifyMessageRequestDto(request.Message, request.Signer, request.Signature), cancellationToken);
        }
    }
}