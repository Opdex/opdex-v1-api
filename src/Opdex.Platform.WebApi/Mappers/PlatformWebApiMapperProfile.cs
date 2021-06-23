using AutoMapper;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.OHLC;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Markets;
using Opdex.Platform.WebApi.Models.Responses.OHLC;
using Opdex.Platform.WebApi.Models.Responses.Pools;
using Opdex.Platform.WebApi.Models.Responses.Tokens;

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
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TokenSnapshotDto, TokenSummaryResponseModel>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolDto, LiquidityPoolResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.SrcToken, opt => opt.MapFrom(src => src.SrcToken))
                .ForMember(dest => dest.LpToken, opt => opt.MapFrom(src => src.LpToken))
                .ForMember(dest => dest.StakingToken, opt => opt.MapFrom(src => src.StakingToken))
                .ForMember(dest => dest.CrsToken, opt => opt.MapFrom(src => src.CrsToken))
                .ForMember(dest => dest.StakingEnabled, opt => opt.MapFrom(src => src.StakingEnabled))
                .ForMember(dest => dest.MiningEnabled, opt => opt.MapFrom(src => src.MiningEnabled))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => MapLiquidityPoolSummary(src.Summary)))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolSnapshotDto, LiquidityPoolSummaryResponseModel>()
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.TransactionCount))
                .ForMember(dest => dest.Reserves, opt => opt.MapFrom(src => MapReserves(src.Reserves, src.SrcTokenDecimals)))
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => MapRewards(src.Rewards)))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => MapStaking(src.Staking)))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => MapVolume(src.Volume, src.SrcTokenDecimals)))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => MapCost(src.Cost, src.SrcTokenDecimals)))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OhlcDecimalDto, OhlcDecimalResponseModel>()
                .ForMember(dest => dest.Open, opt => opt.MapFrom(src => src.Open))
                .ForMember(dest => dest.High, opt => opt.MapFrom(src => src.High))
                .ForMember(dest => dest.Low, opt => opt.MapFrom(src => src.Low))
                .ForMember(dest => dest.Close, opt => opt.MapFrom(src => src.Close))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketSnapshotDto, MarketSnapshotResponseModel>()
                .ForMember(dest => dest.Liquidity, opt => opt.MapFrom(src => src.Liquidity))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => MapStaking(src.Staking)))
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
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => MapMarketSummary(src)))
                .ForAllOtherMembers(opt => opt.Ignore());
        }

        private static MarketSnapshotResponseModel MapMarketSummary(MarketDto snapshot)
        {
            if (snapshot.Summary == null) return null;

            return new MarketSnapshotResponseModel
            {
                StartDate = snapshot.Summary.StartDate,
                EndDate = snapshot.Summary.EndDate,
                Liquidity = snapshot.Summary.Liquidity,
                Volume = snapshot.Summary.Volume,
                Rewards = new RewardsResponseModel
                {
                    MarketUsd = snapshot.Summary.Rewards.MarketUsd,
                    ProviderUsd = snapshot.Summary.Rewards.ProviderUsd,
                    TotalUsd = snapshot.Summary.Rewards.MarketUsd + snapshot.Summary.Rewards.ProviderUsd
                },
                Staking = new StakingResponseModel
                {
                    Usd = snapshot.Summary.Staking.Usd,
                    Weight = snapshot.Summary.Staking.Weight
                }
            };
        }

        private static LiquidityPoolSummaryResponseModel MapLiquidityPoolSummary(LiquidityPoolSnapshotDto snapshot)
        {
            if (snapshot == null) return null;

            var srcDecimals = snapshot.SrcTokenDecimals;

            return new LiquidityPoolSummaryResponseModel
            {
                TransactionCount = snapshot.TransactionCount,
                Reserves = MapReserves(snapshot.Reserves, srcDecimals),
                Rewards = MapRewards(snapshot.Rewards),
                Cost = MapCost(snapshot.Cost, srcDecimals),
                Volume = MapVolume(snapshot.Volume, srcDecimals),
                Staking =  MapStaking(snapshot.Staking),
                StartDate = snapshot.StartDate,
                EndDate = snapshot.EndDate
            };
        }

        private static ReservesResponseModel MapReserves(ReservesDto reservesDto, int srcTokenDecimals)
        {
            if (reservesDto == null) return null;

            return new ReservesResponseModel
            {
                Crs = reservesDto.Crs.ToString().InsertDecimal(TokenConstants.Cirrus.Decimals),
                Src = reservesDto.Src.InsertDecimal(srcTokenDecimals),
                Usd = reservesDto.Usd
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

        private static StakingResponseModel MapStaking(StakingDto stakingDto)
        {
            if (stakingDto == null) return null;

            return new StakingResponseModel
            {
                Weight = stakingDto.Weight.InsertDecimal(TokenConstants.Opdex.Decimals),
                Usd = stakingDto.Usd
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
