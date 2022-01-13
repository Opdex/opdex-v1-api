using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class TokenDtoAssembler : IModelAssembler<Token, TokenDto>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    private const ulong MarketId = 0;

    public TokenDtoAssembler(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<TokenDto> Assemble(Token token)
    {
        var tokenDto = _mapper.Map<TokenDto>(token);

        var attributes = await _mediator.Send(new RetrieveTokenAttributesByTokenIdQuery(token.Id), CancellationToken.None);

        tokenDto.Attributes = attributes.Select(attribute => attribute.AttributeType);

        var summary = await _mediator.Send(new RetrieveTokenSummaryByMarketAndTokenIdQuery(MarketId, token.Id, findOrThrow: false));

        if (summary != null) tokenDto.Summary = _mapper.Map<TokenSummaryDto>(summary);

        return tokenDto;
    }
}
