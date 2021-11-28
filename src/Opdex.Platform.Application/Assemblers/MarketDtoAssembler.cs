using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
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
        private readonly IModelAssembler<MarketToken, MarketTokenDto> _marketTokenAssembler;

        private const SnapshotType SnapshotType = Common.Enums.SnapshotType.Daily;

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

            var now = DateTime.UtcNow.ToEndOf(SnapshotType);
            var yesterday = now.Subtract(TimeSpan.FromDays(1)).ToStartOf(SnapshotType);

            // get staking token if necessary
            var stakingToken = market.IsStakingMarket
                ? await _mediator.Send(new RetrieveTokenByIdQuery(market.StakingTokenId))
                : null;

            // Get yesterday and today's snapshots
            var cursor = new SnapshotCursor(Interval.OneDay, yesterday, now, SortDirectionType.DESC, 2, PagingDirection.Forward, default);
            var marketSnapshots = await _mediator.Send(new RetrieveMarketSnapshotsWithFilterQuery(market.Id, cursor));
            var marketSnapshotList = marketSnapshots.ToList();
            var currentMarketSnapshot = marketSnapshotList.FirstOrDefault();
            var previousMarketSnapshot = marketSnapshotList.LastOrDefault();

            // If today or yesterday wasn't found, fallback to most recent.
            currentMarketSnapshot ??= await _mediator.Send(new RetrieveMarketSnapshotWithFilterQuery(market.Id, now, SnapshotType));

            // Map snapshot summary
            marketDto.Summary = new MarketSummaryDto
            {
                LiquidityUsd = currentMarketSnapshot.LiquidityUsd.Close,
                DailyLiquidityUsdChangePercent = MathExtensions.PercentChange(currentMarketSnapshot.LiquidityUsd.Close,
                                                                              previousMarketSnapshot?.LiquidityUsd?.Close ?? 0m),
                VolumeUsd = currentMarketSnapshot.VolumeUsd,
                StakingWeight = currentMarketSnapshot.Staking.Weight.Close.ToDecimal(TokenConstants.Opdex.Decimals),
                DailyStakingWeightChangePercent = MathExtensions.PercentChange(currentMarketSnapshot.Staking.Weight.Close,
                                                                               previousMarketSnapshot?.Staking?.Weight?.Close ?? UInt256.Zero,
                                                                               TokenConstants.Opdex.Sats),
                StakingUsd = currentMarketSnapshot.Staking.Usd.Close,
                DailyStakingUsdChangePercent = MathExtensions.PercentChange(currentMarketSnapshot.Staking.Usd.Close,
                                                                            previousMarketSnapshot?.Staking?.Usd?.Close ?? 0m),
                Rewards = new RewardsDto
                {
                    ProviderDailyUsd = currentMarketSnapshot.Rewards.ProviderUsd,
                    MarketDailyUsd = currentMarketSnapshot.Rewards.MarketUsd
                }
            };

            // Assemble tokens
            marketDto.CrsToken = await AssembleToken(Address.Cirrus);
            marketDto.StakingToken = stakingToken == null ? null : await AssembleMarketToken(stakingToken.Id, market);

            return marketDto;
        }

        private async Task<MarketTokenDto> AssembleMarketToken(ulong tokenId, Market market)
        {
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(tokenId));
            return await _marketTokenAssembler.Assemble(new MarketToken(market, token));
        }

        private async Task<TokenDto> AssembleToken(Address tokenAddress)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(tokenAddress));
            return await _tokenAssembler.Assemble(token);
        }
    }
}
