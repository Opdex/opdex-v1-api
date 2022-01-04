using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class MarketTokenDtoAssembler : IModelAssembler<MarketToken, MarketTokenDto>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public MarketTokenDtoAssembler(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<MarketTokenDto> Assemble(MarketToken token)
    {
        var marketTokenDto = _mapper.Map<MarketTokenDto>(token);

        if (marketTokenDto.Summary is null)
        {
            var summary = await _mediator.Send(new RetrieveTokenSummaryByMarketAndTokenIdQuery(token.Market.Id, token.Id, findOrThrow: false));
            if (summary is not null) marketTokenDto.Summary = _mapper.Map<TokenSummaryDto>(summary);
        }

        var liquidityPool = token.IsLpt
            ? await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(token.Address))
            : await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(token.Id, token.Market.Id));

        marketTokenDto.LiquidityPool = liquidityPool.Address;

        return marketTokenDto;
    }
}
