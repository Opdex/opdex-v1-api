using MediatR;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools.SwapQuotes;

public class CallCirrusGetAmountOutStandardQuoteQueryHandler : IRequestHandler<CallCirrusGetAmountOutStandardQuoteQuery, UInt256>
{
    private readonly ISmartContractsModule _smartContractsModule;

    public CallCirrusGetAmountOutStandardQuoteQueryHandler(ISmartContractsModule smartContractsModule)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
    }

    public async Task<UInt256> Handle(CallCirrusGetAmountOutStandardQuoteQuery request, CancellationToken cancellationToken)
    {
        var quoteParams = new[]
        {
            new SmartContractMethodParameter(request.AmountIn),
            new SmartContractMethodParameter(request.ReserveIn),
            new SmartContractMethodParameter(request.ReserveOut)
        };

        var localCall = new LocalCallRequestDto(request.Router, request.Router, "GetAmountOut", quoteParams);
        var amountIn = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);
        return amountIn.DeserializeValue<UInt256>();
    }
}