using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
{
    public class CallCirrusGetSrcTokenBalanceQueryHandler
        : IRequestHandler<CallCirrusGetSrcTokenBalanceQuery, decimal>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetSrcTokenBalanceQueryHandler> _logger;
        
        public CallCirrusGetSrcTokenBalanceQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetSrcTokenBalanceQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Todo: try catch requests
        public async Task<decimal> Handle(CallCirrusGetSrcTokenBalanceQuery request, CancellationToken cancellationToken)
        {
            var parameters = new[] {$"9#{request.Owner}"};
            var balanceRequest = new LocalCallRequestDto(request.Token, request.Owner, "GetBalance", parameters);

            var balance = await _smartContractsModule.LocalCallAsync(balanceRequest, cancellationToken);
            ulong.TryParse(balance.Return.ToString(), out var srcBalance);

            return srcBalance.ToDecimal(request.Decimals);
        }
    }
}