using AutoMapper;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses;
using Opdex.Platform.WebApi.Models.Responses.Pools;

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
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolDto, LiquidityPoolResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
                .ForMember(dest => dest.StakingEnabled, opt => opt.MapFrom(src => src.StakingEnabled))
                .ForMember(dest => dest.MiningEnabled, opt => opt.MapFrom(src => src.MiningEnabled))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => MapLiquidityPoolSummary(src)))
                .ForAllOtherMembers(opt => opt.Ignore());

            // CreateMap<LiquidityPoolSnapshotDto, LiquidityPoolSummaryResponseModel>()
            //     .ConstructUsing(src => MapLiquidityPoolSummary(src))
            //     .ForAllOtherMembers(opt => opt.Ignore);

            CreateMap<MarketSnapshotDto, MarketSnapshotResponseModel>()
                .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.TransactionCount))
                .ForMember(dest => dest.Liquidity, opt => opt.MapFrom(src => src.Liquidity))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.ProviderRewards, opt => opt.MapFrom(src => src.ProviderRewards))
                .ForMember(dest => dest.StakerRewards, opt => opt.MapFrom(src => src.StakerRewards))
                .ForMember(dest => dest.SnapshotType, opt => opt.MapFrom(src => src.SnapshotType))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());
        }

        private static LiquidityPoolSummaryResponseModel MapLiquidityPoolSummary(LiquidityPoolDto liquidityPoolDto)
        {
            if (liquidityPoolDto.Summary == null) return null;

            return new LiquidityPoolSummaryResponseModel
            {
                TransactionCount = liquidityPoolDto.Summary.TransactionCount,
                Reserves = MapReserves(liquidityPoolDto),
                Rewards = MapRewards(liquidityPoolDto),
                Cost = MapCost(liquidityPoolDto),
                Volume = MapVolume(liquidityPoolDto),
                Staking =  MapStaking(liquidityPoolDto)
            };
        }

        private static ReservesResponseModel MapReserves(LiquidityPoolDto liquidityPoolDto)
        {
            if (liquidityPoolDto.Summary.Reserves == null) return null;

            return new ReservesResponseModel
            {
                Crs = liquidityPoolDto.Summary.Reserves.Crs.InsertDecimal(TokenConstants.Cirrus.Decimals),
                Src = liquidityPoolDto.Summary.Reserves.Src.InsertDecimal(liquidityPoolDto.Token.Decimals),
                Usd = liquidityPoolDto.Summary.Reserves.Usd
            };
        }

        private static RewardsResponseModel MapRewards(LiquidityPoolDto liquidityPoolDto)
        {
            if (liquidityPoolDto.Summary.Rewards == null) return null;

            var providerUsd = liquidityPoolDto.Summary.Rewards.ProviderUsd;
            var marketUsd = liquidityPoolDto.Summary.Rewards.MarketUsd;

            return new RewardsResponseModel
            {
                ProviderUsd = providerUsd,
                MarketUsd = marketUsd,
                TotalUsd = providerUsd + marketUsd
            };
        }

        private static VolumeResponseModel MapVolume(LiquidityPoolDto liquidityPoolDto)
        {
            if (liquidityPoolDto.Summary.Volume == null) return null;

            return new VolumeResponseModel
            {
                Crs = liquidityPoolDto.Summary.Volume.Crs.InsertDecimal(TokenConstants.Cirrus.Decimals),
                Src = liquidityPoolDto.Summary.Volume.Src.InsertDecimal(liquidityPoolDto.Token.Decimals),
                Usd = liquidityPoolDto.Summary.Volume.Usd
            };
        }

        private static StakingResponseModel MapStaking(LiquidityPoolDto liquidityPoolDto)
        {
            if (liquidityPoolDto.Summary.Staking == null) return null;

            return new StakingResponseModel
            {
                Weight = liquidityPoolDto.Summary.Staking.Weight.InsertDecimal(TokenConstants.Opdex.Decimals),
                Usd = liquidityPoolDto.Summary.Staking.Usd
            };
        }

        private static CostResponseModel MapCost(LiquidityPoolDto liquidityPoolDto)
        {
            if (liquidityPoolDto.Summary.Cost == null) return null;

            return new CostResponseModel
            {
                CrsPerSrc = MapOhlc(liquidityPoolDto.Summary.Cost.CrsPerSrc, TokenConstants.Cirrus.Decimals),
                SrcPerCrs = MapOhlc(liquidityPoolDto.Summary.Cost.SrcPerCrs, liquidityPoolDto.Token.Decimals)
            };
        }

        private static OhlcResponseModel MapOhlc(OhlcDto ohlcDto, int decimals)
        {
            if (ohlcDto == null) return null;

            return new OhlcResponseModel
            {
                Open = ohlcDto.Open.InsertDecimal(decimals),
                High = ohlcDto.High.InsertDecimal(decimals),
                Low = ohlcDto.Low.InsertDecimal(decimals),
                Close = ohlcDto.Close.InsertDecimal(decimals),
            };
        }
    }
}