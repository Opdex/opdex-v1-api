using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools.SwapQuotes
{
    public class CallCirrusGetAmountOutStandardQuoteQueryHandler : IRequestHandler<CallCirrusGetAmountOutStandardQuoteQuery, UInt256>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetAmountOutStandardQuoteQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<UInt256> Handle(CallCirrusGetAmountOutStandardQuoteQuery request, CancellationToken cancellationToken)
        {
            var quoteParams = new[] { $"12#{request.AmountIn}", $"12#{request.ReserveIn}", $"12#{request.ReserveOut}" };
            var localCall = new LocalCallRequestDto(request.Router, request.Router, "GetAmountOut", quoteParams);
            var amountIn = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
            return amountIn.DeserializeValue<UInt256>();
        }
    }
}
