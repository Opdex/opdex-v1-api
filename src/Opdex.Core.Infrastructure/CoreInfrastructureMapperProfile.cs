using System;
using AutoMapper;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs;

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
                .ConstructUsing(src => new Transaction(src.Id, src.Hash, src.Block, src.GasUsed, src.From, src.To, new TransactionLog[0]))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TokenEntity, Token>()
                .ConstructUsing(src => new Token(src.Id, src.Address, src.Name, src.Symbol, src.Decimals, src.Sats, src.TotalSupply))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<BlockEntity, Block>()
                .ConstructUsing(src => new Block(src.Height, src.Hash, src.Time, src.MedianTime))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<PoolEntity, Pool>()
                .ConstructUsing(src => new Pool(src.Id, src.Address, src.TokenId, src.ReserveCrs, src.ReserveSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MarketSnapshotEntity, MarketSnapshot>()
                .ConstructUsing(src => new MarketSnapshot(src.Id, src.TokenCount, src.PoolCount, src.DailyTransactionCount, src.CrsPrice, src.Liquidity, src.DailyFees, src.DailyVolume, src.Block))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            // Transaction Logs
            CreateMap<MintLogEntity, MintLog>()
                .ConstructUsing(src => new MintLog(src.Id, src.TransactionId, src.Address, src.SortOrder, src.Sender, src.AmountCrs, src.AmountSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<BurnLogEntity, BurnLog>()
                .ConstructUsing(src => new BurnLog(src.Id, src.TransactionId, src.Address, src.SortOrder, src.Sender, src.To, src.AmountCrs, src.AmountSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<SwapLogEntity, SwapLog>()
                .ConstructUsing(src => new SwapLog(src.Id, src.TransactionId, src.Address, src.SortOrder, src.Sender, src.To, src.AmountCrsIn, src.AmountCrsOut, src.AmountSrcIn, src.AmountSrcOut))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TransferLogEntity, TransferLog>()
                .ConstructUsing(src => new TransferLog(src.Id, src.TransactionId, src.Address, src.SortOrder, src.From, src.To, src.Amount ))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<ApprovalLogEntity, ApprovalLog>()
                .ConstructUsing(src => new ApprovalLog(src.Id, src.TransactionId, src.Address, src.SortOrder, src.Owner, src.Spender, src.Amount ))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<ReservesLogEntity, ReservesLog>()
                .ConstructUsing(src => new ReservesLog(src.Id, src.TransactionId, src.Address, src.SortOrder, src.ReserveCrs, src.ReserveSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<LiquidityPoolCreatedLogEntity, LiquidityPoolCreatedLog>()
                .ConstructUsing(src => new LiquidityPoolCreatedLog(src.Id, src.TransactionId, src.Address, src.SortOrder, src.Token, src.Pool))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TransactionLogSummaryEntity, TransactionLogSummary>()
                .ConstructUsing(src => new TransactionLogSummary(src.Id, src.TransactionId, src.LogId, src.LogTypeId, src.SortOrder, src.Contract))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}