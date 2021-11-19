using AutoMapper;
using Newtonsoft.Json;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Application.Abstractions.Models.MarketTokens;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Governances;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.WebApi.Models.Responses;
using Opdex.Platform.WebApi.Models.Responses.Blocks;
using Opdex.Platform.WebApi.Models.Responses.Governances;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Snapshots;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;
using Opdex.Platform.WebApi.Models.Responses.Markets;
using Opdex.Platform.WebApi.Models.Responses.MarketTokens;
using Opdex.Platform.WebApi.Models.Responses.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.OHLC;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Deployers;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Governances;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Markets;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vault;
using Opdex.Platform.WebApi.Models.Responses.Vaults;
using Opdex.Platform.WebApi.Models.Responses.Wallet;

namespace Opdex.Platform.WebApi.Mappers
{
    public class PlatformWebApiMapperProfile : Profile
    {
        public PlatformWebApiMapperProfile()
        {
            CreateMap<TokenDto, TokenResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Decimals))
                .ForMember(dest => dest.Sats, opt => opt.MapFrom(src => src.Sats))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TokensDto, TokensResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Tokens))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketTokenDto, MarketTokenResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.Symbol))
                .ForMember(dest => dest.Decimals, opt => opt.MapFrom(src => src.Decimals))
                .ForMember(dest => dest.Sats, opt => opt.MapFrom(src => src.Sats))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForMember(dest => dest.LiquidityPool, opt => opt.MapFrom(src => src.LiquidityPool))
                .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.Market))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketTokensDto, MarketTokensResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Tokens))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketTokenSnapshotsDto, MarketTokenSnapshotsResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Snapshots))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TokenSummaryDto, TokenSummaryResponseModel>()
                .ForMember(dest => dest.PriceUsd, opt => opt.MapFrom(src => src.PriceUsd))
                .ForMember(dest => dest.DailyPriceChangePercent, opt => opt.MapFrom(src => src.DailyPriceChangePercent))
                .ForMember(dest => dest.ModifiedBlock, opt => opt.MapFrom(src => src.ModifiedBlock))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TokenSnapshotDto, TokenSnapshotResponseModel>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TokenSnapshotsDto, TokenSnapshotsResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Snapshots))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolDto, LiquidityPoolResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.TransactionFee, opt => opt.MapFrom(src => src.TransactionFee))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolDto, LiquidityPoolTokenGroupResponseModel>()
                .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.CrsToken))
                .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.SrcToken))
                .ForMember(dest => dest.Lp, opt => opt.MapFrom(src => src.LpToken))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolsDto, LiquidityPoolsResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.LiquidityPools))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolSummaryDto, LiquidityPoolSummaryResponseModel>()
                .ForMember(dest => dest.Reserves, opt => opt.MapFrom(src => src.Reserves))
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.Rewards))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
                .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolSnapshotDto, LiquidityPoolSnapshotResponseModel>()
                .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.TransactionCount))
                .ForMember(dest => dest.Reserves, opt => opt.MapFrom(src => src.Reserves))
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.Rewards))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ReservesDto, ReservesResponseModel>()
                .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.Crs))
                .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Src))
                .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
                .ForMember(dest => dest.DailyUsdChangePercent, opt => opt.MapFrom(src => src.DailyUsdChangePercent))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RewardsDto, RewardsResponseModel>()
                .ForMember(dest => dest.ProviderDailyUsd, opt => opt.MapFrom(src => src.ProviderDailyUsd))
                .ForMember(dest => dest.MarketDailyUsd, opt => opt.MapFrom(src => src.MarketDailyUsd))
                .ForMember(dest => dest.TotalDailyUsd, opt => opt.MapFrom(src => src.ProviderDailyUsd + src.MarketDailyUsd))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<VolumeDto, VolumeResponseModel>()
                .ForMember(dest => dest.DailyUsd, opt => opt.MapFrom(src => src.DailyUsd))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<StakingDto, StakingResponseModel>()
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
                .ForMember(dest => dest.DailyWeightChangePercent, opt => opt.MapFrom(src => src.DailyWeightChangePercent))
                .ForMember(dest => dest.Nominated, opt => opt.MapFrom(src => src.Nominated))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<CostDto, CostResponseModel>()
                .ForMember(dest => dest.CrsPerSrc, opt => opt.MapFrom(src => src.CrsPerSrc))
                .ForMember(dest => dest.SrcPerCrs, opt => opt.MapFrom(src => src.SrcPerCrs))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ReservesSnapshotDto, ReservesSnapshotResponseModel>()
                .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.Crs))
                .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Src))
                .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<RewardsSnapshotDto, RewardsSnapshotResponseModel>()
                .ForMember(dest => dest.ProviderUsd, opt => opt.MapFrom(src => src.ProviderUsd))
                .ForMember(dest => dest.MarketUsd, opt => opt.MapFrom(src => src.MarketUsd))
                .ForMember(dest => dest.TotalUsd, opt => opt.MapFrom(src => src.MarketUsd + src.ProviderUsd))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<VolumeSnapshotDto, VolumeSnapshotResponseModel>()
                .ForMember(dest => dest.Crs, opt => opt.MapFrom(src => src.Crs))
                .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Src))
                .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<StakingSnapshotDto, StakingSnapshotResponseModel>()
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Usd, opt => opt.MapFrom(src => src.Usd))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<CostSnapshotDto, CostSnapshotResponseModel>()
                .ForMember(dest => dest.CrsPerSrc, opt => opt.MapFrom(src => src.CrsPerSrc))
                .ForMember(dest => dest.SrcPerCrs, opt => opt.MapFrom(src => src.SrcPerCrs))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<LiquidityPoolSnapshotsDto, LiquidityPoolSnapshotsResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Snapshots))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MiningPoolDto, MiningPoolResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.LiquidityPool, opt => opt.MapFrom(src => src.LiquidityPool))
                .ForMember(dest => dest.MiningPeriodEndBlock, opt => opt.MapFrom(src => src.MiningPeriodEndBlock))
                .ForMember(dest => dest.RewardPerBlock, opt => opt.MapFrom(src => src.RewardPerBlock.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.RewardPerLpt, opt => opt.MapFrom(src => src.RewardPerLpt.ToDecimal(TokenConstants.Opdex.Decimals)))
                .ForMember(dest => dest.TokensMining, opt => opt.MapFrom(src => src.TokensMining.ToDecimal(TokenConstants.LiquidityPoolToken.Decimals)))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MiningPoolsDto, MiningPoolsResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.MiningPools))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

            CreateMap<OhlcDto<decimal>, OhlcDecimalResponseModel>()
                .ForMember(dest => dest.Open, opt => opt.MapFrom(src => src.Open))
                .ForMember(dest => dest.High, opt => opt.MapFrom(src => src.High))
                .ForMember(dest => dest.Low, opt => opt.MapFrom(src => src.Low))
                .ForMember(dest => dest.Close, opt => opt.MapFrom(src => src.Close))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketSnapshotDto, MarketSnapshotResponseModel>()
                .ForMember(dest => dest.Liquidity, opt => opt.MapFrom(src => src.Liquidity))
                .ForMember(dest => dest.LiquidityDailyChange, opt => opt.MapFrom(src => src.LiquidityDailyChange))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Volume))
                .ForMember(dest => dest.Staking, opt => opt.MapFrom(src => src.Staking))
                .ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.Rewards))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketDto, MarketResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PendingOwner, opt => opt.MapFrom(src => src.PendingOwner))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.StakingToken, opt => opt.MapFrom(src => src.StakingToken))
                .ForMember(dest => dest.CrsToken, opt => opt.MapFrom(src => src.CrsToken))
                .ForMember(dest => dest.AuthPoolCreators, opt => opt.MapFrom(src => src.AuthPoolCreators))
                .ForMember(dest => dest.AuthProviders, opt => opt.MapFrom(src => src.AuthProviders))
                .ForMember(dest => dest.AuthTraders, opt => opt.MapFrom(src => src.AuthTraders))
                .ForMember(dest => dest.MarketFeeEnabled, opt => opt.MapFrom(src => src.MarketFeeEnabled))
                .ForMember(dest => dest.TransactionFee, opt => opt.MapFrom(src => src.TransactionFee))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MarketSnapshotsDto, MarketSnapshotsResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Snapshots))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<AddressAllowanceDto, ApprovedAllowanceResponseModel>()
                .ForMember(dest => dest.Allowance, opt => opt.MapFrom(src => src.Allowance))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Spender, opt => opt.MapFrom(src => src.Spender))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token));

            CreateMap<AddressBalanceDto, AddressBalanceResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token));

            CreateMap<AddressBalancesDto, AddressBalancesResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Balances))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<StakingPositionDto, StakingPositionResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.StakingToken, opt => opt.MapFrom(src => src.StakingToken))
                .ForMember(dest => dest.LiquidityPool, opt => opt.MapFrom(src => src.LiquidityPool));

            CreateMap<StakingPositionsDto, StakingPositionsResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Positions))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

            CreateMap<MiningPositionDto, MiningPositionResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.MiningToken, opt => opt.MapFrom(src => src.MiningToken))
                .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool));

            CreateMap<MiningPositionsDto, MiningPositionsResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Positions))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

            CreateMap<MiningGovernanceDto, MiningGovernanceResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.MinedToken, opt => opt.MapFrom(src => src.MinedToken))
                .ForMember(dest => dest.PeriodEndBlock, opt => opt.MapFrom(src => src.PeriodEndBlock))
                .ForMember(dest => dest.PeriodRemainingBlocks, opt => opt.MapFrom(src => src.PeriodRemainingBlocks))
                .ForMember(dest => dest.PeriodBlockDuration, opt => opt.MapFrom(src => src.PeriodBlockDuration))
                .ForMember(dest => dest.MiningPoolRewardPerPeriod, opt => opt.MapFrom(src => src.MiningPoolRewardPerPeriod))
                .ForMember(dest => dest.PeriodsUntilRewardReset, opt => opt.MapFrom(src => src.PeriodsUntilRewardReset))
                .ForMember(dest => dest.TotalRewardsPerPeriod, opt => opt.MapFrom(src => src.TotalRewardsPerPeriod))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<MiningGovernancesDto, MiningGovernancesResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Governances))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

            CreateMap<VaultDto, VaultResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PendingOwner, opt => opt.MapFrom(src => src.PendingOwner))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Genesis, opt => opt.MapFrom(src => src.Genesis))
                .ForMember(dest => dest.TokensLocked, opt => opt.MapFrom(src => src.TokensLocked))
                .ForMember(dest => dest.TokensUnassigned, opt => opt.MapFrom(src => src.TokensUnassigned))
                .ForMember(dest => dest.LockedToken, opt => opt.MapFrom(src => src.LockedToken))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<VaultsDto, VaultsResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Vaults))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

            CreateMap<BlockDto, BlockResponseModel>()
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
                .ForMember(dest => dest.MedianTime, opt => opt.MapFrom(src => src.MedianTime))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<CursorDto, CursorResponseModel>()
                .ForMember(dest => dest.Next, opt => opt.MapFrom(src => src.Next))
                .ForMember(dest => dest.Previous, opt => opt.MapFrom(src => src.Previous))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<VaultCertificateDto, VaultCertificateResponseModel>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.VestingStartBlock, opt => opt.MapFrom(src => src.VestingStartBlock))
                .ForMember(dest => dest.VestingEndBlock, opt => opt.MapFrom(src => src.VestingEndBlock))
                .ForMember(dest => dest.Redeemed, opt => opt.MapFrom(src => src.Redeemed))
                .ForMember(dest => dest.Revoked, opt => opt.MapFrom(src => src.Revoked));

            CreateMap<VaultCertificatesDto, VaultCertificatesResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Certificates))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor));

            // Transactions
            CreateMap<TransactionsDto, TransactionsResponseModel>()
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Transactions))
                .ForMember(dest => dest.Paging, opt => opt.MapFrom(src => src.Cursor))
                .ForAllOtherMembers(opt => opt.Ignore());

            // Transaction
            CreateMap<TransactionDto, TransactionResponseModel>()
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                .ForMember(dest => dest.NewContractAddress, opt => opt.MapFrom(src => src.NewContractAddress))
                .ForMember(dest => dest.Block, opt => opt.MapFrom(src => src.BlockDto))
                .ForMember(dest => dest.GasUsed, opt => opt.MapFrom(src => src.GasUsed))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events))
                .ForAllOtherMembers(opt => opt.Ignore());

            // Transaction Events
            CreateMap<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.EventType))
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder));

            CreateMap<OwnershipEventDto, OwnershipEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To));

            // Deployer Transaction Events
            CreateMap<ClaimPendingDeployerOwnershipEventDto, ClaimPendingDeployerOwnershipEventResponseModel>()
                .IncludeBase<OwnershipEventDto, OwnershipEventResponseModel>();

            CreateMap<SetPendingDeployerOwnershipEventDto, SetPendingDeployerOwnershipEventResponseModel>()
                .IncludeBase<OwnershipEventDto, OwnershipEventResponseModel>();

            CreateMap<CreateMarketEventDto, CreateMarketEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.Market))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Router, opt => opt.MapFrom(src => src.Router))
                .ForMember(dest => dest.AuthPoolCreators, opt => opt.MapFrom(src => src.AuthPoolCreators))
                .ForMember(dest => dest.AuthProviders, opt => opt.MapFrom(src => src.AuthProviders))
                .ForMember(dest => dest.AuthTraders, opt => opt.MapFrom(src => src.AuthTraders))
                .ForMember(dest => dest.TransactionFee, opt => opt.MapFrom(src => src.TransactionFee))
                .ForMember(dest => dest.StakingToken, opt => opt.MapFrom(src => src.StakingToken))
                .ForMember(dest => dest.EnableMarketFee, opt => opt.MapFrom(src => src.EnableMarketFee));

            // Market Transaction Events
            CreateMap<CreateLiquidityPoolEventDto, CreateLiquidityPoolEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.LiquidityPool, opt => opt.MapFrom(src => src.LiquidityPool))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token));

            CreateMap<ClaimPendingMarketOwnershipEventDto, ClaimPendingMarketOwnershipEventResponseModel>()
                .IncludeBase<OwnershipEventDto, OwnershipEventResponseModel>();

            CreateMap<SetPendingMarketOwnershipEventDto, SetPendingMarketOwnershipEventResponseModel>()
                .IncludeBase<OwnershipEventDto, OwnershipEventResponseModel>();

            CreateMap<ChangeMarketPermissionEventDto, ChangeMarketPermissionEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Permission, opt => opt.MapFrom(src => src.Permission))
                .ForMember(dest => dest.IsAuthorized, opt => opt.MapFrom(src => src.IsAuthorized));

            // Liquidity Pool Transaction Events
            CreateMap<ProvideEventDto, ProvideEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.AmountCrs, opt => opt.MapFrom(src => src.AmountCrs))
                .ForMember(dest => dest.AmountSrc, opt => opt.MapFrom(src => src.AmountSrc))
                .ForMember(dest => dest.AmountLpt, opt => opt.MapFrom(src => src.AmountLpt))
                .ForMember(dest => dest.TokenSrc, opt => opt.MapFrom(src => src.TokenSrc))
                .ForMember(dest => dest.TokenLp, opt => opt.MapFrom(src => src.TokenLp))
                .ForMember(dest => dest.TokenLpTotalSupply, opt => opt.MapFrom(src => src.TokenLpTotalSupply));

            CreateMap<AddLiquidityEventDto, AddLiquidityEventResponseModel>()
                .IncludeBase<ProvideEventDto, ProvideEventResponseModel>();

            CreateMap<RemoveLiquidityEventDto, RemoveLiquidityEventResponseModel>()
                .IncludeBase<ProvideEventDto, ProvideEventResponseModel>();

            CreateMap<SwapEventDto, SwapEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.AmountCrsIn, opt => opt.MapFrom(src => src.AmountCrsIn))
                .ForMember(dest => dest.AmountCrsOut, opt => opt.MapFrom(src => src.AmountCrsOut))
                .ForMember(dest => dest.AmountSrcIn, opt => opt.MapFrom(src => src.AmountSrcIn))
                .ForMember(dest => dest.AmountSrcOut, opt => opt.MapFrom(src => src.AmountSrcOut));

            CreateMap<CollectStakingRewardsEventDto, CollectStakingRewardsEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.Staker, opt => opt.MapFrom(src => src.Staker))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<StakeEventDto, StakeEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.Staker, opt => opt.MapFrom(src => src.Staker))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.TotalStaked, opt => opt.MapFrom(src => src.TotalStaked))
                .ForMember(dest => dest.StakerBalance, opt => opt.MapFrom(src => src.StakerBalance));

            CreateMap<StartStakingEventDto, StartStakingEventResponseModel>()
                .IncludeBase<StakeEventDto, StakeEventResponseModel>();

            CreateMap<StopStakingEventDto, StopStakingEventResponseModel>()
                .IncludeBase<StakeEventDto, StakeEventResponseModel>();

            // Mining Pool Transaction Events
            CreateMap<CollectMiningRewardsEventDto, CollectMiningRewardsEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.Miner, opt => opt.MapFrom(src => src.Miner))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<MineEventDto, MineEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.Miner, opt => opt.MapFrom(src => src.Miner))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
                .ForMember(dest => dest.MinerBalance, opt => opt.MapFrom(src => src.MinerBalance));

            CreateMap<StartMiningEventDto, StartMiningEventResponseModel>()
                .IncludeBase<MineEventDto, MineEventResponseModel>();

            CreateMap<StopMiningEventDto, StopMiningEventResponseModel>()
                .IncludeBase<MineEventDto, MineEventResponseModel>();

            CreateMap<EnableMiningEventDto, EnableMiningEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.RewardRate, opt => opt.MapFrom(src => src.RewardRate))
                .ForMember(dest => dest.MiningPeriodEndBlock, opt => opt.MapFrom(src => src.MiningPeriodEndBlock));

            // Token Transaction Events
            CreateMap<TransferEventDto, TransferEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<ApprovalEventDto, ApprovalEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Owner))
                .ForMember(dest => dest.Spender, opt => opt.MapFrom(src => src.Spender))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<DistributionEventDto, DistributionEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.VaultAmount, opt => opt.MapFrom(src => src.VaultAmount))
                .ForMember(dest => dest.GovernanceAmount, opt => opt.MapFrom(src => src.GovernanceAmount))
                .ForMember(dest => dest.PeriodIndex, opt => opt.MapFrom(src => src.PeriodIndex))
                .ForMember(dest => dest.TotalSupply, opt => opt.MapFrom(src => src.TotalSupply))
                .ForMember(dest => dest.NextDistributionBlock, opt => opt.MapFrom(src => src.NextDistributionBlock));

            // Governance Transaction Events
            CreateMap<RewardMiningPoolEventDto, RewardMiningPoolEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.StakingPool, opt => opt.MapFrom(src => src.StakingPool))
                .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

            CreateMap<NominationEventDto, NominationEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.StakingPool, opt => opt.MapFrom(src => src.StakingPool))
                .ForMember(dest => dest.MiningPool, opt => opt.MapFrom(src => src.MiningPool))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight));

            // Vaults Transaction Events
            CreateMap<ClaimPendingVaultOwnershipEventDto, ClaimPendingVaultOwnershipEventResponseModel>()
                .IncludeBase<OwnershipEventDto, OwnershipEventResponseModel>();

            CreateMap<SetPendingVaultOwnershipEventDto, SetPendingVaultOwnershipEventResponseModel>()
                .IncludeBase<OwnershipEventDto, OwnershipEventResponseModel>();

            CreateMap<CreateVaultCertificateEventDto, CreateVaultCertificateEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Holder, opt => opt.MapFrom(src => src.Holder))
                .ForMember(dest => dest.VestedBlock, opt => opt.MapFrom(src => src.VestedBlock));

            CreateMap<RevokeVaultCertificateEventDto, RevokeVaultCertificateEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.OldAmount, opt => opt.MapFrom(src => src.OldAmount))
                .ForMember(dest => dest.NewAmount, opt => opt.MapFrom(src => src.NewAmount))
                .ForMember(dest => dest.Holder, opt => opt.MapFrom(src => src.Holder))
                .ForMember(dest => dest.VestedBlock, opt => opt.MapFrom(src => src.VestedBlock));

            CreateMap<RedeemVaultCertificateEventDto, RedeemVaultCertificateEventResponseModel>()
                .IncludeBase<TransactionEventDto, TransactionEventResponseModel>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Holder, opt => opt.MapFrom(src => src.Holder))
                .ForMember(dest => dest.VestedBlock, opt => opt.MapFrom(src => src.VestedBlock));

            CreateMap<TransactionQuoteDto, TransactionQuoteResponseModel>()
                .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.Result))
                .ForMember(dest => dest.Error, opt => opt.MapFrom(src => src.Error))
                .ForMember(dest => dest.GasUsed, opt => opt.MapFrom(src => src.GasUsed))
                .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.Events))
                .ForMember(dest => dest.Request, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Request).Base64Encode()))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
