using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
{
    public class GetMarketTokenByMarketAndTokenAddressQueryHandler : IRequestHandler<GetMarketTokenByMarketAndTokenAddressQuery, MarketTokenDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<MarketToken, MarketTokenDto> _tokenAssembler;

        public GetMarketTokenByMarketAndTokenAddressQueryHandler(IMediator mediator, IModelAssembler<MarketToken, MarketTokenDto> tokenAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tokenAssembler = tokenAssembler ?? throw new ArgumentNullException(nameof(tokenAssembler));
        }

        public async Task<MarketTokenDto> Handle(GetMarketTokenByMarketAndTokenAddressQuery request, CancellationToken cancellationToken)
        {
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market), cancellationToken);
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token), cancellationToken);

            var marketToken = new MarketToken(market, token);

            return await _tokenAssembler.Assemble(marketToken);
        }
    }
}
