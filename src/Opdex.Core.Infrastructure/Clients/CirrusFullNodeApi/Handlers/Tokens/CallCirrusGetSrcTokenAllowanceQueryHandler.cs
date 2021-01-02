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
    public class CallCirrusGetSrcTokenAllowanceQueryHandler 
        : IRequestHandler<CallCirrusGetSrcTokenAllowanceQuery, decimal>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetSrcTokenAllowanceQueryHandler> _logger;
        
        public CallCirrusGetSrcTokenAllowanceQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetSrcTokenAllowanceQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Todo: try catch requests
        public async Task<decimal> Handle(CallCirrusGetSrcTokenAllowanceQuery request, CancellationToken cancellationToken)
        {
            var parameters = new[] {$"9#{request.Owner}", $"9#{request.Spender}"};
            var allowanceCall = new LocalCallRequestDto(request.Token, request.Spender, "Allowance", parameters);

            var allowance = await _smartContractsModule.LocalCallAsync(allowanceCall, cancellationToken);
            ulong.TryParse(allowance.Return.ToString(), out var orderBookAllowance);

            return orderBookAllowance.ToDecimal(request.Decimals);
        }
    }
}