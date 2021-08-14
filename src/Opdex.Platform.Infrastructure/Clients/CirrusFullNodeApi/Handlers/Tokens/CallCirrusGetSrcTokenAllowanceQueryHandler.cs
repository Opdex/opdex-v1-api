using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
{
    public class CallCirrusGetSrcTokenAllowanceQueryHandler
        : IRequestHandler<CallCirrusGetSrcTokenAllowanceQuery, UInt256>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetSrcTokenAllowanceQueryHandler> _logger;

        public CallCirrusGetSrcTokenAllowanceQueryHandler(ISmartContractsModule smartContractsModule,
            ILogger<CallCirrusGetSrcTokenAllowanceQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UInt256> Handle(CallCirrusGetSrcTokenAllowanceQuery request, CancellationToken cancellationToken)
        {
            var parameters = new[] {$"9#{request.Owner}", $"9#{request.Spender}"};

            // Todo: using toString() temporarily, adjust all flows to use Address types
            var allowanceCall = new LocalCallRequestDto(request.Token.ToString(), request.Spender.ToString(), "Allowance", parameters);

            try
            {
                var allowanceResponse = await _smartContractsModule.LocalCallAsync(allowanceCall, cancellationToken);
                return new UInt256(allowanceResponse.DeserializeValue<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting allowance for {request.Owner}");
            }

            return UInt256.Zero;
        }
    }
}
