using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.MarketTokens;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.Tokens;
using System.Linq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Assemblers;

public class LiquidityPoolDtoAssembler : IModelAssembler<LiquidityPool, LiquidityPoolDto>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IModelAssembler<MiningPool, MiningPoolDto> _miningPoolAssembler;
    private readonly IModelAssembler<Token, TokenDto> _tokenAssembler;
    private readonly IModelAssembler<MarketToken, MarketTokenDto> _marketTokenAssembler;

    public LiquidityPoolDtoAssembler(IMediator mediator, IMapper mapper,
                                     IModelAssembler<MiningPool, MiningPoolDto> miningPoolAssembler,
                                     IModelAssembler<Token, TokenDto> tokenAssembler,
                                     IModelAssembler<MarketToken, MarketTokenDto> marketTokenAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _miningPoolAssembler = miningPoolAssembler ?? throw new ArgumentNullException(nameof(miningPoolAssembler));
        _tokenAssembler = tokenAssembler ?? throw new ArgumentNullException(nameof(tokenAssembler));
        _marketTokenAssembler = marketTokenAssembler ?? throw new ArgumentNullException(nameof(marketTokenAssembler));
    }

    public async Task<LiquidityPoolDto> Assemble(LiquidityPool pool)
    {
        var market = await _mediator.Send(new RetrieveMarketByIdQuery(pool.MarketId));
        var summary = await _mediator.Send(new RetrieveLiquidityPoolSummaryByLiquidityPoolIdQuery(pool.Id));
        var poolDto = _mapper.Map<LiquidityPoolDto>(pool);

        // Assemble Tokens
        poolDto.Market = market.Address;
        poolDto.CrsToken = await AssembleToken(Address.Cirrus);
        poolDto.SrcToken = await AssembleMarketToken(pool.SrcTokenId, market);
        poolDto.LpToken = await AssembleMarketToken(pool.LpTokenId, market);
        var stakingToken = market.IsStakingMarket && pool.SrcTokenId != market.StakingTokenId
            ? await AssembleMarketToken(market.StakingTokenId, market)
            : null;
        poolDto.StakingToken = stakingToken;

        var stakingEnabled = stakingToken is not null;

        // Calc rewards
        (decimal providerUsd, decimal marketUsd) = MathExtensions.VolumeBasedRewards(summary.VolumeUsd, summary.StakingWeight, stakingEnabled,
                                                                                     market.TransactionFee, market.MarketFeeEnabled);

        // Set Transaction Fee - range from 1-10 to output percentage (e.g. 1 output .1 as in .1%)
        // Math operations would * 100 to get .001
        poolDto.TransactionFee = market.TransactionFee == 0 ? 0 : Math.Round((decimal)market.TransactionFee / 10, 1);

        poolDto.Summary = new LiquidityPoolSummaryDto
        {
            Reserves = new ReservesDto
            {
                Crs = new FixedDecimal(summary.LockedCrs, TokenConstants.Cirrus.Decimals),
                Src = new FixedDecimal(summary.LockedSrc, (byte)poolDto.SrcToken.Decimals),
                Usd = summary.LiquidityUsd,
                DailyUsdChangePercent = summary.DailyLiquidityUsdChangePercent
            },
            Cost = new CostDto
            {
                CrsPerSrc = new FixedDecimal(summary.LockedCrs.Token0PerToken1(summary.LockedSrc, poolDto.SrcToken.Sats), TokenConstants.Cirrus.Decimals),
                SrcPerCrs = new FixedDecimal(summary.LockedSrc.Token0PerToken1(summary.LockedCrs, TokenConstants.Cirrus.Sats), (byte)poolDto.SrcToken.Decimals),
            },
            Volume = new VolumeDto
            {
                DailyUsd = summary.VolumeUsd
            },
            Rewards = new RewardsDto
            {
                ProviderDailyUsd = providerUsd,
                MarketDailyUsd = marketUsd
            },
            CreatedBlock = summary.CreatedBlock,
            ModifiedBlock = summary.ModifiedBlock
        };

        if (!stakingEnabled) return poolDto;

        var governance = await _mediator.Send(new RetrieveMiningGovernanceByTokenIdQuery(stakingToken.Id));
        var nominations = await _mediator.Send(new RetrieveActiveMiningGovernanceNominationsByMiningGovernanceIdQuery(governance.Id));
        var miningPool = await _mediator.Send(new RetrieveMiningPoolByLiquidityPoolIdQuery(pool.Id));

        poolDto.MiningPool = await _miningPoolAssembler.Assemble(miningPool);
        poolDto.Summary.Staking = new StakingDto
        {
            Weight = summary.StakingWeight.ToDecimal(stakingToken.Decimals),
            Usd = MathExtensions.TotalFiat(summary.StakingWeight, stakingToken.Summary.PriceUsd, stakingToken.Sats),
            DailyWeightChangePercent = summary.DailyStakingWeightChangePercent,
            Nominated = nominations.Any(nomination => nomination.LiquidityPoolId == poolDto.Id)
        };

        return poolDto;
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
