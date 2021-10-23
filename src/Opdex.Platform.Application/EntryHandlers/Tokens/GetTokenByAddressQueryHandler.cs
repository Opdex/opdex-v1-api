using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Tokens
{
    public class GetTokenByAddressQueryHandler : IRequestHandler<GetTokenByAddressQuery, TokenDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<Token, TokenDto> _tokenAssembler;

        public GetTokenByAddressQueryHandler(IMediator mediator, IModelAssembler<Token, TokenDto> tokenAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tokenAssembler = tokenAssembler ?? throw new ArgumentNullException(nameof(tokenAssembler));
        }

        public async Task<TokenDto> Handle(GetTokenByAddressQuery request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Address), cancellationToken);

            return await _tokenAssembler.Assemble(token);
        }
    }
}
