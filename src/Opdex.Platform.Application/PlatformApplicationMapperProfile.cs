using AutoMapper;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using System.Linq;

namespace Opdex.Platform.Application
{
    public class PlatformApplicationMapperProfile : Profile
    {
        public PlatformApplicationMapperProfile()
        {
             CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.BlockHeight, opt => opt.MapFrom(src => src.BlockHeight))
                .ForMember(dest => dest.GasUsed, opt => opt.MapFrom(src => src.GasUsed))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.Logs, opt => opt.MapFrom(src => src.Logs))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<Block, BlockDto>()
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.MedianTime, opt => opt.MapFrom(src => src.MedianTime))
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
            
            CreateMap<LiquidityPool, LiquidityPoolDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<Block, BlockDto>()
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.MedianTime, opt => opt.MapFrom(src => src.MedianTime))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<LiquidityPool, LiquidityPoolDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
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
            
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.BlockHeight, opt => opt.MapFrom(src => src.BlockHeight))
                .ForMember(dest => dest.GasUsed, opt => opt.MapFrom(src => src.GasUsed))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .AfterMap((src, dest, ctx) =>
                {
                    var logs = src.Logs.Select(txLog =>
                        {
                            return (TransactionLogDto) (txLog.LogType switch
                            {
                                TransactionLogType.ReservesLog => ctx.Mapper.Map<ReservesLogDto>(txLog),
                                TransactionLogType.BurnLog => ctx.Mapper.Map<BurnLogDto>(txLog),
                                TransactionLogType.MintLog => ctx.Mapper.Map<MintLogDto>(txLog),
                                TransactionLogType.SwapLog => ctx.Mapper.Map<SwapLogDto>(txLog),
                                TransactionLogType.ApprovalLog => ctx.Mapper.Map<ApprovalLogDto>(txLog),
                                TransactionLogType.TransferLog => ctx.Mapper.Map<TransferLogDto>(txLog),
                                TransactionLogType.LiquidityPoolCreatedLog => ctx.Mapper.Map<LiquidityPoolCreatedLogDto>(txLog),
                                TransactionLogType.MiningPoolCreatedLog => ctx.Mapper.Map<MiningPoolCreatedLogDto>(txLog),
                                TransactionLogType.StartMiningLog => ctx.Mapper.Map<StartMiningLogDto>(txLog),
                                TransactionLogType.StartStakingLog => ctx.Mapper.Map<StartStakingLogDto>(txLog),
                                TransactionLogType.StopMiningLog => ctx.Mapper.Map<StopMiningLogDto>(txLog),
                                TransactionLogType.StopStakingLog => ctx.Mapper.Map<StopMiningLogDto>(txLog),
                                TransactionLogType.DistributionLog => ctx.Mapper.Map<DistributionLogDto>(txLog),
                                TransactionLogType.OwnerChangeLog => ctx.Mapper.Map<OwnerChangeLogDto>(txLog),
                                TransactionLogType.MiningPoolRewardedLog => ctx.Mapper.Map<MiningPoolRewardedLogDto>(txLog),
                                TransactionLogType.RewardMiningPoolLog => ctx.Mapper.Map<RewardMiningPoolLogDto>(txLog),
                                TransactionLogType.CollectStakingRewardsLog => ctx.Mapper.Map<CollectStakingRewardsLogDto>(txLog),
                                TransactionLogType.CollectMiningRewardsLog => ctx.Mapper.Map<CollectMiningRewardsLogDto>(txLog),
                                _ => null
                            });
                        })
                        .Where(logDto => logDto != null)
                        .ToList();
                
                    dest.Logs = logs;
                })
                .ForAllOtherMembers(opt => opt.Ignore());
            
            // Transaction Logs
            
            CreateMap<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LogType, opt => opt.MapFrom(src => src.LogType))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Contract))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<ReservesLog, ReservesLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.ReserveCrs, opt => opt.MapFrom(src => src.ReserveCrs))
                .ForMember(dest => dest.ReserveSrc, opt => opt.MapFrom(src => src.ReserveSrc));

            CreateMap<MintLog, MintLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.AmountCrs, opt => opt.MapFrom(src => src.AmountCrs))
                .ForMember(dest => dest.AmountSrc, opt => opt.MapFrom(src => src.AmountSrc));

            CreateMap<BurnLog, BurnLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.AmountCrs, opt => opt.MapFrom(src => src.AmountCrs))
                .ForMember(dest => dest.AmountSrc, opt => opt.MapFrom(src => src.AmountSrc));

            CreateMap<SwapLog, SwapLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.AmountCrsIn, opt => opt.MapFrom(src => src.AmountCrsIn))
                .ForMember(dest => dest.AmountCrsOut, opt => opt.MapFrom(src => src.AmountCrsOut))
                .ForMember(dest => dest.AmountSrcIn, opt => opt.MapFrom(src => src.AmountSrcIn))
                .ForMember(dest => dest.AmountSrcOut, opt => opt.MapFrom(src => src.AmountSrcOut));

            CreateMap<LiquidityPoolCreatedLog, LiquidityPoolCreatedLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Pool, opt => opt.MapFrom(src => src.Pool))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token));

            CreateMap<TransferLog, TransferLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<ApprovalLog, ApprovalLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Spender, opt => opt.MapFrom(src => src.Spender))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
            
            CreateMap<CollectMiningRewardsLog, CollectMiningRewardsLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Miner, opt => opt.MapFrom(src => src.Miner))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
            
            CreateMap<CollectStakingRewardsLog, CollectStakingRewardsLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Staker, opt => opt.MapFrom(src => src.Staker))
                .ForMember(dest => dest.Reward, opt => opt.MapFrom(src => src.Reward));

            CreateMap<DistributionLog, DistributionLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.OwnerAddress, opt => opt.MapFrom(src => src.OwnerAddress))
                .ForMember(dest => dest.MiningAddress, opt => opt.MapFrom(src => src.MiningAddress))
                .ForMember(dest => dest.OwnerAmount, opt => opt.MapFrom(src => src.OwnerAmount))
                .ForMember(dest => dest.MiningAmount, opt => opt.MapFrom(src => src.MiningAmount))
                .ForMember(dest => dest.PeriodIndex, opt => opt.MapFrom(src => src.PeriodIndex));
            
            CreateMap<StartMiningLog, StartMiningLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Miner, opt => opt.MapFrom(src => src.Miner))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
            
            CreateMap<StartStakingLog, StartStakingLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Staker, opt => opt.MapFrom(src => src.Staker))
                .ForMember(dest => dest.TotalStaked, opt => opt.MapFrom(src => src.TotalStaked))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
            
            CreateMap<StopMiningLog, StopMiningLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Miner, opt => opt.MapFrom(src => src.Miner))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<StopStakingLog, StopStakingLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Staker, opt => opt.MapFrom(src => src.Staker))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.TotalStaked, opt => opt.MapFrom(src => src.TotalStaked));
            
            CreateMap<MiningPoolCreatedLog, MiningPoolCreatedLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.StakingPool, opt => opt.MapFrom(src => src.StakingPool))
                .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool));

            CreateMap<MiningPoolRewardedLog, MiningPoolRewardedLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
            
            CreateMap<NominationLog, NominationLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.StakingPool, opt => opt.MapFrom(src => src.StakingPool))
                .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight));

            CreateMap<OwnerChangeLog, OwnerChangeLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To));
            
            CreateMap<RewardMiningPoolLog, RewardMiningPoolLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.StakingPool, opt => opt.MapFrom(src => src.StakingPool))
                .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
        }
    }
}