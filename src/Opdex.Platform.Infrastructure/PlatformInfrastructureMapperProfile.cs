using AutoMapper;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Admins;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernances;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Admins;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;

namespace Opdex.Platform.Infrastructure;

public class PlatformInfrastructureMapperProfile : Profile
{
    public PlatformInfrastructureMapperProfile()
    {
        CreateMap<AdminEntity, Admin>()
            .ConstructUsing(src => new Admin(src.Id, src.Address))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TransactionEntity, Transaction>()
            .ConstructUsing(src => new Transaction(src.Id, src.Hash, src.Block, src.GasUsed, src.From, src.To, src.Success, src.NewContractAddress))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TokenEntity, Token>()
            .ConstructUsing((src, ctx) => new Token(src.Id, src.Address, src.IsLpt, src.Name, src.Symbol, src.Decimals, src.Sats, src.TotalSupply, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TokenSummaryEntity, TokenSummary>()
            .ConstructUsing(src => new TokenSummary(src.Id, src.MarketId, src.TokenId, src.DailyPriceChangePercent, src.PriceUsd, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TokenSnapshotEntity, TokenSnapshot>()
            .ConstructUsing((src, ctx) => new TokenSnapshot(src.Id, src.TokenId, src.MarketId, ctx.Mapper.Map<Ohlc<decimal>>(src.Price),
                                                            (SnapshotType)src.SnapshotTypeId, src.StartDate, src.EndDate, src.ModifiedDate))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TokenDistributionEntity, TokenDistribution>()
            .ConstructUsing(src => new TokenDistribution(src.Id, src.TokenId, src.VaultDistribution, src.MiningGovernanceDistribution, src.PeriodIndex,
                                                         src.DistributionBlock, src.NextDistributionBlock, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TokenAttributeEntity, TokenAttribute>()
            .ConstructUsing(src => new TokenAttribute(src.Id, src.TokenId, (TokenAttributeType)src.AttributeTypeId))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<BlockEntity, Block>()
            .ConstructUsing(src => new Block(src.Height, src.Hash, src.Time, src.MedianTime))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<LiquidityPoolEntity, LiquidityPool>()
            .ConstructUsing(src => new LiquidityPool(src.Id, src.Address, src.Name, src.SrcTokenId, src.LpTokenId, src.MarketId, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<LiquidityPoolSummaryEntity, LiquidityPoolSummary>()
            .ConstructUsing(src => new LiquidityPoolSummary(src.Id, src.LiquidityPoolId, src.LiquidityUsd, src.DailyLiquidityUsdChangePercent,
                                                            src.VolumeUsd, src.StakingWeight, src.DailyStakingWeightChangePercent, src.LockedCrs,
                                                            src.LockedSrc, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<LiquidityPoolSnapshotEntity, LiquidityPoolSnapshot>()
            .ConstructUsing((src, ctx) => new LiquidityPoolSnapshot(src.Id, src.LiquidityPoolId, src.TransactionCount, ctx.Mapper.Map<ReservesSnapshot>(src.Reserves),
                                                                    ctx.Mapper.Map<RewardsSnapshot>(src.Rewards), ctx.Mapper.Map<StakingSnapshot>(src.Staking), ctx.Mapper.Map<VolumeSnapshot>(src.Volume),
                                                                    ctx.Mapper.Map<CostSnapshot>(src.Cost), (SnapshotType)src.SnapshotTypeId, src.StartDate, src.EndDate, src.ModifiedDate))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<SnapshotReservesEntity, ReservesSnapshot>()
            .ConstructUsing((src, ctx) => new ReservesSnapshot(ctx.Mapper.Map<Ohlc<ulong>>(src.Crs), ctx.Mapper.Map<Ohlc<UInt256>>(src.Src), ctx.Mapper.Map<Ohlc<decimal>>(src.Usd)))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<SnapshotRewardsEntity, RewardsSnapshot>()
            .ConstructUsing(src => new RewardsSnapshot(src.ProviderUsd, src.MarketUsd))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<SnapshotStakingEntity, StakingSnapshot>()
            .ConstructUsing((src, ctx) => new StakingSnapshot(ctx.Mapper.Map<Ohlc<UInt256>>(src.Weight), ctx.Mapper.Map<Ohlc<decimal>>(src.Usd)))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<SnapshotVolumeEntity, VolumeSnapshot>()
            .ConstructUsing(src => new VolumeSnapshot(src.Crs, src.Src, src.Usd))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<SnapshotCostEntity, CostSnapshot>()
            .ConstructUsing((src, ctx) => new CostSnapshot(ctx.Mapper.Map<Ohlc<UInt256>>(src.CrsPerSrc), ctx.Mapper.Map<Ohlc<UInt256>>(src.SrcPerCrs)))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<OhlcEntity<UInt256>, Ohlc<UInt256>>()
            .ConstructUsing(src => new Ohlc<UInt256>(src.Open, src.High, src.Low, src.Close))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<OhlcEntity<decimal>, Ohlc<decimal>>()
            .ConstructUsing(src => new Ohlc<decimal>(src.Open, src.High, src.Low, src.Close))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<OhlcEntity<ulong>, Ohlc<ulong>>()
            .ConstructUsing(src => new Ohlc<ulong>(src.Open, src.High, src.Low, src.Close))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MiningPoolEntity, MiningPool>()
            .ConstructUsing(src => new MiningPool(src.Id, src.LiquidityPoolId, src.Address, src.RewardPerBlock, src.RewardPerLpt, src.MiningPeriodEndBlock,
                                                  src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MarketEntity, Market>()
            .ConstructUsing(src => new Market(src.Id, src.Address, src.DeployerId, src.StakingTokenId, src.PendingOwner, src.Owner, src.AuthPoolCreators,
                                              src.AuthProviders, src.AuthTraders, src.TransactionFee, src.MarketFeeEnabled, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MarketSummaryEntity, MarketSummary>()
            .ConstructUsing(src => new MarketSummary(src.Id, src.MarketId, src.LiquidityUsd, src.DailyLiquidityUsdChangePercent, src.VolumeUsd, src.StakingWeight,
                                src.DailyStakingWeightChangePercent, src.StakingUsd, src.DailyStakingUsdChangePercent, src.ProviderRewardsDailyUsd, src.MarketRewardsDailyUsd,
                                src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MarketPermissionEntity, MarketPermission>()
            .ConstructUsing(src => new MarketPermission(src.Id, src.MarketId, src.User, (MarketPermissionType)src.Permission, src.IsAuthorized, src.Blame,
                                                        src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MarketRouterEntity, MarketRouter>()
            .ConstructUsing(src => new MarketRouter(src.Id, src.Address, src.MarketId, src.IsActive, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MiningGovernanceEntity, MiningGovernance>()
            .ConstructUsing(src => new MiningGovernance(src.Id, src.Address, src.TokenId, src.NominationPeriodEnd, src.MiningDuration,
                                                        src.MiningPoolsFunded, src.MiningPoolReward, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MiningGovernanceNominationEntity, MiningGovernanceNomination>()
            .ConstructUsing(src => new MiningGovernanceNomination(src.Id, src.MiningGovernanceId, src.LiquidityPoolId, src.MiningPoolId, src.IsNominated, src.Weight, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<DeployerEntity, Deployer>()
            .ConstructUsing(src => new Deployer(src.Id, src.Address, src.PendingOwner, src.Owner, src.IsActive, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MarketSnapshotEntity, MarketSnapshot>()
            .ConstructUsing((src, ctx) => new MarketSnapshot(src.Id, src.MarketId, ctx.Mapper.Map<Ohlc<decimal>>(src.LiquidityUsd), src.VolumeUsd,
                                                             ctx.Mapper.Map<StakingSnapshot>(src.Staking),
                                                             ctx.Mapper.Map<RewardsSnapshot>(src.Rewards),
                                                             (SnapshotType)src.SnapshotTypeId, src.StartDate, src.EndDate))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultCertificateEntity, VaultCertificate>()
            .ConstructUsing(src => new VaultCertificate(src.Id, src.VaultId, src.Owner, src.Amount, src.VestedBlock, src.Redeemed, src.Revoked,
                                                        src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultProposalCertificateEntity, VaultProposalCertificate>()
            .ConstructUsing(src => new VaultProposalCertificate(src.Id, src.ProposalId, src.CertificateId, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<AddressBalanceEntity, AddressBalance>()
            .ConstructUsing(src => new AddressBalance(src.Id, src.TokenId, src.Owner, src.Balance, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<AddressMiningEntity, AddressMining>()
            .ConstructUsing(src => new AddressMining(src.Id, src.MiningPoolId, src.Owner, src.Balance, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<AddressStakingEntity, AddressStaking>()
            .ConstructUsing(src => new AddressStaking(src.Id, src.LiquidityPoolId, src.Owner, src.Weight, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultProposalEntity, VaultProposal>()
            .ConstructUsing(src => new VaultProposal(src.Id, src.PublicId, src.VaultId, src.Creator, src.Wallet, src.Amount, src.Description,
                                                     (VaultProposalType)src.ProposalTypeId, (VaultProposalStatus)src.ProposalStatusId, src.Expiration,
                                                     src.YesAmount, src.NoAmount, src.PledgeAmount, src.Approved, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultProposalPledgeEntity, VaultProposalPledge>()
            .ConstructUsing(src => new VaultProposalPledge(src.Id, src.VaultId, src.ProposalId, src.Pledger, src.Pledge, src.Balance,
                                                           src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultProposalVoteEntity, VaultProposalVote>()
            .ConstructUsing(src => new VaultProposalVote(src.Id, src.VaultId, src.ProposalId, src.Voter, src.Vote, src.Balance, src.InFavor,
                                                         src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultGovernanceEntity, VaultGovernance>()
            .ConstructUsing(src => new VaultGovernance(src.Id, src.Address, src.TokenId, src.UnassignedSupply, src.VestingDuration, src.ProposedSupply,
                                                       src.TotalPledgeMinimum, src.TotalVoteMinimum, src.CreatedBlock, src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TransactionLogEntity, TransactionLog>()
            .ConstructUsing((src, ctx) =>
            {
                return src.LogTypeId switch
                {
                    // Deployers
                    (int)TransactionLogType.CreateMarketLog => new CreateMarketLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.SetPendingDeployerOwnershipLog => new SetPendingDeployerOwnershipLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.ClaimPendingDeployerOwnershipLog => new ClaimPendingDeployerOwnershipLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),

                    // Markets
                    (int)TransactionLogType.CreateLiquidityPoolLog => new CreateLiquidityPoolLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.ChangeMarketPermissionLog => new ChangeMarketPermissionLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.SetPendingMarketOwnershipLog => new SetPendingMarketOwnershipLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.ClaimPendingMarketOwnershipLog => new ClaimPendingMarketOwnershipLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),

                    // Liquidity Pools
                    (int)TransactionLogType.ReservesLog => new ReservesLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.BurnLog => new BurnLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.MintLog => new MintLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.SwapLog => new SwapLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.CollectStakingRewardsLog => new CollectStakingRewardsLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.StartStakingLog => new StartStakingLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.StopStakingLog => new StopStakingLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),

                    // Mining Pools
                    (int)TransactionLogType.StartMiningLog => new StartMiningLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.StopMiningLog => new StopMiningLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.CollectMiningRewardsLog => new CollectMiningRewardsLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.EnableMiningLog => new EnableMiningLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),

                    // Tokens
                    (int)TransactionLogType.ApprovalLog => new ApprovalLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.TransferLog => new TransferLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.DistributionLog => new DistributionLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),

                    // Mining Governances
                    (int)TransactionLogType.RewardMiningPoolLog => new RewardMiningPoolLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.NominationLog => new NominationLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),

                    // Vault
                    (int)TransactionLogType.CreateVaultCertificateLog => new CreateVaultCertificateLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.RevokeVaultCertificateLog => new RevokeVaultCertificateLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.RedeemVaultCertificateLog => new RedeemVaultCertificateLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),

                    // Vault Governance
                    (int)TransactionLogType.CompleteVaultProposalLog => new CompleteVaultProposalLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.CreateVaultProposalLog => new CreateVaultProposalLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.VaultProposalPledgeLog => new VaultProposalPledgeLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.VaultProposalWithdrawPledgeLog => new VaultProposalWithdrawPledgeLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.VaultProposalVoteLog => new VaultProposalVoteLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                    (int)TransactionLogType.VaultProposalWithdrawVoteLog => new VaultProposalWithdrawVoteLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),

                    // Else
                    _ => null
                };
            })
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TransactionLogSummaryDto, TransactionLog>()
            .ConstructUsing((src, ctx) =>
            {
                return src.Log.Event switch
                {
                    // Deployers
                    nameof(SetPendingDeployerOwnershipLog) => new SetPendingDeployerOwnershipLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(ClaimPendingDeployerOwnershipLog) => new ClaimPendingDeployerOwnershipLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(CreateMarketLog) => new CreateMarketLog(src.Log.Data, src.Address, src.SortOrder),

                    // Markets
                    nameof(SetPendingMarketOwnershipLog) => new SetPendingMarketOwnershipLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(ClaimPendingMarketOwnershipLog) => new ClaimPendingMarketOwnershipLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(ChangeMarketPermissionLog) => new ChangeMarketPermissionLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(CreateLiquidityPoolLog) => new CreateLiquidityPoolLog(src.Log.Data, src.Address, src.SortOrder),

                    // Liquidity Pools
                    nameof(ReservesLog) => new ReservesLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(BurnLog) => new BurnLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(MintLog) => new MintLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(SwapLog) => new SwapLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(StartStakingLog) => new StartStakingLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(StopStakingLog) => new StopStakingLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(CollectStakingRewardsLog) => new CollectStakingRewardsLog(src.Log.Data, src.Address, src.SortOrder),

                    // Mining Pools
                    nameof(StartMiningLog) => new StartMiningLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(StopMiningLog) => new StopMiningLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(CollectMiningRewardsLog) => new CollectMiningRewardsLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(EnableMiningLog) => new EnableMiningLog(src.Log.Data, src.Address, src.SortOrder),

                    // Tokens
                    nameof(ApprovalLog) => new ApprovalLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(TransferLog) => new TransferLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(DistributionLog) => new DistributionLog(src.Log.Data, src.Address, src.SortOrder),

                    // Mining Governances
                    nameof(NominationLog) => new NominationLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(RewardMiningPoolLog) => new RewardMiningPoolLog(src.Log.Data, src.Address, src.SortOrder),

                    // Vaults
                    nameof(CreateVaultCertificateLog) => new CreateVaultCertificateLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(RevokeVaultCertificateLog) => new RevokeVaultCertificateLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(RedeemVaultCertificateLog) => new RedeemVaultCertificateLog(src.Log.Data, src.Address, src.SortOrder),

                    // Vault Governance
                    nameof(CompleteVaultProposalLog) => new CompleteVaultProposalLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(CreateVaultProposalLog) => new CreateVaultProposalLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(VaultProposalPledgeLog) => new VaultProposalPledgeLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(VaultProposalWithdrawPledgeLog) => new VaultProposalWithdrawPledgeLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(VaultProposalVoteLog) => new VaultProposalVoteLog(src.Log.Data, src.Address, src.SortOrder),
                    nameof(VaultProposalWithdrawVoteLog) => new VaultProposalWithdrawVoteLog(src.Log.Data, src.Address, src.SortOrder),

                    // Else
                    _ => null
                };
            });

        CreateMap<Admin, AdminEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<Market, MarketEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.AuthPoolCreators, opt => opt.MapFrom(src => src.AuthPoolCreators))
            .ForMember(dest => dest.AuthProviders, opt => opt.MapFrom(src => src.AuthProviders))
            .ForMember(dest => dest.AuthTraders, opt => opt.MapFrom(src => src.AuthTraders))
            .ForMember(dest => dest.TransactionFee, opt => opt.MapFrom(src => src.TransactionFee))
            .ForMember(dest => dest.StakingTokenId, opt => opt.MapFrom(src => src.StakingTokenId))
            .ForMember(dest => dest.PendingOwner, opt => opt.MapFrom(src => src.PendingOwner))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.DeployerId, opt => opt.MapFrom(src => src.DeployerId))
            .ForMember(dest => dest.MarketFeeEnabled, opt => opt.MapFrom(src => src.MarketFeeEnabled))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MarketSummary, MarketSummaryEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
            .ForMember(dest => dest.LiquidityUsd, opt => opt.MapFrom(src => src.LiquidityUsd))
            .ForMember(dest => dest.DailyLiquidityUsdChangePercent, opt => opt.MapFrom(src => src.DailyLiquidityUsdChangePercent))
            .ForMember(dest => dest.VolumeUsd, opt => opt.MapFrom(src => src.VolumeUsd))
            .ForMember(dest => dest.StakingWeight, opt => opt.MapFrom(src => src.StakingWeight))
            .ForMember(dest => dest.DailyStakingWeightChangePercent, opt => opt.MapFrom(src => src.DailyStakingWeightChangePercent))
            .ForMember(dest => dest.StakingUsd, opt => opt.MapFrom(src => (ulong)src.StakingUsd))
            .ForMember(dest => dest.DailyStakingUsdChangePercent, opt => opt.MapFrom(src => src.DailyStakingUsdChangePercent))
            .ForMember(dest => dest.ProviderRewardsDailyUsd, opt => opt.MapFrom(src => src.ProviderRewardsDailyUsd))
            .ForMember(dest => dest.MarketRewardsDailyUsd, opt => opt.MapFrom(src => src.MarketRewardsDailyUsd))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MarketPermission, MarketPermissionEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Permission, opt => opt.MapFrom(src => (int)src.Permission))
            .ForMember(dest => dest.IsAuthorized, opt => opt.MapFrom(src => src.IsAuthorized))
            .ForMember(dest => dest.Blame, opt => opt.MapFrom(src => src.Blame))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MarketRouter, MarketRouterEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MarketSnapshot, MarketSnapshotEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
            .ForMember(dest => dest.LiquidityUsd, opt => opt.MapFrom(src => src.LiquidityUsd))
            .ForMember(dest => dest.VolumeUsd, opt => opt.MapFrom(src => src.VolumeUsd))
            .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
            .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.Rewards))
            .ForMember(dest => dest.SnapshotTypeId, opt => opt.MapFrom(src => (int)src.SnapshotType))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<Deployer, DeployerEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.PendingOwner, opt => opt.MapFrom(src => src.PendingOwner))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MiningGovernance, MiningGovernanceEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
            .ForMember(dest => dest.NominationPeriodEnd, opt => opt.MapFrom(src => src.NominationPeriodEnd))
            .ForMember(dest => dest.MiningDuration, opt => opt.MapFrom(src => src.MiningDuration))
            .ForMember(dest => dest.MiningPoolsFunded, opt => opt.MapFrom(src => src.MiningPoolsFunded))
            .ForMember(dest => dest.MiningPoolReward, opt => opt.MapFrom(src => src.MiningPoolReward))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MiningGovernanceNomination, MiningGovernanceNominationEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MiningGovernanceId, opt => opt.MapFrom(src => src.MiningGovernanceId))
            .ForMember(dest => dest.LiquidityPoolId, opt => opt.MapFrom(src => src.LiquidityPoolId))
            .ForMember(dest => dest.MiningPoolId, opt => opt.MapFrom(src => src.MiningPoolId))
            .ForMember(dest => dest.IsNominated, opt => opt.MapFrom(src => src.IsNominated))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<Token, TokenEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.IsLpt, opt => opt.MapFrom(src => src.IsLpt))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
            .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Decimals))
            .ForMember(dest => dest.Sats, opt => opt.MapFrom(src => src.Sats))
            .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TokenSummary, TokenSummaryEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
            .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
            .ForMember(dest => dest.DailyPriceChangePercent, opt => opt.MapFrom(src => src.DailyPriceChangePercent))
            .ForMember(dest => dest.PriceUsd, opt => opt.MapFrom(src => src.PriceUsd))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TokenSnapshot, TokenSnapshotEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
            .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.SnapshotTypeId, opt => opt.MapFrom(src => (int)src.SnapshotType))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TokenDistribution, TokenDistributionEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
            .ForMember(dest => dest.VaultDistribution, opt => opt.MapFrom(src => src.VaultDistribution))
            .ForMember(dest => dest.MiningGovernanceDistribution, opt => opt.MapFrom(src => src.MiningGovernanceDistribution))
            .ForMember(dest => dest.NextDistributionBlock, opt => opt.MapFrom(src => src.NextDistributionBlock))
            .ForMember(dest => dest.DistributionBlock, opt => opt.MapFrom(src => src.DistributionBlock))
            .ForMember(dest => dest.PeriodIndex, opt => opt.MapFrom(src => src.PeriodIndex))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TokenAttribute, TokenAttributeEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
            .ForMember(dest => dest.AttributeTypeId, opt => opt.MapFrom(src => (uint)src.AttributeType))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<LiquidityPool, LiquidityPoolEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.SrcTokenId, opt => opt.MapFrom(src => src.SrcTokenId))
            .ForMember(dest => dest.LpTokenId, opt => opt.MapFrom(src => src.LpTokenId))
            .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<LiquidityPoolSummary, LiquidityPoolSummaryEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.LiquidityPoolId, opt => opt.MapFrom(src => src.LiquidityPoolId))
            .ForMember(dest => dest.LiquidityUsd, opt => opt.MapFrom(src => src.LiquidityUsd))
            .ForMember(dest => dest.DailyLiquidityUsdChangePercent, opt => opt.MapFrom(src => src.DailyLiquidityUsdChangePercent))
            .ForMember(dest => dest.VolumeUsd, opt => opt.MapFrom(src => src.VolumeUsd))
            .ForMember(dest => dest.StakingWeight, opt => opt.MapFrom(src => src.StakingWeight))
            .ForMember(dest => dest.DailyStakingWeightChangePercent, opt => opt.MapFrom(src => src.DailyStakingWeightChangePercent))
            .ForMember(dest => dest.LockedSrc, opt => opt.MapFrom(src => src.LockedSrc))
            .ForMember(dest => dest.LockedCrs, opt => opt.MapFrom(src => src.LockedCrs))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<LiquidityPoolSnapshot, LiquidityPoolSnapshotEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.LiquidityPoolId, opt => opt.MapFrom(src => src.LiquidityPoolId))
            .ForMember(dest => dest.SnapshotTypeId, opt => opt.MapFrom(src => (int)src.SnapshotType))
            .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.TransactionCount))
            .ForMember(dest => dest.Reserves, opt => opt.MapFrom(src => src.Reserves))
            .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
            .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
            .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
            .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.Rewards))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<ReservesSnapshot, SnapshotReservesEntity>()
            .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.Crs))
            .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Src))
            .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<StakingSnapshot, SnapshotStakingEntity>()
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VolumeSnapshot, SnapshotVolumeEntity>()
            .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.Crs))
            .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Src))
            .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<RewardsSnapshot, SnapshotRewardsEntity>()
            .ForMember(dest => dest.ProviderUsd, opt => opt.MapFrom(src => src.ProviderUsd))
            .ForMember(dest => dest.MarketUsd, opt => opt.MapFrom(src => src.MarketUsd))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<CostSnapshot, SnapshotCostEntity>()
            .ForMember(dest => dest.CrsPerSrc, opt => opt.MapFrom(src => src.CrsPerSrc))
            .ForMember(dest => dest.SrcPerCrs, opt => opt.MapFrom(src => src.SrcPerCrs))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<Ohlc<ulong>, OhlcEntity<ulong>>()
            .ForMember(dest => dest.Open, opt => opt.MapFrom(src => src.Open))
            .ForMember(dest => dest.High, opt => opt.MapFrom(src => src.High))
            .ForMember(dest => dest.Low, opt => opt.MapFrom(src => src.Low))
            .ForMember(dest => dest.Close, opt => opt.MapFrom(src => src.Close))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<Ohlc<UInt256>, OhlcEntity<UInt256>>()
            .ForMember(dest => dest.Open, opt => opt.MapFrom(src => src.Open))
            .ForMember(dest => dest.High, opt => opt.MapFrom(src => src.High))
            .ForMember(dest => dest.Low, opt => opt.MapFrom(src => src.Low))
            .ForMember(dest => dest.Close, opt => opt.MapFrom(src => src.Close))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<Ohlc<decimal>, OhlcEntity<decimal>>()
            .ForMember(dest => dest.Open, opt => opt.MapFrom(src => src.Open))
            .ForMember(dest => dest.High, opt => opt.MapFrom(src => src.High))
            .ForMember(dest => dest.Low, opt => opt.MapFrom(src => src.Low))
            .ForMember(dest => dest.Close, opt => opt.MapFrom(src => src.Close))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<MiningPool, MiningPoolEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.LiquidityPoolId, opt => opt.MapFrom(src => src.LiquidityPoolId))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.RewardPerBlock, opt => opt.MapFrom(src => src.RewardPerBlock))
            .ForMember(dest => dest.RewardPerLpt, opt => opt.MapFrom(src => src.RewardPerLpt))
            .ForMember(dest => dest.MiningPeriodEndBlock, opt => opt.MapFrom(src => src.MiningPeriodEndBlock))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
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
            .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
            .ForMember(dest => dest.NewContractAddress, opt => opt.MapFrom(src => src.NewContractAddress))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<TransactionLog, TransactionLogEntity>()
            .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
            .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.SerializeLogDetails()))
            .ForMember(dest => dest.LogTypeId, opt => opt.MapFrom(src => (int)src.LogType))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultProposalCertificate, VaultProposalCertificateEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.CertificateId, opt => opt.MapFrom(src => src.CertificateId))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultCertificate, VaultCertificateEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.VaultId, opt => opt.MapFrom(src => src.VaultId))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.VestedBlock, opt => opt.MapFrom(src => src.VestedBlock))
            .ForMember(dest => dest.Redeemed, opt => opt.MapFrom(src => src.Redeemed))
            .ForMember(dest => dest.Revoked, opt => opt.MapFrom(src => src.Revoked))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<AddressBalance, AddressBalanceEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<AddressMining, AddressMiningEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MiningPoolId, opt => opt.MapFrom(src => src.MiningPoolId))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<AddressStaking, AddressStakingEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.LiquidityPoolId, opt => opt.MapFrom(src => src.LiquidityPoolId))
            .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultProposal, VaultProposalEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PublicId, opt => opt.MapFrom(src => src.PublicId))
            .ForMember(dest => dest.VaultId, opt => opt.MapFrom(src => src.VaultId))
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator))
            .ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.Wallet))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ProposalTypeId, opt => opt.MapFrom(src => (byte)src.Type))
            .ForMember(dest => dest.ProposalStatusId, opt => opt.MapFrom(src => (byte)src.Status))
            .ForMember(dest => dest.Expiration, opt => opt.MapFrom(src => src.Expiration))
            .ForMember(dest => dest.YesAmount, opt => opt.MapFrom(src => src.YesAmount))
            .ForMember(dest => dest.NoAmount, opt => opt.MapFrom(src => src.NoAmount))
            .ForMember(dest => dest.PledgeAmount, opt => opt.MapFrom(src => src.PledgeAmount))
            .ForMember(dest => dest.Approved, opt => opt.MapFrom(src => src.Approved))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultProposalPledge, VaultProposalPledgeEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.VaultId, opt => opt.MapFrom(src => src.VaultId))
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.Pledger, opt => opt.MapFrom(src => src.Pledger))
            .ForMember(dest => dest.Pledge, opt => opt.MapFrom(src => src.Pledge))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultProposalVote, VaultProposalVoteEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.VaultId, opt => opt.MapFrom(src => src.VaultId))
            .ForMember(dest => dest.ProposalId, opt => opt.MapFrom(src => src.ProposalId))
            .ForMember(dest => dest.Voter, opt => opt.MapFrom(src => src.Voter))
            .ForMember(dest => dest.Vote, opt => opt.MapFrom(src => src.Vote))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.InFavor, opt => opt.MapFrom(src => src.InFavor))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<VaultGovernance, VaultGovernanceEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
            .ForMember(dest => dest.UnassignedSupply, opt => opt.MapFrom(src => src.UnassignedSupply))
            .ForMember(dest => dest.VestingDuration, opt => opt.MapFrom(src => src.VestingDuration))
            .ForMember(dest => dest.ProposedSupply, opt => opt.MapFrom(src => src.ProposedSupply))
            .ForMember(dest => dest.TotalPledgeMinimum, opt => opt.MapFrom(src => src.TotalPledgeMinimum))
            .ForMember(dest => dest.TotalVoteMinimum, opt => opt.MapFrom(src => src.TotalVoteMinimum))
            .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
            .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
            .ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<Interval, SnapshotType>()
            .ConvertUsing((src, ctx) =>
            {
                return src switch
                {
                    Interval.OneDay => SnapshotType.Daily,
                    Interval.OneHour => SnapshotType.Hourly,
                    _ => throw new ArgumentOutOfRangeException("Interval", "Invalid Interval to Snapshot type")
                };
            });
    }
}
