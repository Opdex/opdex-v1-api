using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools.LiquidityQuotes;

namespace Opdex.Platform.Application.Handlers.Pools
{
    public class RetrieveLiquidityPoolAddLiquidityQuoteQueryHandler : IRequestHandler<RetrieveLiquidityPoolAddLiquidityQuoteQuery, string>
    {
        private readonly IMediator _mediator;

        public RetrieveLiquidityPoolAddLiquidityQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<string> Handle(RetrieveLiquidityPoolAddLiquidityQuoteQuery request, CancellationToken cancellationToken)
        {
            var reserves = await _mediator.Send(new CallCirrusGetOpdexLiquidityPoolReservesQuery(request.Pool), cancellationToken);

            var amountIn = request.AmountCrsIn.HasValue() ? request.AmountCrsIn : request.AmountSrcIn;
            var reserveIn = request.AmountCrsIn.HasValue() ? reserves[0] : reserves[1];
            var reserveOut = request.AmountCrsIn.HasValue() ? reserves[1] : reserves[0];

            return await _mediator.Send(new CallCirrusGetAddLiquidityQuoteQuery(amountIn, reserveIn, reserveOut, request.Market), cancellationToken);
        }
    }
}