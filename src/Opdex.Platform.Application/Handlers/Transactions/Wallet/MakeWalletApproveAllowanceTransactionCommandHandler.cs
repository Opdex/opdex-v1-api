using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletApproveAllowanceTransactionCommandHandler 
        : IRequestHandler<MakeWalletApproveAllowanceTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private const string MethodName = "Approve";
        private const string CrsToSend = "0";
        public MakeWalletApproveAllowanceTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(MakeWalletApproveAllowanceTransactionCommand request, CancellationToken cancellationToken)
        {
            var currentAllowanceRequest = new CallCirrusGetSrcTokenAllowanceQuery(request.Token, request.WalletAddress, request.Spender);
            var currentAllowance = await _mediator.Send(currentAllowanceRequest, cancellationToken);

            var parameters = new []
            {
                request.Spender.ToSmartContractParameter(SmartContractParameterType.Address),
                currentAllowance.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.Amount.ToSmartContractParameter(SmartContractParameterType.UInt256)
            };
            
            var callDto = new SmartContractCallRequestDto(request.Token, request.WalletName, request.WalletAddress, 
                request.WalletPassword, CrsToSend, MethodName, parameters);
            
            return await _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}