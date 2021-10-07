using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
{
    public class GetTokenByAddressFromFullNodeQueryHandler : IRequestHandler<GetTokenByAddressFromFullNodeQuery, TokenDto>
    {
        private readonly IMediator _mediator;

        public GetTokenByAddressFromFullNodeQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<TokenDto> Handle(GetTokenByAddressFromFullNodeQuery request, CancellationToken cancellationToken)
        {
            var block = await _mediator.Send(new RetrieveCirrusBestBlockReceiptQuery(), cancellationToken);

            try
            {
                var tokenSummary = await _mediator.Send(new CallCirrusGetStandardTokenContractSummaryQuery(request.Address, block.Height,
                                                                                                           includeBaseProperties: true,
                                                                                                           includeTotalSupply: true), cancellationToken);

                return new TokenDto
                {
                    Address = request.Address,
                    Decimals = (int)tokenSummary.Decimals.Value,
                    Name = tokenSummary.Name,
                    Sats = tokenSummary.Sats.Value,
                    Symbol = tokenSummary.Symbol,
                    TotalSupply = tokenSummary.TotalSupply.Value
                };
            }
            catch (Exception)
            {
                throw new NotFoundException("Token does not exist.");
            }
        }
    }
}
