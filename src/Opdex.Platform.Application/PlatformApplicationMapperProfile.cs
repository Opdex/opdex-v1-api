using AutoMapper;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Admins;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Application.Abstractions.Models.OHLC;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.OHLC;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Admins;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernances;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Domain.Models.Vaults;
using System.Linq;

namespace Opdex.Platform.Application
{
    public class PlatformApplicationMapperProfile : Profile
    {
        public PlatformApplicationMapperProfile()
        {
            CreateMap<Admin, AdminDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
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
                .ForMember(dest => dest.PendingOwner, opt => opt.MapFrom(src => src.PendingOwner))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.AuthPoolCreators, opt => opt.MapFrom(src => src.AuthPoolCreators))
                .ForMember(dest => dest.AuthProviders, opt => opt.MapFrom(src => src.AuthProviders))
                .ForMember(dest => dest.AuthTraders, opt => opt.MapFrom(src => src.AuthTraders))
                .ForMember(dest => dest.MarketFeeEnabled, opt => opt.MapFrom(src => src.MarketFeeEnabled))
                .ForMember(dest => dest.TransactionFee, opt => opt.MapFrom(src => src.TransactionFee))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TokenSummary, TokenSummaryDto>()
                .ForMember(dest => dest.PriceUsd, opt => opt.MapFrom(src => src.PriceUsd))
                .ForMember(dest => dest.DailyPriceChangePercent, opt => opt.MapFrom(src => src.DailyPriceChangePercent))
                .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Token, TokenDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Decimals))
                .ForMember(dest => dest.Sats, opt => opt.MapFrom(src => src.Sats))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply.ToDecimal(src.Decimals)))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketToken, MarketTokenDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Decimals))
                .ForMember(dest => dest.Sats, opt => opt.MapFrom(src => src.Sats))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply.ToDecimal(src.Decimals)))
                .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.Market.Address))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPool, LiquidityPoolDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Block, BlockDto>()
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.MedianTime, opt => opt.MapFrom(src => src.MedianTime))
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
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.StartDate))
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

            CreateMap<TokenSnapshot, TokenSnapshotDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.StartDate))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketSnapshot, MarketSnapshotDto>()
                .ForMember(dest => dest.MarketId, opt => opt.MapFrom(src => src.MarketId))
                .ForMember(dest => dest.Liquidity, opt => opt.MapFrom(src => src.Liquidity))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.Rewards))
                .ForMember(dest => dest.SnapshotType, opt => opt.MapFrom(src => src.SnapshotType))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.StartDate))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<AddressAllowance, AddressAllowanceDto>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Spender, opt => opt.MapFrom(src => src.Spender))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Vault, VaultDto>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PendingOwner, opt => opt.MapFrom(src => src.PendingOwner))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Genesis, opt => opt.MapFrom(src => src.Genesis))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<VaultCertificate, VaultCertificateDto>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.VestingStartBlock, opt => opt.MapFrom(src => src.CreatedBlock))
                .ForMember(dest => dest.VestingEndBlock, opt => opt.MapFrom(src => src.VestedBlock))
                .ForMember(dest => dest.Redeemed, opt => opt.MapFrom(src => src.Redeemed))
                .ForMember(dest => dest.Revoked, opt => opt.MapFrom(src => src.Revoked));

            CreateMap<AddressBalance, AddressBalanceDto>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Owner))
                .ForAllOtherMembers(opt => opt.Ignore());

            // Transactions and Transaction Events

            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.GasUsed, opt => opt.MapFrom(src => src.GasUsed))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.NewContractAddress, opt => opt.MapFrom(src => src.NewContractAddress))
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder));

            CreateMap<OwnershipLog, OwnershipEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To));

            // Deployer Events
            CreateMap<SetPendingDeployerOwnershipLog, SetPendingDeployerOwnershipEventDto>()
                .IncludeBase<OwnershipLog, OwnershipEventDto>();

            CreateMap<ClaimPendingDeployerOwnershipLog, ClaimPendingDeployerOwnershipEventDto>()
                .IncludeBase<OwnershipLog, OwnershipEventDto>();

            CreateMap<CreateMarketLog, CreateMarketEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.Market))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Router, opt => opt.MapFrom(src => src.Router))
                .ForMember(dest => dest.AuthPoolCreators, opt => opt.MapFrom(src => src.AuthPoolCreators))
                .ForMember(dest => dest.AuthProviders, opt => opt.MapFrom(src => src.AuthProviders))
                .ForMember(dest => dest.AuthTraders, opt => opt.MapFrom(src => src.AuthTraders))
                .ForMember(dest => dest.TransactionFee, opt => opt.MapFrom(src => src.TransactionFee))
                .ForMember(dest => dest.StakingToken, opt => opt.MapFrom(src => src.StakingToken))
                .ForMember(dest => dest.EnableMarketFee, opt => opt.MapFrom(src => src.EnableMarketFee));

            // Market Events
            CreateMap<SetPendingMarketOwnershipLog, SetPendingMarketOwnershipEventDto>()
                .IncludeBase<OwnershipLog, OwnershipEventDto>();

            CreateMap<ClaimPendingMarketOwnershipLog, ClaimPendingMarketOwnershipEventDto>()
                .IncludeBase<OwnershipLog, OwnershipEventDto>();

            CreateMap<ChangeMarketPermissionLog, ChangeMarketPermissionEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Permission, opt => opt.MapFrom(src => src.Permission.ToString()))
                .ForMember(dest => dest.IsAuthorized, opt => opt.MapFrom(src => src.IsAuthorized));

            CreateMap<CreateLiquidityPoolLog, CreateLiquidityPoolEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.LiquidityPool, opt => opt.MapFrom(src => src.Pool))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token));

            // Liquidity Pool Events
            CreateMap<CollectStakingRewardsLog, CollectStakingRewardsEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.Staker, opt => opt.MapFrom(src => src.Staker))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToDecimal(TokenConstants.LiquidityPoolToken.Decimals)));

            CreateMap<StakeLog, StakeEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.Staker, opt => opt.MapFrom(src => src.Staker))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.TotalStaked, opt => opt.MapFrom(src => src.TotalStaked.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.StakerBalance, opt => opt.MapFrom(src => src.StakerBalance.ToDecimal(TokenConstants.Opdex.Decimals)));

            CreateMap<StartStakingLog, StartStakingEventDto>()
                .IncludeBase<StakeLog, StakeEventDto>();

            CreateMap<StopStakingLog, StopStakingEventDto>()
                .IncludeBase<StakeLog, StakeEventDto>();

            // Mining Pool Events
            CreateMap<MineLog, MineEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.Miner, opt => opt.MapFrom(src => src.Miner))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToDecimal(TokenConstants.LiquidityPoolToken.Decimals)))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply.ToDecimal(TokenConstants.LiquidityPoolToken.Decimals)))
                .ForMember(dest => dest.MinerBalance, opt => opt.MapFrom(src => src.MinerBalance.ToDecimal(TokenConstants.LiquidityPoolToken.Decimals)));

            CreateMap<StartMiningLog, StartMiningEventDto>()
                .IncludeBase<MineLog, MineEventDto>();

            CreateMap<StopMiningLog, StopMiningEventDto>()
                .IncludeBase<MineLog, MineEventDto>();

            CreateMap<CollectMiningRewardsLog, CollectMiningRewardsEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.Miner, opt => opt.MapFrom(src => src.Miner))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToDecimal(TokenConstants.Opdex.Decimals)));

            CreateMap<EnableMiningLog, EnableMiningEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.MiningPeriodEndBlock, opt => opt.MapFrom(src => src.MiningPeriodEndBlock))
                .ForMember(dest => dest.RewardRate, opt => opt.MapFrom(src => src.RewardRate.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToDecimal(TokenConstants.Opdex.Decimals)));

            // Token Events
            CreateMap<DistributionLog, DistributionEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.PeriodIndex, opt => opt.MapFrom(src => src.PeriodIndex))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.NextDistributionBlock, opt => opt.MapFrom(src => src.NextDistributionBlock))
                .ForMember(dest => dest.MiningGovernanceAmount, opt => opt.MapFrom(src => src.MiningAmount.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.VaultAmount, opt => opt.MapFrom(src => src.VaultAmount.ToDecimal(TokenConstants.Opdex.Decimals)));

            // Mining Governance Events
            CreateMap<NominationLog, NominationEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
                .ForMember(dest => dest.StakingPool, opt => opt.MapFrom(src => src.StakingPool))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight.ToDecimal(TokenConstants.Opdex.Decimals)));

            CreateMap<RewardMiningPoolLog, RewardMiningPoolEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
                .ForMember(dest => dest.StakingPool, opt => opt.MapFrom(src => src.StakingPool))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToDecimal(TokenConstants.Opdex.Decimals)));

            // Vault Events
            CreateMap<SetPendingVaultOwnershipLog, SetPendingVaultOwnershipEventDto>()
                .IncludeBase<OwnershipLog, OwnershipEventDto>();

            CreateMap<ClaimPendingVaultOwnershipLog, ClaimPendingVaultOwnershipEventDto>()
                .IncludeBase<OwnershipLog, OwnershipEventDto>();

            CreateMap<CreateVaultCertificateLog, CreateVaultCertificateEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.Holder, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.VestedBlock, opt => opt.MapFrom(src => src.VestedBlock));

            CreateMap<RedeemVaultCertificateLog, RedeemVaultCertificateEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.Holder, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.VestedBlock, opt => opt.MapFrom(src => src.VestedBlock));

            CreateMap<RevokeVaultCertificateLog, RevokeVaultCertificateEventDto>()
                .IncludeBase<TransactionLog, TransactionEventDto>()
                .ForMember(dest => dest.OldAmount, opt => opt.MapFrom(src => src.OldAmount.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.NewAmount, opt => opt.MapFrom(src => src.NewAmount.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.Holder, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.VestedBlock, opt => opt.MapFrom(src => src.VestedBlock));

            CreateMap<TransactionQuote, TransactionQuoteDto>()
                .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.Result))
                .ForMember(dest => dest.Error, opt => opt.MapFrom(src => src.Error))
                .ForMember(dest => dest.GasUsed, opt => opt.MapFrom(src => src.GasUsed))
                .ForMember(dest => dest.Request, opt => opt.MapFrom(src => src.Request))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TransactionQuoteRequest, TransactionQuoteRequestDto>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method))
                .ForMember(dest => dest.Parameters, opt => opt.MapFrom(src => src.Parameters))
                .ForMember(dest => dest.Callback, opt => opt.MapFrom(src => src.Callback))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TransactionQuoteRequestParameter, TransactionQuoteRequestParameterDto>()
                .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value.Serialize()))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TransactionQuoteRequestDto, TransactionQuoteRequest>()
                .ConstructUsing((src, ctx) =>
                {
                    var parameters = src.Parameters.Select(p => ctx.Mapper.Map<TransactionQuoteRequestParameter>(p)).ToList();
                    return new TransactionQuoteRequest(src.Sender, src.To, src.Amount, src.Method, src.Callback, parameters);
                })
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TransactionQuoteRequestParameterDto, TransactionQuoteRequestParameter>()
                .ConstructUsing(src => new TransactionQuoteRequestParameter(src.Label, SmartContractMethodParameter.Deserialize(src.Value)))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
