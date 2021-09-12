using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools.SwapQuotes
{
    public class CallCirrusGetAmountOutMultiHopQuoteQueryHandler : IRequestHandler<CallCirrusGetAmountOutMultiHopQuoteQuery, UInt256>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetAmountOutMultiHopQuoteQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<UInt256> Handle(CallCirrusGetAmountOutMultiHopQuoteQuery request, CancellationToken cancellationToken)
        {
            var quoteParams = new[] {
                request.TokenInAmount.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenInReserveCrs.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenInReserveSrc.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenOutReserveCrs.ToSmartContractParameter(SmartContractParameterType.UInt256),
                request.TokenOutReserveSrc.ToSmartContractParameter(SmartContractParameterType.UInt256)
            };

            var localCall = new LocalCallRequestDto(request.Router, request.Router, "GetAmountOut", quoteParams);

            var amountIn = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

            if (amountIn.ErrorMessage != null)
            {
                throw new Exception($"Invalid request: {amountIn.ErrorMessage}");
            }

            return amountIn.DeserializeValue<UInt256>();
        }
    }
}
