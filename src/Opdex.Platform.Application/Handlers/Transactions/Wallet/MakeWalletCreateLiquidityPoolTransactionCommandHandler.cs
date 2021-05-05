using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletCreateLiquidityPoolTransactionCommandHandler
        : IRequestHandler<MakeWalletCreateLiquidityPoolTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        
        public MakeWalletCreateLiquidityPoolTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(MakeWalletCreateLiquidityPoolTransactionCommand request, CancellationToken cancellationToken)
        {
            var parameters = new []
            {
                request.Token.ToSmartContractParameter(SmartContractParameterType.Address)
            };
            
            var callDto = new SmartContractCallRequestDto(request.Market, request.Sender, "0", "CreatePool", parameters);
            
            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}