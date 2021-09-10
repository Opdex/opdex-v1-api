using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers
{
    public class MarketDtoAssembler : IModelAssembler<Market, MarketDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IModelAssembler<Token, TokenDto> _tokenAssembler;

        private const SnapshotType SnapshotType = Common.Enums.SnapshotType.Daily;

        public MarketDtoAssembler(IMediator mediator, IMapper mapper, IModelAssembler<Token, TokenDto> tokenAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _tokenAssembler = tokenAssembler ?? throw new ArgumentNullException(nameof(tokenAssembler));
        }

        public async Task<MarketDto> Assemble(Market market)
        {
            var marketDto = _mapper.Map<MarketDto>(market);

            var now = DateTime.UtcNow.ToEndOf(SnapshotType);
            var yesterday = now.Subtract(TimeSpan.FromDays(1)).ToStartOf(SnapshotType);

            // get staking token if necessary
            var stakingToken = market.IsStakingMarket
                ? await _mediator.Send(new RetrieveTokenByIdQuery(market.StakingTokenId.GetValueOrDefault()))
                : null;

            // Get yesterday and today's snapshots
            var marketSnapshots = await _mediator.Send(new RetrieveMarketSnapshotsWithFilterQuery(market.Id, yesterday, now, SnapshotType));
            var marketSnapshotList = marketSnapshots.ToList();
            var currentMarketSnapshot = marketSnapshotList.FirstOrDefault();
            var previousMarketSnapshot = marketSnapshotList.LastOrDefault();

            // If today or yesterday wasn't found, fallback to most recent.
            currentMarketSnapshot ??= await _mediator.Send(new RetrieveMarketSnapshotWithFilterQuery(market.Id, now, SnapshotType));

            // Map snapshot summary
            marketDto.Summary = _mapper.Map<MarketSnapshotDto>(currentMarketSnapshot);

            // Adjust daily change values
            marketDto.Summary.Staking.SetDailyChange(previousMarketSnapshot?.Staking?.Weight ?? UInt256.Zero);
            marketDto.Summary.SetLiquidityDailyChange(previousMarketSnapshot?.Liquidity ?? 0);

            // Assemble tokens
            marketDto.CrsToken = await AssembleToken(TokenConstants.Cirrus.Address, 0);
            marketDto.StakingToken = stakingToken == null ? null : await AssembleToken(stakingToken.Id, market.Id);

            return marketDto;
        }

        private async Task<TokenDto> AssembleToken(long tokenId, long marketId)
        {
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(tokenId));

            return await AssembleTokenExecute(token, marketId);
        }

        private async Task<TokenDto> AssembleToken(string tokenAddress, long marketId)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(tokenAddress));

            return await AssembleTokenExecute(token, marketId);
        }

        private async Task<TokenDto> AssembleTokenExecute(Token token, long marketId)
        {
            token.SetMarket(marketId);

            return await _tokenAssembler.Assemble(token);
        }
    }
}
