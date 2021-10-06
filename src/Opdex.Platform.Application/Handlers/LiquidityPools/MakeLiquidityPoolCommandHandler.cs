using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.LiquidityPools;
using Opdex.Platform.Domain.Models.MarketTokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MarketTokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MarketTokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.LiquidityPools
{
    public class MakeLiquidityPoolCommandHandler : IRequestHandler<MakeLiquidityPoolCommand, ulong>
    {
        private readonly IMediator _mediator;

        public MakeLiquidityPoolCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ulong> Handle(MakeLiquidityPoolCommand request, CancellationToken cancellationToken)
        {
            if (request.LiquidityPool.Id == 0)
            {
                var marketId = request.LiquidityPool.MarketId;
                var srcTokenId = request.LiquidityPool.SrcTokenId;
                var marketToken = await _mediator.Send(new SelectMarketTokenByMarketAndTokenIdQuery(marketId, srcTokenId, findOrThrow: false));

                if (marketToken == null)
                {
                    marketToken = new MarketToken(marketId, srcTokenId, request.LiquidityPool.ModifiedBlock);

                    await _mediator.Send(new PersistMarketTokenCommand(marketToken));
                }
            }

            return await _mediator.Send(new PersistLiquidityPoolCommand(request.LiquidityPool));
        }
    }
}
