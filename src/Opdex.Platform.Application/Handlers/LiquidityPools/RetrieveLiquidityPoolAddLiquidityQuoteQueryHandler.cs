using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.LiquidityQuotes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools
{
    public class RetrieveLiquidityPoolAddLiquidityAmountInQuoteQueryHandler : IRequestHandler<RetrieveLiquidityPoolAddLiquidityAmountInQuoteQuery, UInt256>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolAddLiquidityAmountInQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<UInt256> Handle(RetrieveLiquidityPoolAddLiquidityAmountInQuoteQuery request, CancellationToken cancellationToken)
        {
            var reserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(request.Pool), cancellationToken);

            var reservesCrs = reserves[0];
            var reservesSrc = reserves[1];
            var reserveIn = request.TokenIn == Address.Cirrus ? reservesCrs : reservesSrc;
            var reserveOut = request.TokenIn == Address.Cirrus ? reservesSrc : reservesCrs;

            return await _mediator.Send(new CallCirrusGetLiquidityPoolAddLiquidityAmountInQuoteQuery(request.AmountIn, reserveIn, reserveOut, request.Router), cancellationToken);
        }
    }
}
