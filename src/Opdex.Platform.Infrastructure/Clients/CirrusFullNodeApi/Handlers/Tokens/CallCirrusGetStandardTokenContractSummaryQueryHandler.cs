using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
{
    public class CallCirrusGetStandardTokenContractSummaryQueryHandler
        : IRequestHandler<CallCirrusGetStandardTokenContractSummaryQuery, StandardTokenContractSummary>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetStandardTokenContractSummaryQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<StandardTokenContractSummary> Handle(CallCirrusGetStandardTokenContractSummaryQuery request, CancellationToken cancellationToken)
        {
            var summary = new StandardTokenContractSummary(request.BlockHeight);
            LocalCallRequestDto localCall;

            // We intentionally make local calls rather than getting storage values due to different token contracts using different state keys
            // to represent state data. IStandardToken256, IOpdexMinedToken, and ILiquidityPoolToken are all different, using property name instead.
            if (request.IncludeBaseProperties)
            {
                localCall = new LocalCallRequestDto(request.Token, request.Token, $"get_{StandardTokenConstants.Properties.Name}", request.BlockHeight);
                var nameResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
                var name = nameResponse.DeserializeValue<string>();

                localCall = new LocalCallRequestDto(request.Token, request.Token, $"get_{StandardTokenConstants.Properties.Symbol}", request.BlockHeight);
                var symbolResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
                var symbol = symbolResponse.DeserializeValue<string>();

                localCall = new LocalCallRequestDto(request.Token, request.Token, $"get_{StandardTokenConstants.Properties.Decimals}", request.BlockHeight);
                var decimalResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
                var decimals = decimalResponse.DeserializeValue<uint>();

                summary.SetBaseProperties(name, symbol, decimals);
            }

            if (request.IncludeTotalSupply)
            {
                localCall = new LocalCallRequestDto(request.Token, request.Token, $"get_{StandardTokenConstants.Properties.TotalSupply}", request.BlockHeight);
                var totalSupplyResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
                summary.SetTotalSupply(totalSupplyResponse.DeserializeValue<UInt256>());
            }

            return summary;
        }
    }
}
