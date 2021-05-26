using AutoMapper;
using Opdex.Platform.Common;
using Opdex.Platform.Domain;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;

namespace Opdex.Platform.Infrastructure
{
    public class PlatformInfrastructureMapperProfile : Profile
    {
        public PlatformInfrastructureMapperProfile()
        {
            CreateMap<TransactionReceiptDto, Transaction>()
                .ConstructUsing(src => new Transaction(src.TransactionHash, src.BlockHeight, src.GasUsed, src.From, src.To, src.Success, src.NewContractAddress))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TransactionEntity, Transaction>()
                .ConstructUsing(src => new Transaction(src.Id, src.Hash, src.Block, src.GasUsed, src.From, src.To, src.Success, new TransactionLog[0], src.NewContractAddress))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TokenEntity, Token>()
                .ConstructUsing(src => new Token(src.Id, src.Address, src.Name, src.Symbol, src.Decimals, src.Sats, src.TotalSupply, src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TokenSnapshotEntity, TokenSnapshot>()
                .ConstructUsing(src => new TokenSnapshot(src.Id, src.TokenId, src.MarketId, src.Price, src.SnapshotTypeId, src.StartDate, src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<TokenDistributionEntity, TokenDistribution>()
                .ConstructUsing(src => new TokenDistribution(src.Id, src.VaultDistribution, src.MiningGovernanceDistribution, src.PeriodIndex, 
                    src.DistributionBlock, src.NextDistributionBlock, src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<BlockEntity, Block>()
                .ConstructUsing(src => new Block(src.Height, src.Hash, src.Time, src.MedianTime))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<LiquidityPoolEntity, LiquidityPool>()
                .ConstructUsing(src => new LiquidityPool(src.Id, src.Address, src.TokenId, src.MarketId, src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<LiquidityPoolSnapshotEntity, LiquidityPoolSnapshot>()
                .ConstructUsing(src => new LiquidityPoolSnapshot(src.Id, src.LiquidityPoolId, src.TransactionCount, src.ReserveCrs, src.ReserveSrc, 
                    src.ReserveUsd, src.VolumeCrs, src.VolumeSrc, src.VolumeUsd, src.StakingWeight, src.StakingUsd, src.ProviderRewards, src.StakerRewards, 
                    (SnapshotType)src.SnapshotTypeId, src.StartDate, src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MiningPoolEntity, MiningPool>()
                .ConstructUsing(src => new MiningPool(src.Id, src.LiquidityPoolId, src.Address, src.RewardPerBlock, src.RewardPerLpt, src.MiningPeriodEndBlock,
                    src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MiningPoolSnapshotEntity, MiningPoolSnapshot>()
                .ConstructUsing(src => new MiningPoolSnapshot(src.Id, src.MiningPoolId, src.MiningWeight, src.MiningUsd, src.RemainingRewards, 
                    (SnapshotType)src.SnapshotTypeId, src.StartDate, src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketEntity, Market>()
                .ConstructUsing(src => new Market(src.Id, src.Address, src.DeployerId, src.StakingTokenId, src.Owner, src.AuthPoolCreators, 
                    src.AuthProviders, src.AuthTraders, src.Fee, src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MiningGovernanceEntity, MiningGovernance>()
                .ConstructUsing(src => new MiningGovernance(src.Id, src.Address, src.TokenId, src.NominationPeriodEnd, src.MiningPoolsFunded, src.MiningPoolReward,
                    src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MiningGovernanceNominationEntity, MiningGovernanceNomination>()
                .ConstructUsing(src => new MiningGovernanceNomination(src.Id, src.LiquidityPoolId, src.MiningPoolId, src.IsNominated, src.Weight, src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<DeployerEntity, Deployer>()
                .ConstructUsing(src => new Deployer(src.Id, src.Address, src.Owner, src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MarketSnapshotEntity, MarketSnapshot>()
                .ConstructUsing(src => new MarketSnapshot(src.Id, src.MarketId, src.TransactionCount, src.Liquidity, src.Volume, src.StakingWeight, src.StakingUsd, 
                    src.ProviderRewards, src.StakerRewards, (SnapshotType)src.SnapshotTypeId, src.StartDate, src.EndDate))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<VaultEntity, Vault>()
                .ConstructUsing(src => new Vault(src.Id, src.Address, src.TokenId, src.Owner, src.Genesis, src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<VaultCertificateEntity, VaultCertificate>()
                .ConstructUsing(src => new VaultCertificate(src.Id, src.VaultId, src.Owner, src.Amount, src.VestedBlock, src.Redeemed, src.Revoked, src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<AddressBalanceEntity, AddressBalance>()
                .ConstructUsing(src => new AddressBalance(src.Id, src.TokenId, src.LiquidityPoolId, src.Owner, src.Balance, src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<AddressAllowanceEntity, AddressAllowance>()
                .ConstructUsing(src => new AddressAllowance(src.Id, src.TokenId, src.LiquidityPoolId, src.Owner, src.Spender, src.Allowance, src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<AddressMiningEntity, AddressMining>()
                .ConstructUsing(src => new AddressMining(src.Id, src.MiningPoolId, src.Owner, src.Balance, src.CreatedBlock, src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<AddressStakingEntity, AddressStaking>()
                .ConstructUsing(src => new AddressStaking(src.Id, src.LiquidityPoolId, src.Owner, src.Weight, src.CreatedBlock, src.ModifiedBlock))
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
                        (int)TransactionLogType.CreateLiquidityPoolLog => new CreateLiquidityPoolLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.CreateMiningPoolLog => new CreateMiningPoolLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.StartStakingLog => new StartStakingLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.StartMiningLog => new StartMiningLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.CollectStakingRewardsLog => new CollectStakingRewardsLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.CollectMiningRewardsLog => new CollectMiningRewardsLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.StopStakingLog => new StopStakingLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.StopMiningLog => new StopMiningLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.RewardMiningPoolLog => new RewardMiningPoolLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.EnableMiningLog => new EnableMiningLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.NominationLog => new NominationLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.ChangeVaultOwnerLog => new ChangeVaultOwnerLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.DistributionLog => new DistributionLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.CreateMarketLog => new CreateMarketLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.ChangeMarketOwnerLog => new ChangeMarketOwnerLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.ChangeMarketPermissionLog => new ChangeMarketPermissionLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
                        (int)TransactionLogType.ChangeMarketLog => new ChangeMarketLog(src.Id, src.TransactionId, src.Contract, src.SortOrder, src.Details),
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
                .ForMember(dest => dest.StakingTokenId, opt => opt.MapFrom(src => src.StakingTokenId))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.DeployerId, opt => opt.MapFrom(src => src.DeployerId))
                .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
                .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
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
            
            CreateMap<Deployer, DeployerEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
                .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MiningGovernance, MiningGovernanceEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
                .ForMember(dest => dest.NominationPeriodEnd, opt => opt.MapFrom(src => src.NominationPeriodEnd))
                .ForMember(dest => dest.MiningPoolsFunded, opt => opt.MapFrom(src => src.MiningPoolsFunded))
                .ForMember(dest => dest.MiningPoolReward, opt => opt.MapFrom(src => src.MiningPoolReward))
                .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
                .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MiningGovernanceNomination, MiningGovernanceNominationEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
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
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Decimals))
                .ForMember(dest => dest.Sats, opt => opt.MapFrom(src => src.Sats))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
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
                .ForMember(dest => dest.VaultDistribution, opt => opt.MapFrom(src => src.VaultDistribution))
                .ForMember(dest => dest.MiningGovernanceDistribution, opt => opt.MapFrom(src => src.MiningGovernanceDistribution))
                .ForMember(dest => dest.NextDistributionBlock, opt => opt.MapFrom(src => src.NextDistributionBlock))
                .ForMember(dest => dest.DistributionBlock, opt => opt.MapFrom(src => src.DistributionBlock))
                .ForMember(dest => dest.PeriodIndex, opt => opt.MapFrom(src => src.PeriodIndex))
                .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
                .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<LiquidityPool, LiquidityPoolEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
                .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
                .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
                .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
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
                .ForMember(dest => dest.RewardPerBlock, opt => opt.MapFrom(src => src.RewardPerBlock))
                .ForMember(dest => dest.RewardPerLpt, opt => opt.MapFrom(src => src.RewardPerLpt))
                .ForMember(dest => dest.MiningPeriodEndBlock, opt => opt.MapFrom(src => src.MiningPeriodEndBlock))
                .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
                .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<MiningPoolSnapshot, MiningPoolSnapshotEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MiningPoolId, opt => opt.MapFrom(src => src.MiningPoolId))
                .ForMember(dest => dest.MiningWeight, opt => opt.MapFrom(src => src.MiningWeight))
                .ForMember(dest => dest.MiningUsd, opt => opt.MapFrom(src => src.MiningUsd))
                .ForMember(dest => dest.RemainingRewards, opt => opt.MapFrom(src => src.RemainingRewards))
                .ForMember(dest => dest.SnapshotTypeId, opt => opt.MapFrom(src => (int)src.SnapshotType))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
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

            CreateMap<PersistTransactionLogCommand, TransactionLogEntity>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details))
                .ForMember(dest => dest.LogTypeId, opt => opt.MapFrom(src => src.LogTypeId))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<Vault, VaultEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Genesis, opt => opt.MapFrom(src => src.Genesis))
                .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
                .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<VaultCertificate, VaultCertificateEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
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
                .ForMember(dest => dest.LiquidityPoolId, opt => opt.MapFrom(src => src.LiquidityPoolId))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
                .ForMember(dest => dest.CreatedBlock, opt => opt.MapFrom(src => src.CreatedBlock))
                .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());
            
            CreateMap<AddressAllowance, AddressAllowanceEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
                .ForMember(dest => dest.LiquidityPoolId, opt => opt.MapFrom(src => src.LiquidityPoolId))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Spender, opt => opt.MapFrom(src => src.Spender))
                .ForMember(dest => dest.Allowance, opt => opt.MapFrom(src => src.Allowance))
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
        }
    }
}