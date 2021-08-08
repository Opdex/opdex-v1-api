using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletDistributeTokensTransactionCommandHandler
        : IRequestHandler<MakeWalletDistributeTokensTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private const string MethodName = "Distribute";
        private const string CrsToSend = "0";

        public MakeWalletDistributeTokensTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(MakeWalletDistributeTokensTransactionCommand request, CancellationToken cancellationToken)
        {
            var callDto = new SmartContractCallRequestDto(request.Token, request.WalletName, request.WalletAddress,
                                                          request.WalletPassword, CrsToSend, MethodName);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}
