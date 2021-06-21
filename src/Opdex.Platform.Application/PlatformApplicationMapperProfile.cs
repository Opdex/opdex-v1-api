using AutoMapper;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Models.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.Models.TransactionLogs.MiningGovernance;
using Opdex.Platform.Application.Abstractions.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TransactionLogs.Vault;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;
using System.Linq;
using Opdex.Platform.Application.Abstractions.Models.OHLC;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.OHLC;
using Opdex.Platform.Domain.Models.Pools.Snapshots;

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

            CreateMap<Market, MarketDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.AuthPoolCreators, opt => opt.MapFrom(src => src.AuthPoolCreators))
                .ForMember(dest => dest.AuthProviders, opt => opt.MapFrom(src => src.AuthProviders))
                .ForMember(dest => dest.AuthTraders, opt => opt.MapFrom(src => src.AuthTraders))
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

            CreateMap<LiquidityPoolSnapshot, LiquidityPoolSnapshotDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LiquidityPoolId, opt => opt.MapFrom(src => src.LiquidityPoolId))
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.TransactionCount))
                .ForMember(dest => dest.SnapshotTypeId, opt => opt.MapFrom(src => (int)src.SnapshotType))
                .ForMember(dest => dest.Reserves, opt => opt.MapFrom(src => src.Reserves))
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.Rewards))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RewardsSnapshot, RewardsDto>()
                .ForMember(dest => dest.ProviderUsd, opt => opt.MapFrom(src => src.ProviderUsd))
                .ForMember(dest => dest.MarketUsd, opt => opt.MapFrom(src => src.MarketUsd))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ReservesSnapshot, ReservesDto>()
                .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.Crs))
                .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Src))
                .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<VolumeSnapshot, VolumeDto>()
                .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.Crs))
                .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Src))
                .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<StakingSnapshot, StakingDto>()
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<CostSnapshot, CostDto>()
                .ForMember(dest => dest.CrsPerSrc, opt => opt.MapFrom(src => src.CrsPerSrc))
                .ForMember(dest => dest.SrcPerCrs, opt => opt.MapFrom(src => src.SrcPerCrs))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OhlcBigIntSnapshot, OhlcBigIntDto>()
                .ForMember(dest => dest.Open, opt => opt.MapFrom(src => src.Open))
                .ForMember(dest => dest.High, opt => opt.MapFrom(src => src.High))
                .ForMember(dest => dest.Low, opt => opt.MapFrom(src => src.Low))
                .ForMember(dest => dest.Close, opt => opt.MapFrom(src => src.Close))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<OhlcDecimalSnapshot, OhlcDecimalDto>()
                .ForMember(dest => dest.Open, opt => opt.MapFrom(src => src.Open))
                .ForMember(dest => dest.High, opt => opt.MapFrom(src => src.High))
                .ForMember(dest => dest.Low, opt => opt.MapFrom(src => src.Low))
                .ForMember(dest => dest.Close, opt => opt.MapFrom(src => src.Close))
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

            CreateMap<TokenSnapshot, TokenSnapshotDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.SnapshotType, opt => opt.MapFrom(src => src.SnapshotType))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
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
                                TransactionLogType.CreateLiquidityPoolLog => ctx.Mapper.Map<CreateLiquidityPoolLogDto>(txLog),
                                TransactionLogType.MineLog => ctx.Mapper.Map<MineLogDto>(txLog),
                                TransactionLogType.StakeLog => ctx.Mapper.Map<StakeLogDto>(txLog),
                                TransactionLogType.DistributionLog => ctx.Mapper.Map<DistributionLogDto>(txLog),
                                TransactionLogType.ChangeVaultOwnerLog => ctx.Mapper.Map<ChangeVaultOwnerLogDto>(txLog),
                                TransactionLogType.EnableMiningLog => ctx.Mapper.Map<EnableMiningLogDto>(txLog),
                                TransactionLogType.RewardMiningPoolLog => ctx.Mapper.Map<RewardMiningPoolLogDto>(txLog),
                                TransactionLogType.CollectStakingRewardsLog => ctx.Mapper.Map<CollectStakingRewardsLogDto>(txLog),
                                TransactionLogType.CollectMiningRewardsLog => ctx.Mapper.Map<CollectMiningRewardsLogDto>(txLog),
                                TransactionLogType.CreateVaultCertificateLog => ctx.Mapper.Map<CreateVaultCertificateLogDto>(txLog),
                                TransactionLogType.RevokeVaultCertificateLog => ctx.Mapper.Map<RevokeVaultCertificateLogDto>(txLog),
                                TransactionLogType.RedeemVaultCertificateLog => ctx.Mapper.Map<RedeemVaultCertificateLogDto>(txLog),
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

            CreateMap<CreateLiquidityPoolLog, CreateLiquidityPoolLogDto>()
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
                .ForMember(dest => dest.VaultAmount, opt => opt.MapFrom(src => src.VaultAmount))
                .ForMember(dest => dest.MiningAmount, opt => opt.MapFrom(src => src.MiningAmount))
                .ForMember(dest => dest.PeriodIndex, opt => opt.MapFrom(src => src.PeriodIndex));

            CreateMap<MineLog, MineLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Miner, opt => opt.MapFrom(src => src.Miner))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
                .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.EventType));

            CreateMap<StakeLog, StakeLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Staker, opt => opt.MapFrom(src => src.Staker))
                .ForMember(dest => dest.TotalStaked, opt => opt.MapFrom(src => src.TotalStaked))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.EventType));

            CreateMap<EnableMiningLog, EnableMiningLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.RewardRate, opt => opt.MapFrom(src => src.RewardRate))
                .ForMember(dest => dest.MiningPeriodEndBlock, opt => opt.MapFrom(src => src.MiningPeriodEndBlock));

            CreateMap<NominationLog, NominationLogDto>()
                .IncludeBase<TransactionLog, TransactionLogDto>()
                .ForMember(dest => dest.StakingPool, opt => opt.MapFrom(src => src.StakingPool))
                .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight));

            CreateMap<ChangeVaultOwnerLog, ChangeVaultOwnerLogDto>()
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