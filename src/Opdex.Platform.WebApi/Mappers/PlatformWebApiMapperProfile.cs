using AutoMapper;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Application.Abstractions.Models.OHLC;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Governances;
using Opdex.Platform.WebApi.Models.Responses.Markets;
using Opdex.Platform.WebApi.Models.Responses.OHLC;
using Opdex.Platform.WebApi.Models.Responses.Pools;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Vaults;
using Opdex.Platform.WebApi.Models.Responses.Wallet;

namespace Opdex.Platform.WebApi.Mappers
{
    public class PlatformWebApiMapperProfile : Profile
    {
        public PlatformWebApiMapperProfile()
        {
            CreateMap<TokenDto, TokenResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Decimals))
                .ForMember(dest => dest.Sats, opt => opt.MapFrom(src => src.Sats))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply.InsertDecimal(src.Decimals)))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TokenSnapshotDto, TokenSummaryResponseModel>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.DailyPriceChange, opt => opt.MapFrom(src => src.DailyPriceChange))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TokenSnapshotDto, TokenSnapshotResponseModel>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolDto, LiquidityPoolResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.Summary.TransactionCount))
                .ForMember(dest => dest.Reserves, opt => opt.MapFrom(src => MapReserves(src.Summary.Reserves, src.SrcToken.Decimals)))
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => MapRewards(src.Summary.Rewards)))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => MapStaking(src.Summary.Staking, src.StakingEnabled)))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => MapVolume(src.Summary.Volume, src.SrcToken.Decimals)))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => MapCost(src.Summary.Cost, src.SrcToken.Decimals)))
                .ForMember(dest => dest.Mining, opt => opt.MapFrom(src => src.MiningPool))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolDto, LiquidityPoolTokenGroupResponseModel>()
                .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.CrsToken))
                .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.SrcToken))
                .ForMember(dest => dest.Lp, opt => opt.MapFrom(src => src.LpToken))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.StakingToken))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolSnapshotDto, LiquidityPoolSummaryResponseModel>()
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.TransactionCount))
                .ForMember(dest => dest.Reserves, opt => opt.MapFrom(src => MapReserves(src.Reserves, src.SrcTokenDecimals)))
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => MapRewards(src.Rewards)))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => MapStaking(src.Staking, src.Staking != null)))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => MapVolume(src.Volume, src.SrcTokenDecimals)))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => MapCost(src.Cost, src.SrcTokenDecimals)))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolSnapshotDto, LiquidityPoolSnapshotResponseModel>()
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.TransactionCount))
                .ForMember(dest => dest.Reserves, opt => opt.MapFrom(src => MapReserves(src.Reserves, src.SrcTokenDecimals)))
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => MapRewards(src.Rewards)))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => MapStaking(src.Staking, src.Staking != null)))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => MapVolume(src.Volume, src.SrcTokenDecimals)))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => MapCost(src.Cost, src.SrcTokenDecimals)))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MiningPoolDto, MiningPoolResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.MiningPeriodEndBlock, opt => opt.MapFrom(src => src.MiningPeriodEndBlock))
                .ForMember(dest => dest.RewardPerBlock, opt => opt.MapFrom(src => src.RewardPerBlock.InsertDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.RewardPerLpt, opt => opt.MapFrom(src => src.RewardPerLpt.InsertDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.TokensMining, opt => opt.MapFrom(src => src.TokensMining.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals)))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OhlcDecimalDto, OhlcDecimalResponseModel>()
                .ForMember(dest => dest.Open, opt => opt.MapFrom(src => src.Open))
                .ForMember(dest => dest.High, opt => opt.MapFrom(src => src.High))
                .ForMember(dest => dest.Low, opt => opt.MapFrom(src => src.Low))
                .ForMember(dest => dest.Close, opt => opt.MapFrom(src => src.Close))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketSnapshotDto, MarketSnapshotResponseModel>()
                .ForMember(dest => dest.Liquidity, opt => opt.MapFrom(src => src.Liquidity))
                .ForMember(dest => dest.LiquidityDailyChange, opt => opt.MapFrom(src => src.LiquidityDailyChange))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => MapStaking(src.Staking, src.Staking != null)))
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => MapRewards(src.Rewards)))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketDto, MarketResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.StakingToken, opt => opt.MapFrom(src => src.StakingToken))
                .ForMember(dest => dest.CrsToken, opt => opt.MapFrom(src => src.CrsToken))
                .ForMember(dest => dest.AuthPoolCreators, opt => opt.MapFrom(src => src.AuthPoolCreators))
                .ForMember(dest => dest.AuthProviders, opt => opt.MapFrom(src => src.AuthProviders))
                .ForMember(dest => dest.AuthTraders, opt => opt.MapFrom(src => src.AuthTraders))
                .ForMember(dest => dest.MarketFeeEnabled, opt => opt.MapFrom(src => src.MarketFeeEnabled))
                .ForMember(dest => dest.TransactionFee, opt => opt.MapFrom(src => src.TransactionFee))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<AddressAllowanceDto, ApprovedAllowanceResponseModel>()
                .ForMember(dest => dest.Allowance, opt => opt.MapFrom(src => src.Allowance))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Spender, opt => opt.MapFrom(src => src.Spender))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token));

            CreateMap<AddressBalanceDto, AddressBalanceResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token));

            CreateMap<StakingPositionDto, StakingPositionResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.StakingToken, opt => opt.MapFrom(src => src.StakingToken))
                .ForMember(dest => dest.LiquidityPool, opt => opt.MapFrom(src => src.LiquidityPool));

            CreateMap<MiningGovernanceDto, MiningGovernanceResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.MinedToken, opt => opt.MapFrom(src => src.MinedToken))
                .ForMember(dest => dest.PeriodEndBlock, opt => opt.MapFrom(src => src.PeriodEndBlock))
                .ForMember(dest => dest.PeriodRemainingBlocks, opt => opt.MapFrom(src => src.PeriodRemainingBlocks))
                .ForMember(dest => dest.PeriodBlockDuration, opt => opt.MapFrom(src => src.PeriodBlockDuration))
                .ForMember(dest => dest.MiningPoolRewardPerPeriod, opt => opt.MapFrom(src => src.MiningPoolRewardPerPeriod.InsertDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.PeriodsUntilRewardReset, opt => opt.MapFrom(src => src.PeriodsUntilRewardReset))
                .ForMember(dest => dest.TotalRewardsPerPeriod, opt => opt.MapFrom(src => src.TotalRewardsPerPeriod.InsertDecimal(TokenConstants.Opdex.Decimals)))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<VaultDto, VaultResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Genesis, opt => opt.MapFrom(src => src.Genesis))
                .ForMember(dest => dest.TokensLocked, opt => opt.MapFrom(src => src.TokensLocked))
                .ForMember(dest => dest.TokensUnassigned, opt => opt.MapFrom(src => src.TokensUnassigned))
                .ForMember(dest => dest.LockedToken, opt => opt.MapFrom(src => src.LockedToken))
                .ForAllOtherMembers(opt => opt.Ignore());
        }

        private static ReservesResponseModel MapReserves(ReservesDto reservesDto, int srcTokenDecimals)
        {
            if (reservesDto == null) return null;

            return new ReservesResponseModel
            {
                Crs = reservesDto.Crs.ToString().InsertDecimal(TokenConstants.Cirrus.Decimals),
                Src = reservesDto.Src.InsertDecimal(srcTokenDecimals),
                Usd = reservesDto.Usd,
                UsdDailyChange = reservesDto.UsdDailyChange,
            };
        }

        private static RewardsResponseModel MapRewards(RewardsDto rewardsDto)
        {
            if (rewardsDto == null) return null;

            var providerUsd = rewardsDto.ProviderUsd;
            var marketUsd = rewardsDto.MarketUsd;

            return new RewardsResponseModel
            {
                ProviderUsd = providerUsd,
                MarketUsd = marketUsd,
                TotalUsd = providerUsd + marketUsd
            };
        }

        private static VolumeResponseModel MapVolume(VolumeDto volumeDto, int srcTokenDecimals)
        {
            if (volumeDto == null) return null;

            return new VolumeResponseModel
            {
                Crs = volumeDto.Crs.ToString().InsertDecimal(TokenConstants.Cirrus.Decimals),
                Src = volumeDto.Src.InsertDecimal(srcTokenDecimals),
                Usd = volumeDto.Usd
            };
        }

        private static StakingResponseModel MapStaking(StakingDto stakingDto, bool stakingEnabled)
        {
            if (stakingDto == null) return null;

            return new StakingResponseModel
            {
                Weight = stakingDto.Weight.InsertDecimal(TokenConstants.Opdex.Decimals),
                Usd = stakingDto.Usd,
                WeightDailyChange = stakingDto.WeightDailyChange,
                IsActive = stakingEnabled
            };
        }

        private static CostResponseModel MapCost(CostDto costDto, int srcTokenDecimals)
        {
            if (costDto == null) return null;

            return new CostResponseModel
            {
                CrsPerSrc = MapOhlc(costDto.CrsPerSrc, TokenConstants.Cirrus.Decimals),
                SrcPerCrs = MapOhlc(costDto.SrcPerCrs, srcTokenDecimals)
            };
        }

        private static OhlcBigIntResponseModel MapOhlc(OhlcBigIntDto ohlcDto, int decimals)
        {
            if (ohlcDto == null) return null;

            return new OhlcBigIntResponseModel
            {
                Open = ohlcDto.Open.InsertDecimal(decimals),
                High = ohlcDto.High.InsertDecimal(decimals),
                Low = ohlcDto.Low.InsertDecimal(decimals),
                Close = ohlcDto.Close.InsertDecimal(decimals),
            };
        }
    }
}
