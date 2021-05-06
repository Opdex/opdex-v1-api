using AutoMapper;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;

namespace Opdex.Platform.Infrastructure
{
    public class PlatformInfrastructureMapperProfile : Profile
    {
        public PlatformInfrastructureMapperProfile()
        {
            CreateMap<TransactionReceiptDto, Transaction>()
                .ConstructUsing(src => new Transaction(src.TransactionHash, src.BlockHeight, src.GasUsed, src.From, src.To, src.NewContractAddress))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TransactionEntity, Transaction>()
                .ConstructUsing(src => new Transaction(src.Id, src.Hash, src.Block, src.GasUsed, src.From, src.To, new TransactionLog[0], null))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TokenEntity, Token>()
                .ConstructUsing(src => new Token(src.Id, src.Address, src.Name, src.Symbol, src.Decimals, src.Sats, src.TotalSupply))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TokenSnapshotEntity, TokenSnapshot>()
                .ConstructUsing(src => new TokenSnapshot(src.Id, src.TokenId, src.Price, src.SnapshotTypeId, src.StartDate, src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<BlockEntity, Block>()
                .ConstructUsing(src => new Block(src.Height, src.Hash, src.Time, src.MedianTime))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<LiquidityPoolEntity, LiquidityPool>()
                .ConstructUsing(src => new LiquidityPool(src.Id, src.Address, src.TokenId, src.MarketId))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<LiquidityPoolSnapshotEntity, LiquidityPoolSnapshot>()
                .ConstructUsing(src => new LiquidityPoolSnapshot(src.Id, src.LiquidityPoolId, src.TransactionCount, src.ReserveCrs, src.ReserveSrc, src.ReserveUsd,
                    src.VolumeCrs, src.VolumeSrc, src.VolumeUsd, src.StakingWeight, src.StakingUsd, src.ProviderRewards, src.StakerRewards, (SnapshotType)src.SnapshotTypeId, src.StartDate, src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MiningPoolEntity, MiningPool>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LiquidityPoolId, opt => opt.MapFrom(src => src.LiquidityPoolId))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.RewardRate, opt => opt.MapFrom(src => src.RewardRate))
                .ForMember(dest => dest.MiningPeriodEndBlock, opt => opt.MapFrom(src => src.MiningPeriodEndBlock))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketEntity, Market>()
                .ConstructUsing(src => new Market(src.Id, src.Address, src.AuthPoolCreators, src.AuthProviders, src.AuthTraders, src.Fee, src.Staking))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MarketSnapshotEntity, MarketSnapshot>()
                .ConstructUsing(src => new MarketSnapshot(src.Id, src.MarketId, src.TransactionCount, src.Liquidity, src.Volume, src.StakingWeight, src.StakingUsd, 
                    src.ProviderRewards, src.StakerRewards, (SnapshotType)src.SnapshotTypeId, src.StartDate, src.EndDate))
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
                        (int)TransactionLogType.StartStakingLog => new StartStakingLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.StartMiningLog => new StartMiningLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.CollectStakingRewardsLog => new CollectStakingRewardsLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.CollectMiningRewardsLog => new CollectMiningRewardsLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.StopStakingLog => new StopStakingLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.StopMiningLog => new StopMiningLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.RewardMiningPoolLog => new RewardMiningPoolLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.MiningPoolRewardedLog => new MiningPoolRewardedLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.NominationLog => new NominationLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.OwnerChangeLog => new OwnerChangeLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.DistributionLog => new DistributionLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.MarketCreatedLog => new MarketCreatedLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.MarketOwnerChangeLog => new MarketOwnerChangeLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.PermissionsChangeLog => new PermissionsChangeLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.MarketChangeLog => new MarketChangeLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        _ => null
                    };
                })
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<Market, MarketEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.AuthPoolCreators, opt => opt.MapFrom(src => src.AuthPoolCreators))
                .ForMember(dest => dest.AuthProviders, opt => opt.MapFrom(src => src.AuthProviders))
                .ForMember(dest => dest.AuthTraders, opt => opt.MapFrom(src => src.AuthTraders))
                .ForMember(dest => dest.Fee, opt => opt.MapFrom(src => src.Fee))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MarketSnapshot, MarketSnapshotEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.TransactionCount))
                .ForMember(dest => dest.Liquidity, opt => opt.MapFrom(src => src.Liquidity))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.StakingWeight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.StakingUsd, opt => opt.MapFrom(src => src.WeightUsd))
                .ForMember(dest => dest.ProviderRewards, opt => opt.MapFrom(src => src.ProviderRewards))
                .ForMember(dest => dest.StakerRewards, opt => opt.MapFrom(src => src.StakerRewards))
                .ForMember(dest => dest.SnapshotTypeId, opt => opt.MapFrom(src => src.SnapshotType))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
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
            
            CreateMap<TokenSnapshot, TokenSnapshotEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.SnapshotTypeId, opt => opt.MapFrom(src => (int)src.SnapshotType))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<LiquidityPool, LiquidityPoolEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
                .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<LiquidityPoolSnapshot, LiquidityPoolSnapshotEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LiquidityPoolId, opt => opt.MapFrom(src => src.LiquidityPoolId))
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.TransactionCount))
                .ForMember(dest => dest.ReserveCrs, opt => opt.MapFrom(src => src.ReserveCrs))
                .ForMember(dest => dest.ReserveSrc, opt => opt.MapFrom(src => src.ReserveSrc))
                .ForMember(dest => dest.ReserveUsd, opt => opt.MapFrom(src => src.ReserveUsd))
                .ForMember(dest => dest.VolumeCrs, opt => opt.MapFrom(src => src.VolumeCrs))
                .ForMember(dest => dest.VolumeSrc, opt => opt.MapFrom(src => src.VolumeSrc))
                .ForMember(dest => dest.VolumeUsd, opt => opt.MapFrom(src => src.VolumeUsd))
                .ForMember(dest => dest.StakingWeight, opt => opt.MapFrom(src => src.StakingWeight))
                .ForMember(dest => dest.StakingUsd, opt => opt.MapFrom(src => src.StakingUsd))
                .ForMember(dest => dest.ProviderRewards, opt => opt.MapFrom(src => src.ProviderRewards))
                .ForMember(dest => dest.StakerRewards, opt => opt.MapFrom(src => src.StakerRewards))
                .ForMember(dest => dest.SnapshotTypeId, opt => opt.MapFrom(src => (int)src.SnapshotType))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
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