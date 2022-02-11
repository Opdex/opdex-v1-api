using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Models.MarketTokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class MarketDtoAssembler : IModelAssembler<Market, MarketDto>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IModelAssembler<Token, TokenDto> _tokenAssembler;
    private readonly IModelAssembler<MarketToken, MarketTokenDto> _marketTokenAssembler;

    public MarketDtoAssembler(IMediator mediator, IMapper mapper, IModelAssembler<Token, TokenDto> tokenAssembler,
                              IModelAssembler<MarketToken, MarketTokenDto> marketTokenAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _tokenAssembler = tokenAssembler ?? throw new ArgumentNullException(nameof(tokenAssembler));
        _marketTokenAssembler = marketTokenAssembler ?? throw new ArgumentNullException(nameof(marketTokenAssembler));
    }

    public async Task<MarketDto> Assemble(Market market)
    {
        var marketDto = _mapper.Map<MarketDto>(market);

        var summary = await _mediator.Send(new RetrieveMarketSummaryByMarketIdQuery(market.Id, findOrThrow: false), CancellationToken.None);
        if (summary is not null)
        {
            marketDto.Summary = _mapper.Map<MarketSummaryDto>(summary);
            // remove staking summary from standard markets
            if (!market.IsStakingMarket) marketDto.Summary.Staking = null;
        }

        marketDto.CrsToken = await AssembleToken(Address.Cirrus);

        if (!market.IsStakingMarket)
        {
            return marketDto;
        }

        var stakingToken = await _mediator.Send(new RetrieveTokenByIdQuery(market.StakingTokenId), CancellationToken.None);
        marketDto.StakingToken = await AssembleMarketToken(stakingToken.Id, market);

        return marketDto;
    }

    private async Task<MarketTokenDto> AssembleMarketToken(ulong tokenId, Market market)
    {
        var token = await _mediator.Send(new RetrieveTokenByIdQuery(tokenId), CancellationToken.None);
        return await _marketTokenAssembler.Assemble(new MarketToken(market, token));
    }

    private async Task<TokenDto> AssembleToken(Address tokenAddress)
    {
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(tokenAddress), CancellationToken.None);
        return await _tokenAssembler.Assemble(token);
    }
}
