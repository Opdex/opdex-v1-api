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
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletCreateLiquidityPoolTransactionCommandHandler
        : IRequestHandler<MakeWalletCreateLiquidityPoolTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly OpdexConfiguration _opdexConfiguration;
        
        public MakeWalletCreateLiquidityPoolTransactionCommandHandler(IMediator mediator, IOptions<OpdexConfiguration> opdexConfiguration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _opdexConfiguration = opdexConfiguration.Value ?? throw new ArgumentNullException(nameof(opdexConfiguration));
        }

        public Task<string> Handle(MakeWalletCreateLiquidityPoolTransactionCommand request, CancellationToken cancellationToken)
        {
            const string methodName = "CreatePool";
            
            var parameters = new []
            {
                request.Token.ToSmartContractParameter(SmartContractParameterType.Address)
            };
            
            var callDto = new SmartContractCallRequestDto(_opdexConfiguration.ControllerContract, request.Sender, "0", methodName, parameters);
            
            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}