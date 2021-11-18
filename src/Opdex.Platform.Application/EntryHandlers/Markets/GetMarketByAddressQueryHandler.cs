using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets
{
    public class GetMarketByAddressQueryHandler : IRequestHandler<GetMarketByAddressQuery, MarketDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<Market, MarketDto> _assembler;

        public GetMarketByAddressQueryHandler(IMediator mediator, IModelAssembler<Market, MarketDto> assembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }

        public async Task<MarketDto> Handle(GetMarketByAddressQuery request, CancellationToken cancellationToken)
        {
            var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.MarketAddress), cancellationToken);

            var marketDto = await _assembler.Assemble(market);

            return marketDto;
        }
    }
}
