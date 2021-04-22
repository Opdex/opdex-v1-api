using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Opdex.Platform.Common;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletApproveAllowanceTransactionCommandHandler 
        : IRequestHandler<MakeWalletApproveAllowanceTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        
        public MakeWalletApproveAllowanceTransactionCommandHandler(IMediator mediator, IOptions<OpdexConfiguration> opdexConfiguration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(MakeWalletApproveAllowanceTransactionCommand request, CancellationToken cancellationToken)
        {
            const string methodName = "Approve";
            const string amountToSend = "0";

            var currentAllowanceRequest = new CallCirrusGetSrcTokenAllowanceQuery(request.Token, request.Owner, request.Spender);
            var currentAllowance = await _mediator.Send(currentAllowanceRequest, cancellationToken);
            
            var parameters = new List<string>
            {
                $"9#{request.Spender}", // spender
                $"12#{currentAllowance}", // currentAmount
                $"12#{request.Amount}" // amount
            }.ToArray();
            
            var callDto = new SmartContractCallRequestDto(request.Token, request.Owner, amountToSend, methodName, parameters);
            
            return await _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}