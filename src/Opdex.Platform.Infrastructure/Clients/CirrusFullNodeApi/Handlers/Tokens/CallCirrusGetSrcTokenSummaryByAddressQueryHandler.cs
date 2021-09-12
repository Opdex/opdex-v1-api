using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
{
    public class CallCirrusGetSrcTokenSummaryByAddressQueryHandler
        : IRequestHandler<CallCirrusGetSrcTokenSummaryByAddressQuery, TokenContractSummary>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetSrcTokenSummaryByAddressQueryHandler> _logger;

        public CallCirrusGetSrcTokenSummaryByAddressQueryHandler(ISmartContractsModule smartContractsModule,
            ILogger<CallCirrusGetSrcTokenSummaryByAddressQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Todo: map the result dto to a domain model
        // Todo: TryCatch the module requests
        public async Task<TokenContractSummary> Handle(CallCirrusGetSrcTokenSummaryByAddressQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_Name", new string[0]);
            var nameResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var name = nameResponse.DeserializeValue<string>();
            if (!name.HasValue()) return null;

            localCall.MethodName = "get_Symbol";
            var symbolResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var symbol = symbolResponse.DeserializeValue<string>();
            if (!symbol.HasValue()) return null;

            localCall.MethodName = "get_Decimals";
            var decimalResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var decimals = decimalResponse.DeserializeValue<uint>();

            localCall.MethodName = "get_TotalSupply";
            var totalSupplyResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            var totalSupply = totalSupplyResponse.DeserializeValue<UInt256>();

            var isLpt = symbol == TokenConstants.LiquidityPoolToken.Symbol && name == TokenConstants.LiquidityPoolToken.Name;

            // Todo: Handle 1 - createdBlock param.
            // Shouldn't need it in this query, maybe domain model should not require it but then again...it kinda should
            return new TokenContractSummary(request.Address, name, symbol, decimals, totalSupply);
        }
    }
}
