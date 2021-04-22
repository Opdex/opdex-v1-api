using AutoMapper;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.TransactionLogs;

namespace Opdex.Platform.Infrastructure
{
    public class PlatformInfrastructureMapperProfile : Profile
    {
        public PlatformInfrastructureMapperProfile()
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
            
            CreateMap<LiquidityPoolEntity, LiquidityPool>()
                .ConstructUsing(src => new LiquidityPool(src.Id, src.Address, src.TokenId, src.ReserveCrs, src.ReserveSrc))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MarketSnapshotEntity, MarketSnapshot>()
                .ConstructUsing(src => new MarketSnapshot(src.Id, src.TokenCount, src.PoolCount, src.DailyTransactionCount, src.CrsPrice, src.Liquidity, src.DailyFees, src.DailyVolume, src.Block))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TransactionLogEntity, TransactionLog>()
                .ConstructUsing((src, ctx) =>
                {
                    return src.LogTypeId switch
                    {
                        (int)TransactionLogType.ReservesLog => new ReservesLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.BurnLog => new BurnLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.MintLog => new MintLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.SwapLog => new SwapLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.ApprovalLog => new ApprovalLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.TransferLog => new TransferLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.LiquidityPoolCreatedLog => new LiquidityPoolCreatedLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.MiningPoolCreatedLog => new MiningPoolCreatedLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.EnterStakingPoolLog => new EnterStakingPoolLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.EnterMiningPoolLog => new EnterMiningPoolLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.CollectStakingRewardsLog => new CollectStakingRewardsLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.CollectMiningRewardsLog => new CollectMiningRewardsLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.ExitStakingPoolLog => new ExitStakingPoolLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.ExitMiningPoolLog => new ExitMiningPoolLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.RewardMiningPoolLog => new RewardMiningPoolLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.MiningPoolRewardedLog => new MiningPoolRewardedLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.NominationLog => new NominationLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.OwnerChangeLog => new OwnerChangeLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.DistributionLog => new DistributionLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        _ => null
                    };
                })
                .ForAllOtherMembers(opt => opt.Ignore());
            
            
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