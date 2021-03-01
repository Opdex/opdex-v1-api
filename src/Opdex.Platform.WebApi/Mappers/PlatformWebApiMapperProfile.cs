using AutoMapper;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses;

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
            
            CreateMap<PairDto, PairResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
                .ForMember(dest => dest.ReserveCrs, opt => opt.MapFrom(src => src.ReserveCrs))
                .ForMember(dest => dest.ReserveSrc, opt => opt.MapFrom(src => src.ReserveSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MarketSnapshotDto, MarketSnapshotResponseModel>()
                .ForMember(dest => dest.Block, opt => opt.MapFrom(src => src.Block))
                .ForMember(dest => dest.Liquidity, opt => opt.MapFrom(src => src.Liquidity))
                .ForMember(dest => dest.TokenCount, opt => opt.MapFrom(src => src.TokenCount))
                .ForMember(dest => dest.PairCount, opt => opt.MapFrom(src => src.PairCount))
                .ForMember(dest => dest.DailyTransactionCount, opt => opt.MapFrom(src => src.DailyTransactionCount))
                .ForMember(dest => dest.CrsPrice, opt => opt.MapFrom(src => src.CrsPrice))
                .ForMember(dest => dest.DailyFees, opt => opt.MapFrom(src => src.DailyFees))
                .ForMember(dest => dest.DailyVolume, opt => opt.MapFrom(src => src.DailyVolume))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}