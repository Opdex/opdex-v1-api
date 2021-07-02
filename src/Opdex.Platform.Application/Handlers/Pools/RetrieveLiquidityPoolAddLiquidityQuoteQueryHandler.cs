using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
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
            
            var reservesCrs = reserves[0];
            var reservesSrc = reserves[1];
            var reserveIn = request.TokenIn == TokenConstants.Cirrus.Address ? reservesCrs : reservesSrc;
            var reserveOut = request.TokenIn == TokenConstants.Cirrus.Address ? reservesSrc : reservesCrs;

            return await _mediator.Send(new CallCirrusGetAddLiquidityQuoteQuery(request.AmountIn, reserveIn, reserveOut, request.Router), cancellationToken);
        }
    }
}