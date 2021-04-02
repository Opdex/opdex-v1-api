using System.Linq;
using AutoMapper;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Models.TransactionEvents;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Core.Application
{
    public class CoreApplicationMapperProfile : Profile
    {
        public CoreApplicationMapperProfile()
        {
            CreateMap<Block, BlockDto>()
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.MedianTime, opt => opt.MapFrom(src => src.MedianTime))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<Pair, PairDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.ReserveCrs, opt => opt.MapFrom(src => src.ReserveCrs))
                .ForMember(dest => dest.ReserveSrc, opt => opt.MapFrom(src => src.ReserveSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<Token, TokenDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Decimals))
                .ForMember(dest => dest.Sats, opt => opt.MapFrom(src => src.Sats))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MarketSnapshot, MarketSnapshotDto>()
                .ForMember(dest => dest.Block, opt => opt.MapFrom(src => src.Block))
                .ForMember(dest => dest.Liquidity, opt => opt.MapFrom(src => src.Liquidity))
                .ForMember(dest => dest.TokenCount, opt => opt.MapFrom(src => src.TokenCount))
                .ForMember(dest => dest.PairCount, opt => opt.MapFrom(src => src.PairCount))
                .ForMember(dest => dest.DailyTransactionCount, opt => opt.MapFrom(src => src.DailyTransactionCount))
                .ForMember(dest => dest.CrsPrice, opt => opt.MapFrom(src => src.CrsPrice))
                .ForMember(dest => dest.DailyFees, opt => opt.MapFrom(src => src.DailyFees))
                .ForMember(dest => dest.DailyVolume, opt => opt.MapFrom(src => src.DailyVolume))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.BlockHeight, opt => opt.MapFrom(src => src.BlockHeight))
                .ForMember(dest => dest.GasUsed, opt => opt.MapFrom(src => src.GasUsed))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .AfterMap((src, dest, ctx) =>
                {
                    var events = src.Events.Select(txEvent =>
                        {
                            return (TransactionEventDto) (txEvent.EventType switch
                            {
                                nameof(SyncEvent) => ctx.Mapper.Map<SyncEventDto>(txEvent),
                                nameof(BurnEvent) => ctx.Mapper.Map<BurnEventDto>(txEvent),
                                nameof(MintEvent) => ctx.Mapper.Map<MintEventDto>(txEvent),
                                nameof(SwapEvent) => ctx.Mapper.Map<SwapEventDto>(txEvent),
                                nameof(ApprovalEvent) => ctx.Mapper.Map<ApprovalEventDto>(txEvent),
                                nameof(TransferEvent) => ctx.Mapper.Map<TransferEventDto>(txEvent),
                                nameof(PairCreatedEvent) => ctx.Mapper.Map<PairCreatedEventDto>(txEvent),
                                _ => null
                            });
                        })
                        .Where(eventDto => eventDto != null)
                        .ToList();
                
                    dest.Events = events;
                })
                .ForAllOtherMembers(opt => opt.Ignore());
            
            // Transaction Events
            
            CreateMap<TransactionEvent, TransactionEventDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.EventType))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<SyncEvent, SyncEventDto>()
                .IncludeBase<TransactionEvent, TransactionEventDto>()
                .ForMember(dest => dest.ReserveCrs, opt => opt.MapFrom(src => src.ReserveCrs))
                .ForMember(dest => dest.ReserveSrc, opt => opt.MapFrom(src => src.ReserveSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MintEvent, MintEventDto>()
                .IncludeBase<TransactionEvent, TransactionEventDto>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.AmountCrs, opt => opt.MapFrom(src => src.AmountCrs))
                .ForMember(dest => dest.AmountSrc, opt => opt.MapFrom(src => src.AmountSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<BurnEvent, BurnEventDto>()
                .IncludeBase<TransactionEvent, TransactionEventDto>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.AmountCrs, opt => opt.MapFrom(src => src.AmountCrs))
                .ForMember(dest => dest.AmountSrc, opt => opt.MapFrom(src => src.AmountSrc))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<SwapEvent, SwapEventDto>()
                .IncludeBase<TransactionEvent, TransactionEventDto>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.AmountCrsIn, opt => opt.MapFrom(src => src.AmountCrsIn))
                .ForMember(dest => dest.AmountCrsOut, opt => opt.MapFrom(src => src.AmountCrsOut))
                .ForMember(dest => dest.AmountSrcIn, opt => opt.MapFrom(src => src.AmountSrcIn))
                .ForMember(dest => dest.AmountSrcOut, opt => opt.MapFrom(src => src.AmountSrcOut))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<PairCreatedEvent, PairCreatedEventDto>()
                .IncludeBase<TransactionEvent, TransactionEventDto>()
                .ForMember(dest => dest.Pair, opt => opt.MapFrom(src => src.Pair))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TransferEvent, TransferEventDto>()
                .IncludeBase<TransactionEvent, TransactionEventDto>()
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<ApprovalEvent, ApprovalEventDto>()
                .IncludeBase<TransactionEvent, TransactionEventDto>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Spender, opt => opt.MapFrom(src => src.Spender))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}