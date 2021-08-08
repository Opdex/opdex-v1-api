using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
{
    public class CallCirrusGetSrcTokenBalanceQueryHandler
        : IRequestHandler<CallCirrusGetSrcTokenBalanceQuery, string>
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
        public async Task<string> Handle(CallCirrusGetSrcTokenBalanceQuery request, CancellationToken cancellationToken)
        {
            var parameters = new[] {$"9#{request.Owner}"};
            var balanceRequest = new LocalCallRequestDto(request.Token, request.Owner, "GetBalance", parameters);

            var response = await _smartContractsModule.LocalCallAsync(balanceRequest, cancellationToken);

            return response.Return.ToString();
        }
    }
}