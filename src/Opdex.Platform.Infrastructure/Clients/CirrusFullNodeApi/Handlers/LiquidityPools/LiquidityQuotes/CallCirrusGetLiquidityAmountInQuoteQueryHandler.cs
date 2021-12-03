using MediatR;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.LiquidityQuotes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools.LiquidityQuotes;

public class CallCirrusGetLiquidityAmountInQuoteQueryHandler : IRequestHandler<CallCirrusGetLiquidityAmountInQuoteQuery, UInt256>
{
    private readonly ISmartContractsModule _smartContractsModule;

    public CallCirrusGetLiquidityAmountInQuoteQueryHandler(ISmartContractsModule smartContractsModule)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
    }

    public async Task<UInt256> Handle(CallCirrusGetLiquidityAmountInQuoteQuery request, CancellationToken cancellationToken)
    {
        var quoteParams = new[]
        {
            new SmartContractMethodParameter(request.AmountA),
            new SmartContractMethodParameter(request.ReserveA),
            new SmartContractMethodParameter(request.ReserveB),
        };

        var localCall = new LocalCallRequestDto(request.Router, "GetLiquidityQuote", quoteParams);
        var amountIn = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

        if (amountIn.ErrorMessage != null)
        {
            throw new Exception($"Invalid request: {amountIn.ErrorMessage.Value}");
        }

        return amountIn.DeserializeValue<UInt256>();
    }
}