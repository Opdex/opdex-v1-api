using AutoMapper;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs;

namespace Opdex.Indexer.Infrastructure
{
    public class IndexerInfrastructureMapperProfile : Profile
    {
        public IndexerInfrastructureMapperProfile()
        {
            CreateMap<Token, TokenEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Decimals))
                .ForMember(dest => dest.Sats, opt => opt.MapFrom(src => src.Sats))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<LiquidityPool, LiquidityPoolEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
                .ForMember(dest => dest.ReserveCrs, opt => opt.MapFrom(src => src.ReserveCrs))
                .ForMember(dest => dest.ReserveSrc, opt => opt.MapFrom(src => src.ReserveSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MiningPool, MiningPoolEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LiquidityPoolId, opt => opt.MapFrom(src => src.LiquidityPoolId))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.RewardRate, opt => opt.MapFrom(src => src.RewardRate))
                .ForMember(dest => dest.MiningPeriodEndBlock, opt => opt.MapFrom(src => src.MiningPeriodEndBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<Block, BlockEntity>()
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.MedianTime, opt => opt.MapFrom(src => src.MedianTime))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Transaction, TransactionEntity>()
                .ForMember(dest => dest.Block, opt => opt.MapFrom(src => src.BlockHeight))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.GasUsed, opt => opt.MapFrom(src => src.GasUsed))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<PersistTransactionLogCommand, TransactionLogEntity>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details))
                .ForMember(dest => dest.LogTypeId, opt => opt.MapFrom(src => src.LogTypeId))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}