using MediatR;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.LiquidityQuotes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools.LiquidityQuotes
{
    public class CallCirrusGetAddLiquidityQuoteQueryHandler : IRequestHandler<CallCirrusGetAddLiquidityQuoteQuery, UInt256>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetAddLiquidityQuoteQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<UInt256> Handle(CallCirrusGetAddLiquidityQuoteQuery request, CancellationToken cancellationToken)
        {
            var quoteParams = new[] { $"12#{request.AmountA}", $"12#{request.ReserveA}", $"12#{request.ReserveB}" };
            var localCall = new LocalCallRequestDto(request.Market, request.Market, "GetLiquidityQuote", quoteParams);
            var amountIn = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

            if (amountIn.ErrorMessage != null)
            {
                throw new Exception($"Invalid request: {amountIn.ErrorMessage}");
            }

            return amountIn.DeserializeValue<UInt256>();
        }
    }
}
