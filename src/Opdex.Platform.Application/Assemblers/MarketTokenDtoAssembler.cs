using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.MarketTokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Distribution;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Wrapped;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Linq;
using System.Threading;
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

        var tokenAttributes = await _mediator.Send(new RetrieveTokenAttributesByTokenIdQuery(token.Id), CancellationToken.None);
        var attributeTypes = tokenAttributes.Select(attribute => attribute.AttributeType).ToList();
        marketTokenDto.Attributes = attributeTypes;

        var tokenWrapped = await _mediator.Send(new RetrieveTokenWrappedByTokenIdQuery(token.Id, false), CancellationToken.None);
        if (tokenWrapped is not null)
        {
            marketTokenDto.WrappedToken = _mapper.Map<WrappedTokenDetailsDto>(tokenWrapped);
        }

        var tokenIsMinedToken = attributeTypes.Contains(TokenAttributeType.Staking);
        if (tokenIsMinedToken)
        {
            var tokenDistributions = await _mediator.Send(new RetrieveDistributionsByTokenIdQuery(token.Id), CancellationToken.None);
            var tokenDistributionAssembler = new MinedTokenDistributionScheduleDtoAssembler(_mediator, token);
            marketTokenDto.Distribution = await tokenDistributionAssembler.Assemble(tokenDistributions.ToList().AsReadOnly());
        }

        var isLpt = attributeTypes.Contains(TokenAttributeType.Provisional);

        var liquidityPool = isLpt
            ? await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(token.Address))
            : await _mediator.Send(new RetrieveLiquidityPoolBySrcTokenIdAndMarketIdQuery(token.Id, token.Market.Id));

        marketTokenDto.LiquidityPool = liquidityPool.Address;

        return marketTokenDto;
    }
}
