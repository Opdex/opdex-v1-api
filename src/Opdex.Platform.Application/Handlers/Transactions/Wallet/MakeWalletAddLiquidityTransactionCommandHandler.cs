using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Opdex.Platform.Common;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletAddLiquidityTransactionCommandHandler : IRequestHandler<MakeWalletAddLiquidityTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly OpdexConfiguration _opdexConfiguration;
        
        public MakeWalletAddLiquidityTransactionCommandHandler(IMediator mediator, IOptions<OpdexConfiguration> opdexConfiguration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _opdexConfiguration = opdexConfiguration.Value ?? throw new ArgumentNullException(nameof(opdexConfiguration));
        }

        public Task<string> Handle(MakeWalletAddLiquidityTransactionCommand request, CancellationToken cancellationToken)
        {
            const string methodName = "AddLiquidity";
            
            // Todo: Convert to decimals
            var crsToSend = Math.Round((decimal)request.AmountCrsDesired / 100_000_000, 8).ToString();
            
            var parameters = new List<string>
            {
                $"9#{request.Token}", // token
                $"12#{request.AmountSrcDesired}", // srcDesired
                $"7#{request.AmountCrsMin}", // crsMin
                $"12#{request.AmountSrcMin}", // srcMin
                $"9#{request.To}", // to
                "7#0", // deadline
            };
            
            var callDto = new SmartContractCallRequestDto(_opdexConfiguration.ControllerContract, request.To, crsToSend, methodName, parameters.ToArray());
            
            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}