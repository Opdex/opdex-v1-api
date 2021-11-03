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
    public class RetrieveLiquidityAmountInQuoteQueryHandler : IRequestHandler<RetrieveLiquidityAmountInQuoteQuery, UInt256>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityAmountInQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<UInt256> Handle(RetrieveLiquidityAmountInQuoteQuery request, CancellationToken cancellationToken)
        {
            var reserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(request.Pool), cancellationToken);

            var reserveIn = request.TokenIn == Address.Cirrus ? reserves.Crs : reserves.Src;
            var reserveOut = request.TokenIn == Address.Cirrus ? reserves.Src : reserves.Crs;

            return await _mediator.Send(new CallCirrusGetLiquidityAmountInQuoteQuery(request.AmountIn, reserveIn, reserveOut, request.Router), cancellationToken);
        }
    }
}
