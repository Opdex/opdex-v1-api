using System;
using AutoMapper;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents;

namespace Opdex.Core.Infrastructure
{
    public class CoreInfrastructureMapperProfile : Profile
    {
        public CoreInfrastructureMapperProfile()
        {
            CreateMap<TransactionReceiptDto, Transaction>()
                .ConstructUsing(src => new Transaction(src.TransactionHash, src.BlockHeight, src.GasUsed, src.From, src.To))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TransactionEntity, Transaction>()
                .ConstructUsing(src => new Transaction(src.Id, src.Hash, src.Block, src.GasUsed, src.From, src.To, new TransactionEvent[0]))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TokenEntity, Token>()
                .ConstructUsing(src => new Token(src.Id, src.Address, src.Name, src.Symbol, src.Decimals, src.Sats, src.TotalSupply))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<BlockEntity, Block>()
                .ConstructUsing(src => new Block(src.Height, src.Hash, src.Time, src.MedianTime))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<PairEntity, Pair>()
                .ConstructUsing(src => new Pair(src.Id, src.Address, src.TokenId, src.ReserveCrs, src.ReserveSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MarketSnapshotEntity, MarketSnapshot>()
                .ConstructUsing(src => new MarketSnapshot(src.Id, src.TokenCount, src.PairCount, src.DailyTransactionCount, src.CrsPrice, src.Liquidity, src.DailyFees, src.DailyVolume, src.Block))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            // Transaction Events
            CreateMap<MintEventEntity, MintEvent>()
                .ConstructUsing(src => new MintEvent(src.Id, src.TransactionId, src.Address, src.SortOrder, src.Sender, src.AmountCrs, src.AmountSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<BurnEventEntity, BurnEvent>()
                .ConstructUsing(src => new BurnEvent(src.Id, src.TransactionId, src.Address, src.SortOrder, src.Sender, src.To, src.AmountCrs, src.AmountSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<SwapEventEntity, SwapEvent>()
                .ConstructUsing(src => new SwapEvent(src.Id, src.TransactionId, src.Address, src.SortOrder, src.Sender, src.To, src.AmountCrsIn, src.AmountCrsOut, src.AmountSrcIn, src.AmountSrcOut))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TransferEventEntity, TransferEvent>()
                .ConstructUsing(src => new TransferEvent(src.Id, src.TransactionId, src.Address, src.SortOrder, src.From, src.To, src.Amount ))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<ApprovalEventEntity, ApprovalEvent>()
                .ConstructUsing(src => new ApprovalEvent(src.Id, src.TransactionId, src.Address, src.SortOrder, src.Owner, src.Spender, src.Amount ))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<SyncEventEntity, SyncEvent>()
                .ConstructUsing(src => new SyncEvent(src.Id, src.TransactionId, src.Address, src.SortOrder, src.ReserveCrs, src.ReserveSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<PairCreatedEventEntity, PairCreatedEvent>()
                .ConstructUsing(src => new PairCreatedEvent(src.Id, src.TransactionId, src.Address, src.SortOrder, src.Token, src.Pair))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TransactionEventSummaryEntity, TransactionEventSummary>()
                .ConstructUsing(src => new TransactionEventSummary(src.Id, src.TransactionId, src.EventId, src.EventTypeId, src.SortOrder, src.Contract))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}