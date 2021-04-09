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
        }
    }
}