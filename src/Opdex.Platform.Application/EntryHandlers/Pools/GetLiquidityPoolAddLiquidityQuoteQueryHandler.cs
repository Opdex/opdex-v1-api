using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools;

namespace Opdex.Platform.Application.EntryHandlers.Pools
{
    public class GetLiquidityPoolAddLiquidityQuoteQueryHandler : IRequestHandler<GetLiquidityPoolAddLiquidityQuoteQuery, string>
    {
        private readonly IMediator _mediator;

        public GetLiquidityPoolAddLiquidityQuoteQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<string> Handle(GetLiquidityPoolAddLiquidityQuoteQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new RetrieveLiquidityPoolAddLiquidityQuoteQuery(request.AmountCrsIn, request.AmountSrcIn, request.Pool, request.Market), cancellationToken);
        }
    }
}